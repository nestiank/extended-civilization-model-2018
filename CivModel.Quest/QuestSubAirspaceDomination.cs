using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Hwan.HwanPlayerNumber;
using static CivModel.Zap.SwedePlayerNumber;

namespace CivModel.Quests
{
    public class QuestSubAirspaceDomination : Quest, IBattleObserver
    {
        private const string KillCount = "KillCount";


        public QuestSubAirspaceDomination(Game game)
            : base(game.GetPlayerSwede(), game.GetPlayerHwan(), typeof(QuestSubAirspaceDomination))
        {
        }

        public override void OnQuestDeployTime()
        {
            if (Requestee.Research >= 0)
            {
                if (Game.Random.Next(2) == 0)
                    Deploy();
            }
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
            Requestee.SpecialResource[SpecialResourceAirspaceDomination.Instance] = 1;

            foreach (Player player in Game.Players)
            {
                if (player.Team == Game.Players[0].Team)
                {
                    foreach (Unit unit in player.Units)
                    {
                        if (unit is Hwan.LEOSpaceArmada || unit is Zap.LEOSpaceArmada)
                        {
                            unit.AttackPower = unit.AttackPower * 3;
                            unit.MaxAP = 4;
                        }
                    }
                }
            }

            Cleanup();
        }

        public void OnBeforeBattle(Actor attacker, Actor defender)
        {
        }

        public void OnAfterBattle(Actor attacker, Actor defender, Player atkOwner, Player defOwner, BattleResult result)
        {
            if (atkOwner == Requestee && defOwner == Game.Players[7] && defender is CivModel.Zap.LEOSpaceArmada && defender.Owner == null)
            {
                if (Progresses[KillCount].Value < 3)
                    Progresses[KillCount].Value += 1;
                else if(Progresses[KillCount].IsFull)
                {
                    Status = QuestStatus.Completed;
                }
            }

            else if (atkOwner == Game.Players[7] && defOwner == Requestee && attacker is CivModel.Zap.LEOSpaceArmada && attacker.Owner == null)
            {
                if (Progresses[KillCount].Value < 3)
                    Progresses[KillCount].Value += 1;
                else if (Progresses[KillCount].IsFull)
                {
                    Status = QuestStatus.Completed;
                }
            }
        }
    }
}
