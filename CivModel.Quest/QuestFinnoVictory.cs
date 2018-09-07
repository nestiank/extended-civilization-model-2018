using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;

namespace CivModel.Quests
{
    public class QuestFinnoVictory : QuestUltimateBase
    {
        protected override string DelayProgress => "delay";

        protected override List<KeyValuePair<string, ISpecialResource>> RequiredResources { get; }
            = new List<KeyValuePair<string, ISpecialResource>>();

        public QuestFinnoVictory(Game game)
            : base(null, game.GetPlayerFinno(), typeof(QuestFinnoVictory))
        {
            RequiredResources.Add(new KeyValuePair<string, ISpecialResource>("autism", AutismBeamAmplificationCrystal.Instance));
            RequiredResources.Add(new KeyValuePair<string, ISpecialResource>("necro", Necronomicon.Instance));
            RequiredResources.Add(new KeyValuePair<string, ISpecialResource>("rlyeh", GatesOfRlyeh.Instance));
        }

        protected override QuestUltimateBase GetEnemyUltimateQuest()
        {
            return Game.GetPlayerHwan().Quests.OfType<QuestHwanVictory>().FirstOrDefault();
        }
    }
}
