using System;
using System.Xml.Linq;
using System.Reflection;

namespace CivModel
{
    class CityPrototype : TileBuildingPrototype
    {
        public double InitialPopulation { get; }

        public CityPrototype(XElement node, Assembly packageAssembly)
            : base(node, packageAssembly)
        {
            var xmlns = PrototypeLoader.Xmlns;
            InitialPopulation = Convert.ToDouble(node.Element(xmlns + "InitialPopulation").Value);
        }
    }
}
