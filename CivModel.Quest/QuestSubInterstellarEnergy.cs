using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class QuestSubInterstellarEnergy : Quest, ITileObjectObserver
    {
        public override string Name => "불가사의 - 성간 에너지";

        public override int PostingTurn => 5;
        public override int LimitTurn => 10;

        public override string GoalNotice => "제한 기간내 에뮤 연방의 영토내 불가사의 1개, 핀란드 제국의 영토내에 불가사의 1개를 배치하세요.";
        public override string RewardNotice => "[특수 자원 : 성간 에너지 추출기] 획득";
        public override string CompleteNotice => @"우주를 떠다니는, 행성의 기로 정화되지 않은 순수 마력을 추출하는 방법을 핀란드는 전쟁 말기에 터득하게 됩니다. 오늘도 이를 위해 사용되었던 거대 시설들의 잔해를 영국의 스톤헨지와 같은데서 찾아볼수가 있죠. 이러한 작업을 위해선 지구의 정확한 반대편에도 비슷한 시설을 건축할 필요가 있었는데, 안타깝게도 이 두번째 짝인 시설은 하이퍼워가 끝나며 바다 밑으로 가라않게 됩니다. 그럼에도 우리는 스톤헨지의 실제 기능애 대한 증거를 찾아볼수가 있습니다. 이러한 스톤헨지의 초자연적 기능 때문에 오늘날 오스트렐리아와 뉴질랜드로 영국인들이 자연스럽게 끌릴수 밖에 없었던 것이죠.";

        public QuestSubInterstellarEnergy(Player requestee) : base(null, requestee)
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
            Requestee.SpecialResource[InterstellarEnergyExtractor.Instance] = 1;

            Cleanup();
        }

        public void TileObjectCreated(TileObject obj)
        {
            if (obj is CivModel.Finno.Preternaturality Extractor && Extractor.Owner == Requestee && Extractor.PlacedPoint.Value.TileOwner == Extractor.Owner.Game.Players[5])
            {
                Status = QuestStatus.Completed;
            }
        }

        public void TileObjectPlaced(TileObject obj) { }
    }
}
