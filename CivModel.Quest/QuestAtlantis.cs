using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class QuestAtlantis : Quest, ITileObjectObserver
    {
        public override string Name => "[불가사의] - 아틀란티스";

        public override int PostingTurn => 10;
        public override int LimitTurn => 5;

        public override string GoalNotice => "아틀란티스의 영토내에 [불가사의]를 건설해 주세요.";
        public override string RewardNotice => "[특수 자원: 네크로노미콘] 1 획득";
        public override string CompleteNotice => @"환국의 오티즘 빔 반사 어레이는 위대하며 신성한 프로토-카가네이트에게 치명적이었습니다. 스푸르도 스파르데 스푸르도 1세는 순식간에 저능아 수준으로 머리가 퇴보해 버렸고 그의 측근들 대다수 또한 마찬가지가 되버렸죠. 제정신을 유지하던 스푸뤼도 슈푀르데 스푸르도 2세(Spürdo Špärde II Spurdo)는 스푸르도 스파르데 스푸르도 1세를 폐위 시키고 새로 황제로 즉위를 한뒤, 오티즘을 극복하고 쳐들오던 환국의 무리들을 저지할 마지막 수단을 생각해내게 됩니다. 바로 그들의 주술의 원천이자 르'뤼에의 지배자, 그레이트 올드 원 크툴루(̴̺͔̭̩̭͕̰C͈̫̺͠t̡͕͖̺͓̘͖͎h̤̩̫̱̼̀u̱̱̤̮̝͚͞ḷ̹̭̞̭h̢͖͚u, , ̴̮̬̰̰ͅ크̤̦̦͚툴̢̜̞̯̭̟̱루҉̹̰͉̙͓̘)) 를 소환해 환국의 무리들을 죄다 없애버리는 것이였지요. 하지만 그 분 을 소환하기 위해선 핀란드 주술의 고대서 N̩͔̼̫̳̗e͉̠̥̲cr̙̱̠̤͓̬ͅo͔̥͇̲͇̳n̸̳̙̩͎̹̞o̰͕͙͈͡mi̠ͅc̦͖̲̮̜͡o͔ṋ͚̺͈͇͎̗ 이 필요했습니다. 이를 찾으려 노력하다 좌절해버렸던 순간, 위대하며 신성한 프로토-카가네이트의 아우국, 아틀란티스에서 연락이 왔죠. 바로 그들의 섬의 중앙, 바다 속 깊은 곳에 어마어마하며 어두운 마력의 원천이 느껴진다는 것이었습니다. 이 사실에 마지막 희망을 걸고, 스푸뤼도 슈푀르데 스푸르도 2세는 아틀란티스에 대규모 고고학적 프로젝트를 벌이게 됩니다. 비록 오티즘 빔의 영향을 받아 섬의 지반이 불안정해지긴 했지만, 결국 핀란드 제국은 N̩͔̼̫̳̗e͉̠̥̲cr̙̱̠̤͓̬ͅo͔̥͇̲͇̳n̸̳̙̩͎̹̞o̰͕͙͈͡mi̠ͅc̦͖̲̮̜͡o͔ṋ͚̺͈͇͎̗ 을 얻는데 성공합니다. 그리고 그 분 을 소환할 준비에 들어가지요.";

        public QuestAtlantis(Player requestee) : base(null, requestee)
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
            Requestee.SpecialResource[Necronomicon.Instance] = 1;
            foreach (var TheQuest in Requestee.Quests)
            {
                if (TheQuest is QuestRlyeh)
                {
                    TheQuest.Status = QuestStatus.Deployed;
                }
            }

            Cleanup();
        }

        public void TileObjectCreated(TileObject obj)
        {
            if (obj is CivModel.Finno.Preternaturality Atlantis && Atlantis.Owner == Requestee)
            {
                Status = QuestStatus.Completed;
            }
        }

        public void TileObjectPlaced(TileObject obj) { }
    }
}
