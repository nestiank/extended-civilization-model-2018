using System;
using System.Xml.Linq;
using System.Reflection;

namespace CivModel
{
    class InteriorBuildingPrototype : ProductionResultPrototype
    {
        public double GoldLogistics { get; }
        public double ProvidedLabor { get; }
        public double ResearchCapacity { get; }
        public double ResearchIncome { get; }
        public double PopulationCoefficient { get; }

        public InteriorBuildingPrototype(XElement node, Assembly packageAssembly)
            : base(node, packageAssembly)
        {
            var xmlns = PrototypeLoader.Xmlns;
            GoldLogistics = Convert.ToDouble(node.Element(xmlns + "GoldLogistics").Value);
            ProvidedLabor = Convert.ToDouble(node.Element(xmlns + "ProvidedLabor").Value);
            ResearchCapacity = Convert.ToDouble(node.Element(xmlns + "ResearchCapacity").Value);
            ResearchIncome = Convert.ToDouble(node.Element(xmlns + "ResearchIncome").Value);
            PopulationCoefficient = Convert.ToDouble(node.Element(xmlns + "PopulationCoefficient").Value);
        }
    }
}
