using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CivModel
{
    /// <summary>
    /// Represents one of progresses of <see cref="CivModel.Quest"/>.
    /// </summary>
    /// <seealso cref="CivModel.Quest"/>.
    public class QuestProgress
    {
        /// <summary>
        /// The <see cref="CivModel.Quest"/> which has this object.
        /// </summary>
        public Quest Quest { get; }

        /// <summary>
        /// The <see cref="CivModel.Game"/> object.
        /// </summary>
        public Game Game => Quest.Game;

        /// <summary>
        /// The identifier of this progress. This value is unique per <see cref="CivModel.Quest"/>.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The description of this progress.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// The maximum value of this progress.
        /// </summary>
        /// <seealso cref="Value"/>
        public int MaxValue { get; private set; }

        /// <summary>
        /// The current value of this progress.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">value is less than zero or bigger than MaxValue</exception>
        /// <seealso cref="MaxValue"/>
        public int Value
        {
            get => _value;
            set
            {
                if (value < 0 || value > MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value), "value is less than zero or bigger than MaxValue");
                _value = value;
            }
        }
        private int _value = 0;

        /// <summary>
        /// Whether this progress is full.
        /// This value is equal to <c><see cref="Value"/> == <see cref="MaxValue"/></c>.
        /// </summary>
        public bool IsFull => Value == MaxValue;

        internal QuestProgress(Quest quest, QuestProgressPrototype proto)
        {
            Quest = quest;
            ApplyPrototype(proto);
        }

        private void ApplyPrototype(QuestProgressPrototype proto)
        {
            Id = proto.Id;
            Description = proto.Description;
            MaxValue = proto.MaxValue;
        }
    }
}
