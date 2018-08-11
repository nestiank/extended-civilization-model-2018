using System;
using System.Xml.Linq;
using System.Reflection;

namespace CivModel
{
    class QuestPrototype : GuidObjectPrototype
    {
        public int PostingTurn { get; }
        public int LimitTurn { get; }
        public string GoalNotice { get; }
        public string RewardNotice { get; }
        public string CompleteNotice { get; }

        public QuestPrototype(XElement node, Assembly packageAssembly)
            : base(node, packageAssembly)
        {
            var xmlns = PrototypeLoader.Xmlns;
            PostingTurn = ParseQuestTurn(node.Element(xmlns + "PostingTurn").Value);
            LimitTurn = ParseQuestTurn(node.Element(xmlns + "LimitTurn").Value);
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
