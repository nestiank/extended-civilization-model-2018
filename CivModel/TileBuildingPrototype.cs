using System;
using System.Xml.Linq;
using System.Reflection;

namespace CivModel
{
    class TileBuildingPrototype : ActorPrototype
    {
        public double ProvidedGold { get; }
        public double ProvidedHappiness { get; }
        public double ProvidedLabor { get; }

        public TileBuildingPrototype(XElement node, Assembly packageAssembly)
            : base(node, packageAssembly)
        {
            var xmlns = PrototypeLoader.Xmlns;
            ProvidedGold = Convert.ToDouble(node.Element(xmlns + "ProvidedGold").Value);
            ProvidedHappiness = Convert.ToDouble(node.Element(xmlns + "ProvidedHappiness").Value);
            ProvidedLabor = Convert.ToDouble(node.Element(xmlns + "ProvidedLabor").Value);
        }
    }
}
