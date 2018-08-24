using System;
using System.Xml.Linq;

namespace CivModel
{
    /// <summary>
    /// Represents the information about a skill of <see cref="Actor"/>.
    /// </summary>
    public struct SkillInfo
    {
        /// <summary>
        /// The skill name
        /// </summary>
        public string SkillName;

        /// <summary>
        /// The skill description
        /// </summary>
        public string SkillDescription;

        internal SkillInfo(XElement node)
        {
            var xmlns = PrototypeLoader.Xmlns;
            SkillName = node.Attribute("name").Value;
            SkillDescription = node.Attribute("description").Value;
        }
    }
}
