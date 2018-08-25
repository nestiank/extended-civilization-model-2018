using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Reflection;

namespace CivModel
{
    /// <summary>
    /// Represents a prototype of <see cref="Quest"/>.
    /// </summary>
    /// <seealso cref="GuidObjectPrototype"/>
    /// <seealso cref="Quest"/>
    public class QuestPrototype : GuidObjectPrototype
    {
        /// <summary>
        /// Whether the quest is visible to user or not.
        /// </summary>
        public bool IsVisible { get; }

        /// <summary>
        /// [퀘스트 게시 기간]. <c>-1</c> if forever.
        /// </summary>
        public int PostingTurn { get; }

        /// <summary>
        /// [퀘스트 제한 기간]. <c>-1</c> if forever.
        /// </summary>
        public int LimitTurn { get; }

        /// <summary>
        /// [퀘스트 설명].
        /// </summary>
        public string QuestDescription { get; }

        /// <summary>
        /// [퀘스트 조건].
        /// </summary>
        public string GoalNotice { get; }

        /// <summary>
        /// [퀘스트 보상].
        /// </summary>
        public string RewardNotice { get; }

        /// <summary>
        /// [교육용 알림].
        /// </summary>
        public string CompleteNotice { get; }

        /// <summary>
        /// The list of progress of this quest.
        /// </summary>
        public IReadOnlyList<QuestProgressPrototype> Progresses;

        internal QuestPrototype(XElement node, Assembly packageAssembly)
            : base(node, packageAssembly)
        {
            IsVisible = Convert.ToBoolean(node.Attribute("visible").Value);

            var xmlns = PrototypeLoader.Xmlns;
            PostingTurn = ParseQuestTurn(node.Element(xmlns + "PostingTurn").Value);
            LimitTurn = ParseQuestTurn(node.Element(xmlns + "LimitTurn").Value);
            QuestDescription = node.Element(xmlns + "QuestDescription").Value;
            GoalNotice = node.Element(xmlns + "GoalNotice").Value;
            RewardNotice = node.Element(xmlns + "RewardNotice").Value;
            CompleteNotice = node.Element(xmlns + "CompleteNotice").Value;

            var idSet = new HashSet<string>();
            var list = new List<QuestProgressPrototype>();
            foreach (var child in node.Elements(xmlns + "Progress"))
            {
                var proto = new QuestProgressPrototype(child);
                if (idSet.Add(proto.Id))
                {
                    list.Add(proto);
                }
                else
                {
                    throw new InvalidDataException("there is duplicated id for quest progress");
                }
            }
            Progresses = list;
        }

        private static int ParseQuestTurn(string str)
        {
            if (str == "forever")
                return -1;
            else
                return Convert.ToInt32(str);
        }
    }
}
