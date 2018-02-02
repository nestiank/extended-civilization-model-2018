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
using CivModel.Common;

namespace WinformView
{
    public partial class TileInfo : Form
    {
        private Game _game;
        private Terrain.Point _tile;

        private TextBox tbCity = null;

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

            if (_tile == _tile.TileOwnerCity?.PlacedPoint)
            {
                tbCity = new TextBox();
                tbCity.Location = cbxCity.Location;
                tbCity.Size = cbxCity.Size;
                tbCity.Text = _tile.TileOwnerCity.Name;
                cbxCity.Enabled = false;
                cbxCity.Visible = false;
                Controls.Add(tbCity);
            }
            else
            {
                var cities = _game.Players.SelectMany(player => player.Cities).ToList();
                cbxCity.DataSource = cities;
                cbxCity.DisplayMember = "Name";
                cbxCity.ValueMember = "Name";
                cbxCity.SelectedIndex = cities.IndexOf(_tile.TileOwnerCity);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _tile.Type = (TerrainType)cbxTileType.SelectedItem;

            if (tbCity != null)
            {
                try
                {
                    _tile.TileOwnerCity.Name = tbCity.Text;
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("도시 이름이 잘못됬거나 이미 존재하는 도시 이름입니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                    tbCity.Text = _tile.TileOwnerCity.Name;
                }
            }
            else
            {
                var selcity = (CityCenter)cbxCity.SelectedItem;
                if (selcity != _tile.TileOwnerCity)
                {
                    selcity.AddTerritory(_tile);
                }
            }
        }
    }
}
