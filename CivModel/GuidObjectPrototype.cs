using System;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.IO;

namespace CivModel
{
    class GuidObjectPrototype
    {
        public Assembly PackageAssembly { get; }
        public string PackageAssemblyQualifiedName => Name + ", " + PackageAssembly.FullName;

        public string Name { get; }
        public Guid Guid { get; }

        public Type TargetType { get; }

        public string TextName { get; }

        public GuidObjectPrototype(XElement node, Assembly packageAssembly)
        {
            PackageAssembly = packageAssembly;

            Name = node.Attribute("name").Value;
            Guid = Guid.Parse(node.Attribute("guid").Value);

            TargetType = Type.GetType(PackageAssemblyQualifiedName);
            if (TargetType == null)
                throw new InvalidDataException("there is invalid name attribute in prototype data");

            var xmlns = PrototypeLoader.Xmlns;
            TextName = node.Element(xmlns + "TextName").Value;
        }

        public object TryCreate(object[] param)
        {
            var type = Type.GetType(PackageAssemblyQualifiedName);
            var ctor = type.GetConstructor(param.Select(x => x.GetType()).ToArray());
            if (ctor == null || !ctor.IsPublic)
                return null;
            else
                return ctor.Invoke(param);
        }

        public object Create(object[] param)
        {
            if (TryCreate(param) is object obj)
                return obj;
            else
                throw new MissingMethodException("there is no constructor to call with specified arguments");
        }

        public object CreateOnTile(Player player, Terrain.Point pt)
        {
            var type = Type.GetType(PackageAssemblyQualifiedName);

            // without donation
            var ctor = type.GetConstructor(new Type[] { typeof(Player), typeof(Terrain.Point) });
            if (ctor != null && ctor.IsPublic)
                return ctor.Invoke(new object[] { player, pt });

            // with donation
            ctor = type.GetConstructor(new Type[] { typeof(Player), typeof(Terrain.Point), typeof(Player) });
            if (ctor != null && ctor.IsPublic)
                return ctor.Invoke(new object[] { player, pt, null });

            // InteriorBuilding
            if (pt.TileBuilding is CityBase city && city.Owner == player)
            {
                ctor = type.GetConstructor(new Type[] { typeof(CityBase) });
                if (ctor != null && ctor.IsPublic)
                    return ctor.Invoke(new object[] { city });
            }

            throw new MissingMethodException("there is no constructor to call");
        }
    }
}
