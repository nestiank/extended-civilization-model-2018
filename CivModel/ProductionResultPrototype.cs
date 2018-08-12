using System;
using System.Xml.Linq;
using System.Reflection;

namespace CivModel
{
    /// <summary>
    /// Represents a prototype of <see cref="IProductionResult"/>.
    /// </summary>
    /// <seealso cref="GuidObjectPrototype"/>
    /// <seealso cref="IProductionResult"/>
    public class ProductionResultPrototype : GuidObjectPrototype
    {
        /// <summary>
        /// The total labor cost to finish this production.
        /// </summary>
        public double TotalLaborCost { get; }

        /// <summary>
        /// The maximum labor which can put into this production per turn.
        /// </summary>
        public double LaborCapacityPerTurn { get; }

        /// <summary>
        /// The total gold cost to finish this production.
        /// </summary>
        public double TotalGoldCost { get; }

        /// <summary>
        /// The maximum gold which can put into this production per turn.
        /// </summary>
        public double GoldCapacityPerTurn { get; }

        internal ProductionResultPrototype(XElement node, Assembly packageAssembly)
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
