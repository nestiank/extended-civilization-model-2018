using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;

namespace CivModel.Quests
{
    public class QuestFinnoTuto2 : Quest, IProductionObserver
    {
        private const string Product1 = "product1";
        private const string Product2 = "product2";

        public QuestFinnoTuto2(Game game)
            : base(game.GetPlayerFinno(), game.GetPlayerFinno(), typeof(QuestFinnoTuto2))
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
            var p1 = Requestee.Production.Reverse()
                .Where(p => p.Factory is Finno.JediKnightProductionFactory)
                .Take(Progresses[Product1].MaxValue);
            var p2 = Requestee.Production.Reverse()
                .Where(p => p.Factory is Finno.AncientFinnoGermaniumMineProductionFactory)
                .Take(Progresses[Product2].MaxValue);
            foreach (var production in p1.Concat(p2).ToArray())
            {
                Requestee.Production.Remove(production);
                production.IsCompleted = true;
                Requestee.Deployment.AddLast(production);
            }

            var quest = Requestee.Quests.OfType<QuestFinnoTuto3>().FirstOrDefault();
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

        public void OnProductionListChanged(Player player)
        {
            if (player == Requestee)
            {
                var p1 = Requestee.Production.Count(p => p.Factory is Finno.JediKnightProductionFactory);
                var p2 = Requestee.Production.Count(p => p.Factory is Finno.AncientFinnoGermaniumMineProductionFactory);

                Progresses[Product1].SafeSetValue(p1);
                Progresses[Product2].SafeSetValue(p2);

                if (IsTotalProgressFull)
                    Complete();
            }
        }
        public void OnDeploymentListChanged(Player player) { }
        public void OnProductionDeploy(Terrain.Point point, Production production, object result) { }
    }
}
