using System;
using System.Xml.Linq;
using System.Reflection;

namespace CivModel
{
    /// <summary>
    /// Represents a prototype of <see cref="TileBuilding"/>.
    /// </summary>
    /// <seealso cref="ActorPrototype"/>
    /// <seealso cref="TileBuilding"/>
    public class TileBuildingPrototype : ActorPrototype
    {
        /// <summary>
        /// The amount of gold this building provides.
        /// </summary>
        public double ProvidedGold { get; }

        /// <summary>
        /// The amount of happiness this building provides.
        /// </summary>
        public double ProvidedHappiness { get; }

        /// <summary>
        /// The labor which this tile building offers.
        /// </summary>
        public double ProvidedLabor { get; }

        internal TileBuildingPrototype(XElement node, Assembly packageAssembly)
            : base(node, packageAssembly)
        {
            var xmlns = PrototypeLoader.Xmlns;
            ProvidedGold = Convert.ToDouble(node.Element(xmlns + "ProvidedGold").Value);
            ProvidedHappiness = Convert.ToDouble(node.Element(xmlns + "ProvidedHappiness").Value);
            ProvidedLabor = Convert.ToDouble(node.Element(xmlns + "ProvidedLabor").Value);
        }
    }
}
