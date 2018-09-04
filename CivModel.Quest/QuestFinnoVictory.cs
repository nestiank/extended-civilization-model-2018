using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;

namespace CivModel.Quests
{
    public class QuestFinnoVictory : Quest, ITurnObserver, IQuestObserver
    {
        private const string Autism = "autism";
        private const string Necro = "necro";
        private const string Rlyeh = "rlyeh";
        private const string Delay = "delay";

        public QuestFinnoVictory(Game game)
            : base(null, game.GetPlayerFinno(), typeof(QuestFinnoVictory))
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
            Progresses[Delay].Value = 0;
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
            Progresses[Autism].Value = Math.Min(1, Requestee.SpecialResource[AutismBeamAmplificationCrystal.Instance]);
            Progresses[Necro].Value = Math.Min(1, Requestee.SpecialResource[Necronomicon.Instance]);
            Progresses[Rlyeh].Value = Math.Min(1, Requestee.SpecialResource[GatesOfRlyeh.Instance]);
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
                if (!Progresses[Delay].IsFull)
                    Progresses[Delay].Value += 1;
            }
            else
            {
                Progresses[Delay].Value = 0;
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

            if (Progresses[Delay].IsFull)
            {
                if (!GetCondition())
                {
                    Progresses[Delay].Value = 0;
                }
                else
                {
                    var hwanVictory = Game.GetPlayerHwan().Quests.OfType<QuestHwanVictory>().FirstOrDefault();
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

        internal bool GetCondition()
        {
            var c1 = Progresses[Autism].Value > 0;
            var c2 = Progresses[Necro].Value > 0;
            var c3 = Progresses[Rlyeh].Value > 0;
            return c1 && c2 && c3;
        }
    }
}
