using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Hwan.HwanPlayerNumber;
using static CivModel.Zap.EgyptPlayerNumber;

namespace CivModel.Quests
{
    public class QuestEgyptKingdom : Quest, ITileObjectObserver
    {
        public override string Name => "불가사의 - 오티즘 빔 반사 어레이";

        public override int PostingTurn => 5;
        public override int LimitTurn => 10;

        public override string GoalNotice => "[이집트 캉덤]의 영토 내에 기한 내 [불가사의]를 건축해 주세요";
        public override string RewardNotice => "[특수 자원 : 가자 피라미드 외계 통신 기구] 1 획득";
        public override string CompleteNotice => @"뾰족한 삼각기둥의 피라미드는 전세계 어디에서나 찾아볼 수가 있지요. 고대 이집트의 피라미드들은 물론, 피라미드 형태의 구조물들은 황허문명, 인더스 문명, 아메리카 대륙의 잉카 문명 등, 언뜻 보기에는 완전히 서로 관계가 없는 여러 위치와 문화들에서 발견이 됩니다. 당시 고대의 기술로는 건축을 하는게 불가능한 구조물이죠. 왜 그럴까요? 그 이유는 당연히 이 모두가 고대 한국, 즉 환국의 유산이기 때문이랍니다. 세계 최초의 피라미드는 장안 지역에 환국이 5만BC에 건축이 되었으며, 이들은 고대 환국이 궤도 밖 우주 식민지들과 통신하기 위한 용도로 사용이 되었답니다. 안타깝게도 핀란드와의 전쟁을 통해 대다수의 통신 장비가 전쟁 초기에 파괴되어 우주 식민 함대를 부르지 못했지만, 전쟁 말기 이집트 가자 지역에 피라미드를 또 건축해, 결국 우주 함대를 부르는데 성공했지요.";

        public QuestEgyptKingdom(Game game)
            : base(game.GetPlayerEgypt(), game.GetPlayerHwan())
        {
        }

        public override void OnQuestDeployTime()
        {
            if (Requestee.SpecialResource[SpecialResourceCthulhuProjectInfo.Instance] > 0)
            {
                if (Game.Random.Next(10) < 7)
                    Deploy();
            }
        }

        protected override void OnAccept()
        {
            Game.TileObjectObservable.AddObserver(this);
        }

        private void Cleanup()
        {
            Game.TileObjectObservable.RemoveObserver(this);
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

        public void TileObjectProduced(TileObject obj)
        {
            if(obj is CivModel.Hwan.Preternaturality pyramid && pyramid.Owner == Requester && pyramid.Donator == Requestee)
            {
                Status = QuestStatus.Completed;
            }
        }

        public void TileObjectPlaced(TileObject obj) {}
    }
}
