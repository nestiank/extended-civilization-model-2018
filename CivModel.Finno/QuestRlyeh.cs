using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public class QuestRlyeh :Quest, ITileObjectObserver
    {
        public override string Name => "[불가사의] - R'̧l̨̜y͎͎̜̺̬e͕͇͇͚͓̹h̢̳͎̗͇͇̙";

        public override int PostingTurn => 5;
        public override int LimitTurn => 5;

        public override string GoalNotice => "R'̧l̨̜y͎͎̜̺̬e͕͇͇͚͓̹h̢̳͎̗͇͇̙의 영토내에 [불가사의]를 건설해 주세요.";
        public override string RewardNotice => "[특수 자원: G̼͈͉̖a̙͉͔͍̙t͍̞͕e̺̹̼̬s̷̘̯ ͉̪͙̯ͅo̮̝͔̩̖f͚͚͖̳̻͇ ̮̻̮͎͇͇R̝'̥̬͝l͚̼y҉̫e͏̜̲͔͈̲͖ͅh́] 1 획득";
        public override string CompleteNotice => @"환국의 계속된 공세에 밀리던 핀란드 제국은 결국 사람이기를 포기합니다. 그들은 진짜로 사악하며 악한 무리들과 손을 잡고, 그 분 을 소환할 수 있게 R̝'̥̬͝l͚̼y҉̫e͏̜̲͔͈̲͖ͅh́로 가는 포탈을 생성하게 되죠. 환국이 오티즘 빔을 반사하여 뇌에 문제가 생겨서 이런 생각을 했는지, 아니면 그저 궁지에 몰려서 이런 선택을 하게 되었는지는 후대 (재야) 역사학자들이 아직도 해결하지 못한 미스테리로 남아있습니다. 하지만 그 결과는 참담했습니다. 아틀란티스의 존재에 대한 모든 증거가 소실되었고, 레무리아는 가라앉았지요. 그리고 비록 서로를 극도로 혐오하였지만, 그때까지 존재했고 앞으로 존재할 모든 인류 문명보다 위대했던 환국과 고대 핀란드 제국은 동시에 멸망하게 됩니다. 이들의 존재에 대한 대다수의 증거도 이 사건으로 인해 소실되게 되죠. 지구가 그대로 우주에서 지워지지 않았던 유일한 이유는 아마 환국이 동시에 불렀던 외계 식민 함대들 덕분일 겁니다.";

        public QuestRlyeh(Player requestee) : base(null, requestee)
        {
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
            Requestee.SpecialResource[GatesOfRlyeh.Instance] = 1;

            Cleanup();
        }

        public void TileObjectCreated(TileObject obj)
        {
            if (obj is Preternaturality Rlyeh && Rlyeh.Owner == Requestee)
            {
                Status = QuestStatus.Completed;
            }
        }

        public void TileObjectPlaced(TileObject obj) { }
    }
}
