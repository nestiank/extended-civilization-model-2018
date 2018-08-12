using System;
using System.Xml.Linq;
using System.Reflection;

namespace CivModel
{
    /// <summary>
    /// Represents a prototype of city.
    /// </summary>
    /// <seealso cref="CivModel.TileBuildingPrototype"/>
    /// <seealso cref="CityBase"/>
    public class CityPrototype : TileBuildingPrototype
    {
        /// <summary>
        /// The initial population of city.
        /// </summary>
        public double InitialPopulation { get; }

        internal CityPrototype(XElement node, Assembly packageAssembly)
            : base(node, packageAssembly)
        {
            var xmlns = PrototypeLoader.Xmlns;
            InitialPopulation = Convert.ToDouble(node.Element(xmlns + "InitialPopulation").Value);
        }
    }
}
