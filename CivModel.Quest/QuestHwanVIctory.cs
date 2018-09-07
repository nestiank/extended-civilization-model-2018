using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;

namespace CivModel.Quests
{
    public class QuestHwanVictory : QuestUltimateBase
    {
        protected override string DelayProgress => "delay";

        protected override List<KeyValuePair<string, ISpecialResource>> RequiredResources { get; }
            = new List<KeyValuePair<string, ISpecialResource>>();

        private const string Autism = "autism";
        private const string Cthulhu = "cthulhu";
        private const string Alien = "alien";
        private const string Delay = "delay";

        public QuestHwanVictory(Game game)
            : base(null, game.GetPlayerHwan(), typeof(QuestHwanVictory))
        {
            RequiredResources.Add(new KeyValuePair<string, ISpecialResource>("autism", SpecialResourceAutismBeamReflex.Instance));
            RequiredResources.Add(new KeyValuePair<string, ISpecialResource>("cthulhu", SpecialResourceCthulhuProjectInfo.Instance));
            RequiredResources.Add(new KeyValuePair<string, ISpecialResource>("alien", SpecialResourceAlienCommunication.Instance));
        }

        protected override QuestUltimateBase GetEnemyUltimateQuest()
        {
            return Game.GetPlayerFinno().Quests.OfType<QuestFinnoVictory>().FirstOrDefault();
        }
    }
}
