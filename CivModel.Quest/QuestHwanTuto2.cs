using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Hwan.HwanPlayerNumber;

namespace CivModel.Quests
{
    public class QuestHwanTuto2 : Quest, IProductionObserver
    {
        private const string Product1 = "product1";
        private const string Product2 = "product2";

        public QuestHwanTuto2(Game game)
            : base(game.GetPlayerHwan(), game.GetPlayerHwan(), typeof(QuestHwanTuto2))
        {
        }

        public override void OnQuestDeployTime()
        {
            Deploy();Accept();
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
            var quest = Requestee.Quests.OfType<QuestHwanTuto3>().FirstOrDefault();
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
                var p1 = Requestee.Production.Count(p => p.Factory is Hwan.ProtoNinjaProductionFactory);
                var p2 = Requestee.Production.Count(p => p.Factory is Hwan.HwanEmpireKimchiFactoryProductionFactory);

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
