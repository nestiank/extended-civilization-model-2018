using System;
using System.Xml.Linq;

namespace CivModel
{
    /// <summary>
    /// Represents a prototype of <see cref="QuestProgress"/>.
    /// </summary>
    /// <seealso cref="QuestProgress"/>
    public class QuestProgressPrototype
    {
        /// <summary>
        /// The identifier of this progress. This value is unique per <see cref="Quest"/>.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The description of this progress.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The maximum value of this progress.
        /// </summary>
        public int MaxValue { get; }

        /// <summary>
        /// Whether this progress is enabled.
        /// </summary>
        public bool Enabled { get; }

        internal QuestProgressPrototype(XElement node)
        {
            Id = node.Attribute("id").Value;
            Description = node.Attribute("description").Value;
            MaxValue = Convert.ToInt32(node.Attribute("maxValue").Value);
            Enabled = Convert.ToBoolean(node.Attribute("enabled").Value);
        }
    }
}
