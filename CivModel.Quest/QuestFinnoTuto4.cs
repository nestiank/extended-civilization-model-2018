using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;

namespace CivModel.Quests
{
    public class QuestFinnoTuto4 : Quest, ITurnObserver
    {
        private const string Happy = "happy";
        private const string Gold = "gold";
        private const string Research = "research";

        public QuestFinnoTuto4(Game game)
            : base(game.GetPlayerFinno(), game.GetPlayerFinno(), typeof(QuestFinnoTuto4))
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
            var quest = Requestee.Quests.OfType<QuestFinnoTuto5>().FirstOrDefault();
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

            Progresses[Happy].SafeSetValue((int)Requestee.Happiness);
            Progresses[Gold].SafeSetValue((int)Requestee.GoldNetIncome);
            Progresses[Research].SafeSetValue((int)Requestee.Research);

            if (IsTotalProgressFull)
                Complete();
        }

        public void PostSubTurn(Player playerInTurn)
        {
            if (playerInTurn == Requestee)
                RetrieveProgress();
        }

        public void PreTurn() { }
        public void AfterPreTurn() { }
        public void PostTurn() { }
        public void AfterPostTurn() { }
        public void PreSubTurn(Player playerInTurn) { }
        public void AfterPreSubTurn(Player playerInTurn) { }
        public void AfterPostSubTurn(Player playerInTurn) { }
    }
}
