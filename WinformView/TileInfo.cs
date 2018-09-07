using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CivModel;
using CivModel.FakeModule;

namespace WinformView
{
    public partial class TileInfo : Form
    {
        private Game _game;
        private Terrain.Point _tile;

        private TextBox tbCity = null;

        private struct InteriorSelection
        {
            public InteriorBuilding Building;
            public InteriorSelection(InteriorBuilding building)
            {
                Building = building;
            }
            public override string ToString() => Building.GetType().Name;
        }
        private struct PlayerSelection
        {
            public string Name;
            public Player Player;
            public PlayerSelection(string name, Player player)
            {
                Name = name;
                Player = player;
            }
            public override string ToString() => Name;
        }

        public TileInfo(Game game, Terrain.Point tile)
        {
            _game = game;
            _tile = tile;

            InitializeComponent();
        }

        private void TileInfo_Load(object sender, EventArgs e)
        {
            tbUnit.Text = _tile.Unit?.GetType().FullName ?? "(null)";

            tbTileBuilding.Text = _tile.TileBuilding?.GetType().FullName ?? "(null)";

            int len = Enum.GetNames(typeof(TerrainType)).Length;
            object[] arType = new object[len];
            for (int i = 0; i < arType.Length; ++i)
            {
                arType[i] = (TerrainType)i;
            }
            cbxTileType.Items.AddRange(arType);
            cbxTileType.SelectedIndex = (int)_tile.Type;

            if (_tile.TileBuilding is CityBase city)
            {
                lbPlayer.Text = "도시 이름 : ";
                tbCity = new TextBox();
                tbCity.Location = cbxPlayer.Location;
                tbCity.Size = cbxPlayer.Size;
                tbCity.TabIndex = cbxPlayer.TabIndex;
                tbCity.Text = city.CityName;
                cbxPlayer.Enabled = false;
                cbxPlayer.Visible = false;
                Controls.Add(tbCity);

                tbPopulation.Enabled = true;
                tbPopulation.ReadOnly = false;
                tbPopulation.Text = city.Population.ToString();

                lbxInterior.Enabled = true;
                var range = city.InteriorBuildings
                    .Select(building => (object)new InteriorSelection(building))
                    .ToArray();
                lbxInterior.Items.AddRange(range);
            }
            else
            {
                var players = _game.Players.Select((p, i) => (object)new PlayerSelection("player " + i, p)).ToArray();
                cbxPlayer.Items.AddRange(players);
                cbxPlayer.SelectedIndex = Array.FindIndex(players, p => p == _tile.TileOwner);
            }

            if (_tile.TileBuilding != null)
            {
                cbxPlayer.Enabled = false;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _tile.Type = (TerrainType)cbxTileType.SelectedItem;

            if (tbCity != null)
            {
                var city = (CityBase)_tile.TileBuilding;
                try
                {
                    city.SetCityName(tbCity.Text);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("도시 이름이 잘못됬거나 이미 존재하는 도시 이름입니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                    tbCity.Text = city.CityName;
                }

                try
                {
                    city.Population = Convert.ToDouble(tbPopulation.Text);
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException || ex is ArgumentOutOfRangeException)
                {
                    MessageBox.Show("인구는 1 이상의 실수여야 합니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                    tbPopulation.Text = city.Population.ToString();
                }
            }
            else
            {
                if (cbxPlayer.SelectedItem is PlayerSelection sel)
                {
                    if (sel.Player != _tile.TileOwner)
                        sel.Player.TryAddTerritory(_tile);
                }
            }
        }

        private void btnInteriorDelete_Click(object sender, EventArgs e)
        {
            if (lbxInterior.SelectedItem is InteriorSelection sel)
            {
                sel.Building.Destroy();
                lbxInterior.Items.Remove(lbxInterior.SelectedItem);
            }
        }
    }
}
