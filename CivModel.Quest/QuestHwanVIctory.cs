using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;

namespace CivModel.Quests
{
    public class QuestHwanVictory : Quest, ITurnObserver, IQuestObserver
    {
        private const string Autism = "autism";
        private const string Cthulhu = "cthulhu";
        private const string Alien = "alien";

        private int _markedTurn;

        public QuestHwanVictory(Game game)
            : base(null, game.GetPlayerHwan(), typeof(QuestHwanVictory))
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
            _markedTurn = int.MinValue;
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
            Progresses[Autism].Value = Math.Min(1, Requestee.SpecialResource[SpecialResourceAutismBeamReflex.Instance]);
            Progresses[Cthulhu].Value = Math.Min(1, Requestee.SpecialResource[SpecialResourceCthulhuProjectInfo.Instance]);
            Progresses[Alien].Value = Math.Min(1, Requestee.SpecialResource[SpecialResourceAlienCommunication.Instance]);
        }
        public void QuestAccepted(Quest quest) { }
        public void QuestGivenup(Quest quest) { }

        public void AfterPreSubTurn(Player playerInTurn)
        {
            if (playerInTurn != Requestee)
                return;

            _markedTurn = GetCondition() ? Game.TurnNumber : int.MinValue;
        }

        public void AfterPostSubTurn(Player playerInTurn)
        {
            if (playerInTurn != Requestee)
                return;

            if (Requestee.HasEnding)
            {
                Disable();
                return;
            }

            if (_markedTurn + 1 == Game.TurnNumber && GetCondition())
            {
                var finnoVictory = Game.GetPlayerFinno().Quests.OfType<QuestFinnoVictory>().FirstOrDefault();
                if (!finnoVictory.GetCondition())
                {
                    Requestee.AchieveEnding(new HwanUltimateVictory(Game));
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
                finnoVictory.Complete();
            }
        }

        public void PreTurn() { }
        public void AfterPreTurn() { }
        public void PostTurn() { }
        public void AfterPostTurn() { }
        public void PreSubTurn(Player playerInTurn) { }
        public void PostSubTurn(Player playerInTurn) { }

        internal bool GetCondition()
        {
            var c1 = Progresses[Autism].Value > 0;
            var c2 = Progresses[Cthulhu].Value > 0;
            var c3 = Progresses[Alien].Value > 0;
            return c1 && c2 && c3;
        }
    }
}
