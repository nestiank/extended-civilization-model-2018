using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class QuestSubAirspaceDomination : Quest, IBattleObserver
    {
        public override string Name => "군사 동맹 - 궤도 장악권";

        public override int PostingTurn => 10;
        public override int LimitTurn => 15;

        public override string GoalNotice => "제한 기간 내 레무리아 유닛 [저궤도 우주 함선]를 4대 이상 파괴하세요.";
        public override string RewardNotice => "[특수 자원 : 궤도 장악권] 1 획득";
        public override string CompleteNotice => @"레무리아인들은 옛부터 신성한 핀란드-카가네이트와 조공 관계를 맺은 깊은 동맹 관계를 가지고 있었습니다. 하지만 그들은 또한 전쟁과 폭력을 극도로 혐오했지요. 유라시아 대륙의 두 패권국, 환과 수오미 사이의 관계가 레무리아는 하는 수 없이 전쟁에 참여하게 됩니다. 레무리아의 마법은 너무나 발달하여 고도의 기술력과 분간이 불가능한 수준이었습니다. 환국의 우주 기술과 맞먹을 수준이었던 레무리아는 이를 사용해 우주 함대를 만들어, 환국의 궤도 장악을 견제합니다. 안타깝게도 환국의 우주함대는 이미 경험으로 전투에 단련되있었고, ᛐᚱᚮᛚᚮᛚᚮᛚ 전투와 ᚹᛐᚠᚱᚮᚠᛚᛒᛒᛩ전투에서 결정적으로 패한 레무리아 대 공중 함대는 이후 전쟁에 큰 영향을 미치지 못하게 됩니다.";

        public int flag = 0;

        public QuestSubAirspaceDomination(Player requestee) : base(null, requestee)
        {
        }

        protected override void OnAccept()
        {
            Game.TurnObservable.AddObserver(this);
            Game.BattleObservable.AddObserver(this);
        }

        private void Cleanup()
        {
            Game.TurnObservable.RemoveObserver(this);
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

        public void OnBattle(Actor attacker, Actor defender, BattleResult result)
        {
            if (attacker.Owner == Requestee && defender is CivModel.Zap.LEOSpaceArmada && result == BattleResult.Victory && defender.Owner == defender.Owner.Game.Players[7])
            {
                if (flag < 3)
                    flag += 1;
                else if(flag >= 3)
                {
                    Status = QuestStatus.Completed;
                    flag = 0;
                }
            }
        }
    }
}
