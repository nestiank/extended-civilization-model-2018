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
        public DeploySelector()
        {
            InitializeComponent();
        }

        private class SelectionObject
        {
            public string Name;
            public IProductionFactory Factory;

            public SelectionObject(string name, IProductionFactory factory)
            {
                Name = name;
                Factory = factory;
            }
            public override string ToString() => Name;
        }
        private class CityProductionFactory : IProductionFactory
        {
            private class CityProduction : Production
            {
                public CityProduction(IProductionFactory factory, Player owner) : base(factory, owner, 1, 1) { }
                public override bool IsPlacable(Terrain.Point point) => true;
                public override void Place(Terrain.Point point) => new CityCenter(Owner, point);
            }
            public Production Create(Player owner)
            {
                return new CityProduction(this, owner);
            }
        }
        private void DeploySelector_Load(object sender, EventArgs e)
        {
            var ar = new object[] {
                new SelectionObject("CityCenter", new CityProductionFactory())
            };
            lbxSelection.Items.AddRange(ar);
        }
    }
}
