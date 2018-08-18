using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Hwan.HwanPlayerNumber;
using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Zap.AtlantisPlayerNumber;

namespace CivModel.Quests
{
    public class QuestPorjectCthulhu : Quest, Hwan.ISpyRelatedQuest
    {
        public QuestPorjectCthulhu(Game game)
            : base(game.GetPlayerAtlantis(), game.GetPlayerHwan(), typeof(QuestPorjectCthulhu))
        {
        }

        public override void OnQuestDeployTime()
        {
            if (Requestee.SpecialResource[SpecialResourceAutismBeamReflex.Instance] > 0)
            {
                if (Game.Random.Next(10) < 7)
                    Deploy();
            }
        }

        protected override void OnAccept()
        {
        }

        private void Cleanup()
        {
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        protected override void OnComplete()
        {
            Requestee.SpecialResource[SpecialResourceCthulhuProjectInfo.Instance] = 1;

            foreach (Player player in Game.Players)
            {
                if(player.Team == Game.Players[0].Team)
                {
                    foreach (Unit unit in player.Units)
                    {
                        unit.DefencePower = unit.DefencePower * 2;
                    }
                }
            }

            Cleanup();
        }

        void Hwan.ISpyRelatedQuest.OnSpyAction(Hwan.Spy spy)
        {
            if (Status == QuestStatus.Accepted && spy.PlacedPoint is Terrain.Point pt)
            {
                if (spy.Owner == Requestee && pt.TileOwner == Game.GetPlayerFinno())
                {
                    Complete();
                }
            }
        }
    }
}
