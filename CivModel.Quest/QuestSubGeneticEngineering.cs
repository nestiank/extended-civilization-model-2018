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
        public override string Name => "불가사의 - 유전 연구학";

        public override int PostingTurn => 5;
        public override int LimitTurn => 10;

        public override string GoalNotice => "제한 기간내 기술력을 (퀘스트 수락 때 기술력)+3000 만큼 올리세요.";
        public override string RewardNotice => "[특수 자원 : Ubermensch] 획득";
        public override string CompleteNotice => @"핀란드의 심화된 유전자 연구는 대다수 재야사학자들에 의해 많이 연구가 되었으며 가장 잘 알려져있는, 남아있는 Hyperwar의 유산이기도 합니다. 전쟁 중기 환국의 Girl's Generation과 같은 계속된 정신적 공격에 전장에서 큰 피해를 입었던 핀란드 카가네이트는 이를 견더내기 위해 더 강인한 정신과 육체를 가진, 신 인류를 개발할 필요성을 느끼게 되었고, 아틀란티스와 함께 진행되던 이 연구의 대표적 실패작으로는 오늘날의 게르만족이라 불리는 자들이 있죠.";

        private const double _requiredResearch = 3000;
        private double _targetResearch;

        public QuestSubGeneticEngineering(Game game)
            : base(game.GetPlayerAtlantis(), game.GetPlayerFinno())
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
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        protected override void OnComplete()
        {
            Requestee.SpecialResource[Ubermensch.Instance] = 1;
            Cleanup();
        }

        protected override void FixedPostTurn()
        {
            if (Status == QuestStatus.Accepted)
            {
                if (Requestee.Research >= _targetResearch)
                {
                    Status = QuestStatus.Completed;
                }
            }

            base.FixedPostTurn();
        }
    }
}
