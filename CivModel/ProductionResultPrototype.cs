using System;
using System.Xml.Linq;
using System.Reflection;

namespace CivModel
{
    class ProductionResultPrototype : GuidObjectPrototype
    {
        public double TotalLaborCost { get; }
        public double LaborCapacityPerTurn { get; }
        public double TotalGoldCost { get; }
        public double GoldCapacityPerTurn { get; }

        public ProductionResultPrototype(XElement node, Assembly packageAssembly)
            : base(node, packageAssembly)
        {
            var xmlns = PrototypeLoader.Xmlns;
            TotalLaborCost = Convert.ToDouble(node.Element(xmlns + "TotalLaborCost").Value);
            LaborCapacityPerTurn = Convert.ToDouble(node.Element(xmlns + "LaborCapacityPerTurn").Value);
            TotalGoldCost = Convert.ToDouble(node.Element(xmlns + "TotalGoldCost").Value);
            GoldCapacityPerTurn = Convert.ToDouble(node.Element(xmlns + "GoldCapacityPerTurn").Value);
        }
    }
}
