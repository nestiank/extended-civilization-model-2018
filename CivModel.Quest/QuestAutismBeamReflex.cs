using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;
using static CivModel.Zap.FishPlayerNumber;

namespace CivModel.Quests
{
    public class QuestAutismBeamReflex : Quest
    {
        public override string Name => "불가사의 - 오티즘 빔 반사 어레이";

        public override int PostingTurn => 10;
        public override int LimitTurn => 10;

        public override string GoalNotice => "기술력을 제한 기간 안에 X 이상으로 올리세요";
        public override string RewardNotice => "[특수 자원 : 오티즘빔 반사 어레이] 1 획득";
        public override string CompleteNotice => @"핀란드의 무시무시한 병기 전략 오티즘 빔은 그들이 전장에서 사용하는 전술 무기 오티즘 드론의 주무기를 키운 버전이라 설명을 할 수 있습니다. 이 광선에 맞은 사람은 순식간에 말도 제대로 못하며 집중을 못하는 바보가 되어버립니다. 오늘날 이 현상은 자폐증이라 알려져 있지요. 다행이도 위대한 환국은 재빨리 외계 통신 피라미드들을 반사 용도로 바꾸어, 핀란드가 평양을 향해 쏜 오티즘 전략 빔을 반사하는데 성공합니다. 이를 통해 핀란드는 민족 전체가 역으로 오티즘 빔에 맞아, 전쟁의 판도가 크게 바뀌게 됩니다.";

        public QuestAutismBeamReflex(Game game)
            : base(game.GetPlayerFish(), game.GetPlayerHwan())
        {
        }

        public override void OnQuestDeployTime()
        {
            var finno = Game.GetPlayerFinno();
            if (finno.SpecialResource[AutismBeamAmplificationCrystal.Instance] > 0)
            {
                if (Game.Random.Next(10) < 7)
                    Deploy();
            }
        }

        protected override void OnAccept()
        {
        }

        private void Cleanup()
        {
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        protected override void OnComplete()
        {
            Requestee.SpecialResource[SpecialResourceAutismBeamReflex.Instance] = 1;

            Cleanup();
        }

        protected override void FixedPostTurn()
        {
            if (Status == QuestStatus.Accepted)
            {
                if (Requestee.Research >= 0)
                {
                    Status = QuestStatus.Completed;
                }
            }

            base.FixedPostTurn();
        }
    }
}
