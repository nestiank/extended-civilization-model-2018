using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Hwan.HwanPlayerNumber;

namespace CivModel.Quests
{
    public class QuestHwanTuto4 : Quest, ITurnObserver
    {
        private const string Happy = "happy";
        private const string Gold = "gold";
        private const string Research = "research";

        public QuestHwanTuto4(Game game)
            : base(game.GetPlayerHwan(), game.GetPlayerHwan(), typeof(QuestHwanTuto4))
        {
        }

        public override void OnQuestDeployTime()
        {
        }

        protected override void OnAccept()
        {
            Game.TurnObservable.AddObserver(this, ObserverPriority.Model);
            RetrieveProgress();
        }

        private void Cleanup()
        {
            Game.TurnObservable.RemoveObserver(this);
        }

        protected override void OnComplete()
        {
            for (int i = 0; i < 10; ++i)
            {
                Requestee.Deployment.AddLast(Hwan.HwanEmpireCityCentralLabProductionFactory.Instance.Create(Requestee));
            }

            var quest = Requestee.Quests.OfType<QuestHwanTuto5>().FirstOrDefault();
            if (quest != null)
            {
                quest.Deploy();
            }
            Cleanup();
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        private void RetrieveProgress()
        {
            Requestee.EstimateResourceInputs();

            Progresses[Happy].SafeSetValue((int)Requestee.HappinessIncome);
            Progresses[Gold].SafeSetValue((int)Requestee.GoldNetIncome);
            Progresses[Research].SafeSetValue((int)Requestee.ResearchIncome);

            if (IsTotalProgressFull)
                Complete();
        }

        public void PreSubTurn(Player playerInTurn)
        {
            if (playerInTurn == Requestee)
                RetrieveProgress();
        }

        public void PreTurn() { }
        public void AfterPreTurn() { }
        public void PostTurn() { }
        public void AfterPostTurn() { }
        public void AfterPreSubTurn(Player playerInTurn) { }
        public void PostSubTurn(Player playerInTurn) { }
        public void AfterPostSubTurn(Player playerInTurn) { }
    }
}
