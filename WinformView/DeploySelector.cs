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
    public partial class DeploySelector : Form
    {
        private class SelectionObject
        {
            public string Name;
            public Action<Terrain.Point> Placer;

            public SelectionObject(string name, Action<Terrain.Point> placer)
            {
                Name = name;
                Placer = placer;
            }
            public override string ToString() => Name;
        }

        private Game _game;
        private Action _onClose;

        public DeploySelector(Game game, Action onClose)
        {
            _game = game;
            _onClose = onClose;

            InitializeComponent();
        }

        private void DeploySelector_Load(object sender, EventArgs e)
        {
            Action<Terrain.Point> wrapper(Action<CityBase> placer)
            {
                return pt => {
                    if (pt.TileBuilding is CityBase city)
                        placer(city);
                };
            }

            var player = _game.PlayerInTurn;
            var ar = new object[] {
                new SelectionObject("CityCenter", pt => new CityCenter(player, pt)),
                new SelectionObject("FakeKnight", pt => new FakeKnight(player, pt)),
                new SelectionObject("Pioneer", pt => new Pioneer(player, pt)),
                new SelectionObject("FakeFactory", wrapper(city => new FakeFactory(city)))
            };
            lbxSelection.Items.AddRange(ar);
        }

        private void DeploySelector_FormClosed(object sender, FormClosedEventArgs e)
        {
            _onClose();
        }

        public void Deploy(Terrain.Point pt)
        {
            if (lbxSelection.SelectedItem is SelectionObject sel)
            {
                sel.Placer(pt);
            }
        }
    }
}
