using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;
using static CivModel.Zap.FishPlayerNumber;

namespace CivModel.Quests
{
    public class QuestAutismBeamReflex : Quest
    {
        private const string TecCount = "TecCount";

        public QuestAutismBeamReflex(Game game)
            : base(game.GetPlayerFish(), game.GetPlayerHwan(), typeof(QuestAutismBeamReflex))
        {
        }

        public override void OnQuestDeployTime()
        {
            var finno = Game.GetPlayerFinno();
            if (finno.SpecialResource[AutismBeamAmplificationCrystal.Instance] > 0)
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
            Progresses[TecCount].Value = 0;
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        protected override void OnComplete()
        {
            Requestee.SpecialResource[SpecialResourceAutismBeamReflex.Instance] = 1;

            Cleanup();
        }

        protected override void FixedPostTurn()
        {
            if (Status == QuestStatus.Accepted)
            {
                Progresses[TecCount].Value = (int)Math.Min(Requestee.Research, 8000);
                if (Requestee.Research >= 8000)
                {
                    Status = QuestStatus.Completed;
                }
            }

            base.FixedPostTurn();
        }
    }
}
