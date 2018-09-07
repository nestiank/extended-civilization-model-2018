using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Zap.AtlantisPlayerNumber;

namespace CivModel.Quests
{
    public class QuestSubGeneticEngineering : Quest, ITurnObserver
    {
        private const string TecCount = "TecCount";

        private double _startResearch;

        public QuestSubGeneticEngineering(Game game)
            : base(game.GetPlayerAtlantis(), game.GetPlayerFinno(), typeof(QuestSubGeneticEngineering))
        {
        }

        public override void OnQuestDeployTime()
        {
            if (Game.Random.Next(10) == 0)
                Deploy();
        }

        protected override void OnAccept()
        {
            Game.TurnObservable.AddObserver(this, ObserverPriority.Model);
            _startResearch = Requestee.Research;
        }

        private void Cleanup()
        {
            Game.TurnObservable.RemoveObserver(this);
            Progresses[TecCount].Value = 0;
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        protected override void OnComplete()
        {
            Requestee.SpecialResource[Ubermensch.Instance] = 1;

            foreach (Player player in Game.Players)
            {
                if (player.Team == Game.Players[1].Team)
                {
                    foreach (Unit unit in player.Units)
                    {
                        unit.MaxAP = 3;
                    }
                }
            }

            Cleanup();
        }

        public void PostTurn()
        {
            if (Status == QuestStatus.Accepted)
            {
                Progresses[TecCount].Value = (int)Math.Min(Requestee.Research - _startResearch, Progresses[TecCount].MaxValue);
                if (Progresses[TecCount].IsFull)
                {
                    Status = QuestStatus.Completed;
                }
            }
        }

        public void PreTurn() { }
        public void AfterPreTurn() { }
        public void AfterPostTurn() { }
        public void PreSubTurn(Player playerInTurn) { }
        public void AfterPreSubTurn(Player playerInTurn) { }
        public void PostSubTurn(Player playerInTurn) { }
        public void AfterPostSubTurn(Player playerInTurn) { }
    }
}
