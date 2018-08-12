using System;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.IO;

namespace CivModel
{
    /// <summary>
    /// The base class represents a prototype.
    /// </summary>
    public class GuidObjectPrototype
    {
        /// <summary>
        /// The assembly from which this prototype is.
        /// </summary>
        public Assembly PackageAssembly { get; }

        /// <summary>
        /// The <see cref="Type.AssemblyQualifiedName"/> of <see cref="TargetType"/>.
        /// </summary>
        public string PackageAssemblyQualifiedName => Name + ", " + PackageAssembly.FullName;

        /// <summary>
        /// The name of <see cref="TargetType"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The unique identifier of <see cref="TargetType"/>.
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// The type of an object of the kind of this prototype.
        /// </summary>
        public Type TargetType { get; }

        /// <summary>
        /// The name of an object of the kind of this prototype.
        /// </summary>
        public string TextName { get; }

        internal GuidObjectPrototype(XElement node, Assembly packageAssembly)
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

        internal object TryCreate(object[] param)
        {
            var type = Type.GetType(PackageAssemblyQualifiedName);
            var ctor = type.GetConstructor(param.Select(x => x.GetType()).ToArray());
            if (ctor == null || !ctor.IsPublic)
                return null;
            else
                return ctor.Invoke(param);
        }

        internal object Create(object[] param)
        {
            if (TryCreate(param) is object obj)
                return obj;
            else
                throw new MissingMethodException("there is no constructor to call with specified arguments");
        }

        internal object CreateOnTile(Player player, Terrain.Point pt)
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
