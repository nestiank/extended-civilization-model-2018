using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Text;
using System.IO;
using System.Reflection;

namespace CivModel
{
    class PrototypeLoader
    {
        public static readonly XNamespace Xmlns
            = @"https://git.kucatdog.net/true-history-committee/civiii-model-proto/wikis/external/PrototypeSchema";

        private static readonly XmlSchemaSet _schema = new XmlSchemaSet();

        static PrototypeLoader()
        {
            using (var reader = new StringReader(Properties.Resources.PrototypeSchema))
            {
                _schema.Add(null, XmlReader.Create(reader));
            }
        }

        private Dictionary<Type, GuidObjectPrototype> _prototypes = new Dictionary<Type, GuidObjectPrototype>();
        private Dictionary<Guid, GuidObjectPrototype> _dictGuidProto = new Dictionary<Guid, GuidObjectPrototype>();

        private Dictionary<Guid, XDocument> _candidates = new Dictionary<Guid, XDocument>();
        private Dictionary<Guid, GameConstants> _gameConstants = new Dictionary<Guid, GameConstants>();

        public GuidObjectPrototype TryGetPrototype(Guid guid)
        {
            if (_dictGuidProto.TryGetValue(guid, out var proto))
                return proto;
            else
                return null;
        }

        public GuidObjectPrototype TryGetPrototype(Type type)
        {
            if (_prototypes.TryGetValue(type, out var proto))
                return proto;
            else
                return null;
        }

        public GuidObjectPrototype GetPrototype(Type type)
        {
            if (TryGetPrototype(type) is GuidObjectPrototype proto)
                return proto;
            else
                throw new KeyNotFoundException("the prototype of specified type is not found");
        }

        public Proto GetPrototype<Proto>(Type type)
            where Proto : GuidObjectPrototype
        {
            if (GetPrototype(type) is Proto proto)
                return proto;
            else
                throw new KeyNotFoundException("the prototype of specified type cannot be cast into specified prototype");
        }

        public GameConstants TryGetGameConstants(Guid schemeGuid)
        {
            if (_gameConstants.TryGetValue(schemeGuid, out var constants))
                return constants;
            else
                return null;
        }

        public GameConstants GetGameConstants(Guid schemeGuid)
        {
            if (TryGetGameConstants(schemeGuid) is GameConstants constants)
                return constants;
            else
                throw new KeyNotFoundException("the GameConstants of specified scheme is not found");
        }

        public void AddData(TextReader input)
        {
            try
            {
                var document = XDocument.Load(input);
                document.Validate(_schema, null, true);

                var guid = Guid.Parse(document.Root.Attribute("guid").Value);
                if (_candidates.ContainsKey(guid))
                    throw new InvalidDataException("there is duplicated Scheme GUID in prototype packages");

                _candidates.Add(guid, document);
            }
            catch (XmlSchemaException e)
            {
                throw new InvalidDataException("invalid prototype data", e);
            }
        }

        public void EnablePackage(Guid guid, Type type)
        {
            if (_candidates.TryGetValue(guid, out var document) && document != null)
            {
                var name = document.Root.Attribute("name").Value;
                if (name != type.FullName)
                    throw new KeyNotFoundException("package data type name mismatch");

                foreach (var child in document.Root.Elements().Skip(1))
                {
                    LoadNode(child, type.Assembly, guid);
                }

                _candidates[guid] = null;
            }
        }

        private void LoadNode(XElement node, Assembly packageAssembly, Guid schemeGuid)
        {
            if (node.Name == Xmlns + "GameConstants")
            {
                if (_gameConstants.ContainsKey(schemeGuid))
                    throw new InvalidDataException("there is duplicated GameConstants for the same game scheme.");

                _gameConstants[schemeGuid] = new GameConstants(node);
            }
            else
            {
                GuidObjectPrototype proto;

                if (node.Name == Xmlns + "City")
                    proto = new CityPrototype(node, packageAssembly);
                else if (node.Name == Xmlns + "TileBuilding")
                    proto = new TileBuildingPrototype(node, packageAssembly);
                else if (node.Name == Xmlns + "InteriorBuilding")
                    proto = new InteriorBuildingPrototype(node, packageAssembly);
                else if (node.Name == Xmlns + "Unit")
                    proto = new UnitPrototype(node, packageAssembly);
                else if (node.Name == Xmlns + "Quest")
                    proto = new QuestPrototype(node, packageAssembly);
                else if (node.Name == Xmlns + "Ending")
                    proto = new EndingPrototype(node, packageAssembly);
                else
                    throw new NotImplementedException();

                if (_prototypes.ContainsKey(proto.TargetType))
                    throw new InvalidDataException("there is duplicated Type in prototype data");
                if (_dictGuidProto.ContainsKey(proto.Guid))
                    throw new InvalidDataException("there is duplicated GUID in prototype data");

                _prototypes.Add(proto.TargetType, proto);
                _dictGuidProto.Add(proto.Guid, proto);
            }
        }
    }
}
