using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class QuestSubMoaiForceField : Quest, ITileObjectObserver
    {
        public override string Name => "건물 기증 - 모아이 포스 필드";

        public override int PostingTurn => 5;
        public override int LimitTurn => 10;

        public override string GoalNotice => "제한 기간내 이스터 왕국의 영토내 [환 제국 5차산업혁명 요새]를 건설해주세요.";
        public override string RewardNotice => "[특수 자원 : 모아이 포스 필드] 획득";
        public override string CompleteNotice => @"이스터 섬의 오늘날에도 찾아볼수 있는 모아이 석상들. 이는 고대의 아직도 풀리지 않는 미스테리중 하나지요. 하지만 이에 대한 해답은 간단합니다. 모아이 석상들과 비슷한 돌하르방이 제주도에서 발견된다는것을 고려하면 이 또한 환 민족의 잔재란 것을 우리는 논리적으로 파악할 수가 있습니다. 모아이 석상들은 Hyperwar때 사용이 되었던 강력한 역장 force field (에너지 방어막 energy shield )을 생성하는 초고도로 발달된 로봇들을 본따 만들어진 것이죠. 이 로봇들은 Hyperwar동안 이스터 섬의 주민들을 시공간 왜곡으로 부터 보호해주었고, 결국 문명이 붕괴된 이후 살아남은 생존자들은 이 로봇들을 신으로 섬기게 되며 그들의 현상을 본따 석상을 만들게 된것입니다.";

        public QuestSubMoaiForceField(Player requestee) : base(null, requestee)
        {
            this.Status = QuestStatus.Deployed;
        }

        protected override void OnAccept()
        {
            Game.TurnObservable.AddObserver(this);
            Game.TileObjectObservable.AddObserver(this);
        }

        private void Cleanup()
        {
            Game.TurnObservable.RemoveObserver(this);
            Game.TileObjectObservable.RemoveObserver(this);
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        protected override void OnComplete()
        {
            Requestee.SpecialResource[SpecialResourceMoaiForceField.Instance] = 1;

            Cleanup();
        }

        public void TileObjectCreated(TileObject obj)
        {
            if (obj is CivModel.Hwan.HwanEmpireFIRFortress Moai && Moai.Owner == Requestee)
            {
                Status = QuestStatus.Completed;
            }
        }

        public void TileObjectPlaced(TileObject obj) { }
    }
}
