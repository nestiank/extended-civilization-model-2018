using System;
using System.Xml.Linq;
using System.Reflection;

namespace CivModel
{
    /// <summary>
    /// Represents a prototype of <see cref="InteriorBuilding"/>.
    /// </summary>
    /// <seealso cref="ProductionResultPrototype"/>
    /// <seealso cref="InteriorBuilding"/>
    public class InteriorBuildingPrototype : ProductionResultPrototype
    {
        /// <summary>
        /// The amount of gold logistics of this actor.
        /// </summary>
        public double GoldLogistics { get; }

        /// <summary>
        /// The amount of gold this building provides.
        /// </summary>
        public double ProvidedLabor { get; }

        /// <summary>
        /// The amount of happiness this building provides.
        /// </summary>
        public double ResearchCapacity { get; }

        /// <summary>
        /// The amount of labor this building provides.
        /// </summary>
        public double ResearchIncome { get; }

        /// <summary>
        /// The population coefficient for the city where this building is.
        /// </summary>
        public double PopulationCoefficient { get; }

        internal InteriorBuildingPrototype(XElement node, Assembly packageAssembly)
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
