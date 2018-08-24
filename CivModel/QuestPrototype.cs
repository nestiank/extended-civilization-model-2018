using System;
using System.Xml.Linq;
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

        internal QuestPrototype(XElement node, Assembly packageAssembly)
            : base(node, packageAssembly)
        {
            var xmlns = PrototypeLoader.Xmlns;
            PostingTurn = ParseQuestTurn(node.Element(xmlns + "PostingTurn").Value);
            LimitTurn = ParseQuestTurn(node.Element(xmlns + "LimitTurn").Value);
            QuestDescription = node.Element(xmlns + "QuestDescription").Value;
            GoalNotice = node.Element(xmlns + "GoalNotice").Value;
            RewardNotice = node.Element(xmlns + "RewardNotice").Value;
            CompleteNotice = node.Element(xmlns + "CompleteNotice").Value;
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
