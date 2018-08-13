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
        public int flag = 0;

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
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        protected override void OnComplete()
        {
            Requestee.SpecialResource[SpecialResourceAirspaceDomination.Instance] = 1;

            Cleanup();
        }

        public void OnBeforeBattle(Actor attacker, Actor defender)
        {
        }

        public void OnAfterBattle(Actor attacker, Actor defender, Player atkOwner, Player defOwner, BattleResult result)
        {
            if (atkOwner == Requestee && defOwner == Game.Players[7] && defender is CivModel.Zap.LEOSpaceArmada && defender.Owner == null)
            {
                if (flag < 3)
                    flag += 1;
                else if(flag >= 3)
                {
                    Status = QuestStatus.Completed;
                    flag = 0;
                }
            }

            else if (atkOwner == Game.Players[7] && defOwner == Requestee && attacker is CivModel.Zap.LEOSpaceArmada && attacker.Owner == null)
            {
                if (flag < 3)
                    flag += 1;
                else if (flag >= 3)
                {
                    Status = QuestStatus.Completed;
                    flag = 0;
                }
            }
        }
    }
}
