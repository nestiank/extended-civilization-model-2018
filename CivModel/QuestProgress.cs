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
        public string Description { get; set; }

        /// <summary>
        /// Whether this progress is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The maximum value of this progress.
        /// </summary>
        /// <seealso cref="Value"/>
        public int MaxValue
        {
            get => _maxValue;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "value is negative");

                if (_value > value)
                    _value = value;
                _maxValue = value;
            }
        }
        private int _maxValue = 0;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestProgress"/> class.
        /// </summary>
        /// <param name="quest">The quest.</param>
        /// <param name="id">The identifier of progress.</param>
        public QuestProgress(Quest quest, string id)
        {
            Quest = quest;
            Id = id;
        }

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
            Enabled = proto.Enabled;
        }

        /// <summary>
        /// Increment <see cref="Value"/> by <paramref name="amount"/> safely.
        /// If <c><see cref="Value"/> + <paramref name="amount"/></c> is bigger than <see cref="MaxValue"/>,
        ///  set <see cref="Value"/> by <see cref="MaxValue"/>.
        /// </summary>
        /// <param name="amount">the amount to increment</param>
        public void SafeIncrement(int amount = 1)
        {
            Value = Math.Min(MaxValue, Value + amount);
        }

        /// <summary>
        /// Set <see cref="Value"/> by <paramref name="value"/> safely.
        /// If <paramref name="value"/> is less than zero or bigger than <see cref="MaxValue"/>,
        ///  set <see cref="Value"/> by zero or <see cref="MaxValue"/> respectively.
        /// </summary>
        /// <param name="value"></param>
        public void SafeSetValue(int value)
        {
            Value = Math.Max(0, Math.Min(MaxValue, value));
        }
    }
}
