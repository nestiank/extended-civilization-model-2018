using System;
using System.Xml.Linq;
using System.Reflection;

namespace CivModel
{
    /// <summary>
    /// Represents a prototype of <see cref="Ending"/>.
    /// </summary>
    /// <seealso cref="GuidObjectPrototype"/>
    /// <seealso cref="Ending"/>
    public class EndingPrototype : GuidObjectPrototype
    {
        /// <summary>
        /// The type of this ending.
        /// </summary>
        public EndingType Type { get; }

        internal EndingPrototype(XElement node, Assembly packageAssembly)
            : base(node, packageAssembly)
        {
            Type = ToEndingType(node.Attribute("type").Value);
        }

        private static EndingType ToEndingType(string str)
        {
            switch (str)
            {
                case "victory": return EndingType.Victory;
                case "draw": return EndingType.Draw;
                case "defeat": return EndingType.Defeat;
                default: throw new ArgumentException("argument cannot be converted into EndingType", nameof(str));
            }
        }
    }
}
