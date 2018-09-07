using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public abstract class QuestUltimateBase : Quest, ITurnObserver, IQuestObserver
    {
        protected abstract string DelayProgress { get; }

        protected abstract List<KeyValuePair<string, ISpecialResource>> RequiredResources { get; }

        public QuestUltimateBase(Player requester, Player requestee, Type type)
            : base(requester, requestee, type)
        {
        }

        public override void OnQuestDeployTime()
        {
            if (!Requestee.HasEnding)
            {
                Deploy();
                Accept();
            }
        }

        protected override void OnAccept()
        {
            Game.TurnObservable.AddObserver(this, ObserverPriority.Model);
            Game.QuestObservable.AddObserver(this, ObserverPriority.Model);
        }

        private void Cleanup()
        {
            Game.TurnObservable.RemoveObserver(this);
            Game.QuestObservable.RemoveObserver(this);
        }

        protected override void OnComplete()
        {
            Cleanup();
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        public void QuestCompleted(Quest quest)
        {
            foreach (var pr in RequiredResources)
            {
                Progresses[pr.Key].Value = Math.Min(1, Requestee.SpecialResource[pr.Value]);
            }
        }
        public void QuestAccepted(Quest quest) { }
        public void QuestGivenup(Quest quest) { }

        public void AfterPostTurn()
        {
            if (Requestee.HasEnding)
            {
                Disable();
                return;
            }

            if (GetCondition())
            {
                if (!Progresses[DelayProgress].IsFull)
                    Progresses[DelayProgress].Value += 1;
            }
            else
            {
                Progresses[DelayProgress].Value = 0;
            }
        }

        public void AfterPreSubTurn(Player playerInTurn)
        {
            if (playerInTurn != Requestee)
                return;

            if (Requestee.HasEnding)
            {
                Disable();
                return;
            }

            if (Progresses[DelayProgress].IsFull)
            {
                if (!GetCondition())
                {
                    Progresses[DelayProgress].Value = 0;
                }
                else
                {
                    var hwanVictory = GetEnemyUltimateQuest();
                    if (!hwanVictory.GetCondition())
                    {
                        Requestee.AchieveEnding(new FinnoUltimateVictory(Game));
                        foreach (var player in Game.Players)
                        {
                            if (player != Requestee && !player.HasEnding)
                            {
                                player.AchieveEnding(new UltimateDefeat(Game));
                            }
                        }
                    }
                    else
                    {
                        foreach (var player in Game.Players)
                        {
                            if (!player.HasEnding)
                                player.AchieveEnding(new UltimateDraw(Game));
                        }
                    }
                    Complete();
                    hwanVictory.Complete();
                }
            }
        }

        public void PreTurn() { }
        public void AfterPreTurn() { }
        public void PostTurn() { }
        public void PreSubTurn(Player playerInTurn) { }
        public void PostSubTurn(Player playerInTurn) { }
        public void AfterPostSubTurn(Player playerInTurn) { }

        protected abstract QuestUltimateBase GetEnemyUltimateQuest();

        public virtual bool GetCondition()
        {
            foreach (var progress in Progresses)
            {
                if (progress.Enabled && progress.Value == 0)
                    return false;
            }
            return true;
        }
    }
}
