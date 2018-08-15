using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Hwan.HwanPlayerNumber;
using static CivModel.Zap.EgyptPlayerNumber;

namespace CivModel.Quests
{
    public class QuestEgyptKingdom : Quest, ITileObjectObserver
    {
        public QuestEgyptKingdom(Game game)
            : base(game.GetPlayerEgypt(), game.GetPlayerHwan(), typeof(QuestEgyptKingdom))
        {
        }

        public override void OnQuestDeployTime()
        {
            if (Requestee.SpecialResource[SpecialResourceCthulhuProjectInfo.Instance] > 0)
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
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        protected override void OnComplete()
        {
            Requestee.SpecialResource[SpecialResourceAlienCommunication.Instance] = 1;

            Cleanup();
        }

        public void TileObjectProduced(TileObject obj)
        {
            if(obj is CivModel.Hwan.Preternaturality pyramid && pyramid.Owner == Requester && pyramid.Donator == Requestee)
            {
                Status = QuestStatus.Completed;
            }
        }

        public void TileObjectPlaced(TileObject obj) {}
    }
}
