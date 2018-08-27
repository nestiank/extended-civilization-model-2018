using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Zap.EmuPlayerNumber;

namespace CivModel.Quests
{
    public class QuestWarAliance : Quest, ITileObjectObserver
    {
        private const string ProductCount = "ProductCount";

        public QuestWarAliance(Game game)
            : base(game.GetPlayerEmu(), game.GetPlayerFinno(), typeof(QuestWarAliance))
        {
        }

        public override void OnQuestDeployTime()
        {
            if (Requestee.Research >= 0)
            {
                if (Game.Random.Next(10) < 7)
                    Deploy();
            }
        }

        protected override void OnAccept()
        {
            Game.TileObjectObservable.AddObserver(this, ObserverPriority.Model);
        }

        private void Cleanup()
        {
            Game.TileObjectObservable.RemoveObserver(this);

            Progresses[ProductCount].Value = 0;
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        protected override void OnComplete()
        {
            Requestee.SpecialResource[AutismBeamAmplificationCrystal.Instance] = 1;

            Cleanup();
        }

        public void TileObjectProduced(TileObject obj)
        {
            if (obj is CivModel.Finno.AutismBeamDrone drone && drone.Owner == Game.GetPlayerFinno())
            {
                Progresses[ProductCount].Value += 1;
                if (Progresses[ProductCount].IsFull)
                {
                    Status = QuestStatus.Completed;
                }
            }
        }

        public void TileObjectPlaced(TileObject obj) { }
    }
}
