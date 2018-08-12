using System;
using System.Xml.Linq;
using System.Reflection;

namespace CivModel
{
    /// <summary>
    /// Represents a prototype of <see cref="Unit"/>.
    /// </summary>
    /// <seealso cref="ActorPrototype"/>
    /// <seealso cref="Unit"/>
    public class UnitPrototype : ActorPrototype
    {
        internal UnitPrototype(XElement node, Assembly packageAssembly)
            : base(node, packageAssembly)
        {
        }
    }
}
