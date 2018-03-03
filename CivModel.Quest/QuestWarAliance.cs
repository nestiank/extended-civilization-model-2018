using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class QuestWarAliance : Quest, IBattleObserver
    {
        public override string Name => "[전쟁 동맹] - 에뮤 연방";

        public override int PostingTurn => 15;
        public override int LimitTurn => 10;

        public int flag = 0;

        public override string GoalNotice => "오티즘 빔 드론 3대를 생산하여 어류 공하국 병력을 공격하세요.";
        public override string RewardNotice => "[특수 자원: 오티즘 빔 증폭 크리스탈] 1 획득";
        public override string CompleteNotice => @"쵸슨 피플의 무리들이 전장에서 수오미 제국을 밀수 있던 큰 원인 중 하나는 그들의 뛰어난 장군들이었습니다. 이를 막기 위해 개발된 기술이 바로 오티즘 빔으로, 이 광선에 맞은 사람은 순식간에 말도 제대로 못하며 집중을 못하는 바보가 되어버립니다. 오늘날 이 현상은 자폐증이라 알려져 있지요. 헬신키 깊은 지하의 던전에서 개발된 이 병기를 증폭시켜 환국의 리더쉽을 통째로 날려버린다면, 분명 위대한 수오미 제국은 승리를 쟁취할 수 있을것이라 황제 스푸르도 스파르데 스푸르도 1세(Spurdo Spärde I Spurdo)는 생각했지요. 덕분에 위대하며 신성한 프로토-카가네이트는 일시적으로 전쟁에서 엄청난 이점을 쥐게 됩니다. 안타깝게도 환국에선 이미 대항책을 준비하고 있었습니다.....";

        public QuestWarAliance(Player requestee) : base(null, requestee)
        {
        }

        protected override void OnAccept()
        {
            Game.TurnObservable.AddObserver(this);
        }

        private void Cleanup()
        {
            Game.TurnObservable.RemoveObserver(this);
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        protected override void OnComplete()
        {
            Requestee.SpecialResource[AutismBeamAmplificationCrystal.Instance] = 1;
            foreach(var Player in Game.Players)
            {
                foreach(var TheQuest in Player.Quests)
                {
                    if (TheQuest is QuestAutismBeamReflex)
                    {
                        TheQuest.Status = QuestStatus.Deployed;
                    }
                }
            }


            Cleanup();
        }

        public void OnBattle(Actor attacker, Actor defender, BattleResult result)
        {
            if (attacker.Owner == Requestee && attacker is CivModel.Finno.AutismBeamDrone && defender.Owner == defender.Owner.Game.Players[4] && flag < 2)
            {
                flag += 1;               
            }
            else if(attacker.Owner == Requestee && attacker is CivModel.Finno.AutismBeamDrone && defender.Owner == defender.Owner.Game.Players[4] && flag >= 2)
            {
                flag = 0;
                Status = QuestStatus.Completed;
            }
        }
    }
}
