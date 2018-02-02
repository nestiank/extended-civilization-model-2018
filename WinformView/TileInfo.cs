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

        public TileInfo(Game game, Terrain.Point tile)
        {
            _game = game;
            _tile = tile;

            InitializeComponent();
        }

        private void TileInfo_Load(object sender, EventArgs e)
        {
            tbUnit.Text = _tile.Unit?.GetType().FullName ?? "null";

            tbTileBuilding.Text = _tile.TileBuilding?.GetType().FullName ?? "null";

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
                cbxCity.DataSource = new CityCenter[] { _tile.TileOwnerCity };
                cbxCity.DisplayMember = "Name";
                cbxCity.ValueMember = "Name";
                cbxCity.SelectedIndex = 0;
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

            var selcity = (CityCenter)cbxCity.SelectedItem;
            if (selcity != _tile.TileOwnerCity)
            {
                selcity.AddTerritory(_tile);
            }
        }
    }
}
