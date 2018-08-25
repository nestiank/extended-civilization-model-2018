using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public sealed class FakeQuest : Quest, IBattleObserver
    {
        private const string KillCount = "killCount";

        public FakeQuest(Player requester, Player requestee)
            : base(requester, requestee, typeof(FakeQuest))
        {
        }

        public override void OnQuestDeployTime()
        {
            Deploy();
        }

        protected override void OnAccept()
        {
            Game.BattleObservable.AddObserver(this, ObserverPriority.Model);
        }

        private void Cleanup()
        {
            Game.BattleObservable.RemoveObserver(this);

            Progresses[KillCount].Value = 0;
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        protected override void OnComplete()
        {
            for (int i = 0; i < 6; ++i)
            {
                var production = FakeKnightProductionFactory.Instance.Create(Requestee);
                production.IsCompleted = true;
                Requestee.Deployment.AddLast(production);
            }

            Requestee.SpecialResource[FakeResource.Instance] = 1;

            Cleanup();
        }

        public void OnBeforeBattle(Actor attacker, Actor defender)
        {
        }

        public void OnAfterBattle(Actor attacker, Actor defender, Player atkOwner, Player defOwner, BattleResult result)
        {
            if (atkOwner == Requestee && attacker is FakeKnight)
            {
                Progresses[KillCount].Value += 1;
                if (Progresses[KillCount].IsFull)
                {
                    Status = QuestStatus.Completed;
                }
            }
        }
    }
}
