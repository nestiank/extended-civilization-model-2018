using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Zap.AtlantisPlayerNumber;

namespace CivModel.Quests
{
    public class QuestSubGeneticEngineering : Quest
    {
        private const string TecCount = "TecCount";

        private const double _requiredResearch = 7000;
        private double _targetResearch;

        public QuestSubGeneticEngineering(Game game)
            : base(game.GetPlayerAtlantis(), game.GetPlayerFinno(), typeof(QuestSubGeneticEngineering))
        {
        }

        public override void OnQuestDeployTime()
        {
            if (Game.Random.Next(2) == 0)
                Deploy();
        }

        protected override void OnAccept()
        {
            _targetResearch = Requestee.Research + _requiredResearch;
        }

        private void Cleanup()
        {
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

        protected override void FixedPostTurn()
        {
            if (Status == QuestStatus.Accepted)
            {
                Progresses[TecCount].Value = (int)Math.Min(_targetResearch - Requestee.Research, 7000);
                if (Requestee.Research >= _targetResearch)
                {
                    Status = QuestStatus.Completed;
                }
            }

            base.FixedPostTurn();
        }
    }
}
