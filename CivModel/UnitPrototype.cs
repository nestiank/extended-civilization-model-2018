using System;
using System.Xml.Linq;
using System.Reflection;

namespace CivModel
{
    class UnitPrototype : ActorPrototype
    {
        public UnitPrototype(XElement node, Assembly packageAssembly)
            : base(node, packageAssembly)
        {
        }
    }
}
