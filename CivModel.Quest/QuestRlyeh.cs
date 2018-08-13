using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Zap.AtlantisPlayerNumber;

namespace CivModel.Quests
{
    public class QuestRlyeh :Quest, ITileObjectObserver
    {
        public QuestRlyeh(Game game)
            : base(game.GetPlayerAtlantis(), game.GetPlayerFinno(), typeof(QuestRlyeh))
        {
        }

        public override void OnQuestDeployTime()
        {
            if (Requestee.SpecialResource[Necronomicon.Instance] > 0)
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
            Requestee.SpecialResource[GatesOfRlyeh.Instance] = 1;

            Cleanup();
        }

        public void TileObjectProduced(TileObject obj)
        {
            if (obj is CivModel.Finno.Preternaturality rlyeh && rlyeh.Owner == Requester && rlyeh.Donator == Requestee)
            {
                Status = QuestStatus.Completed;
            }
        }

        public void TileObjectPlaced(TileObject obj) { }
    }
}
