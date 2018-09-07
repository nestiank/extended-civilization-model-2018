using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;

namespace CivModel.Quests
{
    public class QuestFinnoTuto3 : Quest, IProductionObserver
    {
        private const string Deploy1 = "deploy1";
        private const string Deploy2 = "deploy2";

        public QuestFinnoTuto3(Game game)
            : base(game.GetPlayerFinno(), game.GetPlayerFinno(), typeof(QuestFinnoTuto3))
        {
        }

        public override void OnQuestDeployTime()
        {
        }

        protected override void OnAccept()
        {
            Game.ProductionObservable.AddObserver(this, ObserverPriority.Model);
        }

        private void Cleanup()
        {
            Game.ProductionObservable.RemoveObserver(this);
        }

        protected override void OnComplete()
        {
            var quest = Requestee.Quests.OfType<QuestFinnoTuto4>().FirstOrDefault();
            if (quest != null)
            {
                quest.Deploy();
                quest.Accept();
            }
            Cleanup();
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        public void OnProductionDeploy(Terrain.Point point, Production production, object result)
        {
            if (production.Owner == Requestee)
            {
                switch (production.Factory)
                {
                    case Finno.JediKnightProductionFactory factory:
                        Progresses[Deploy1].SafeIncrement();
                        break;
                    case Finno.AncientFinnoGermaniumMineProductionFactory factory:
                        Progresses[Deploy2].SafeIncrement();
                        break;
                }

                if (IsTotalProgressFull)
                    Complete();
            }
        }
        public void OnProductionListChanged(Player player) { }
        public void OnDeploymentListChanged(Player player) { }
    }
}
