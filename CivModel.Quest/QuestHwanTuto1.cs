using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Hwan.HwanPlayerNumber;

namespace CivModel.Quests
{
    public class QuestHwanTuto1 : Quest, ITileObjectObserver, IBattleObserver, IActorObserver
    {
        private const string Move = "move";
        private const string Attack = "attack";
        private const string Hold = "hold";

        public QuestHwanTuto1(Game game)
            : base(game.GetPlayerHwan(), game.GetPlayerHwan(), typeof(QuestHwanTuto1))
        {
        }

        public override void OnQuestDeployTime()
        {
            Deploy();
            Accept();
        }

        protected override void OnAccept()
        {
            Game.TileObjectObservable.AddObserver(this, ObserverPriority.Model);
            Game.BattleObservable.AddObserver(this, ObserverPriority.Model);
            Game.ActorObservable.AddObserver(this, ObserverPriority.Model);
        }

        private void Cleanup()
        {
            Game.TileObjectObservable.RemoveObserver(this);
            Game.BattleObservable.RemoveObserver(this);
            Game.ActorObservable.RemoveObserver(this);
        }

        protected override void OnComplete()
        {
            if (Requestee.Cities.FirstOrDefault() is CityBase city)
            {
                var adj = city.PlacedPoint.Value.Adjacents().First(
                    pt => pt.HasValue && pt.Value.TileOwner == Requestee && pt.Value.Unit == null).Value;
                new Hwan.JediKnight(Requestee, adj).OnAfterProduce(null);
            }

            var quest = Requestee.Quests.OfType<QuestHwanTuto2>().FirstOrDefault();
            if (quest != null)
            {
                quest.Deploy();
                quest.Accept();
            }
            Cleanup();
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        private List<TileObject> _producedList = new List<TileObject>();
        public void TileObjectProduced(TileObject obj)
        {
            _producedList.Add(obj);
        }
        public void TileObjectPlaced(TileObject obj)
        {
            if (_producedList.Remove(obj))
                return;

            if (obj is Unit unit && unit.Owner == Requestee)
            {
                Progresses[Move].SafeIncrement();
                if (IsTotalProgressFull)
                    Complete();
            }
        }

        public void OnBeforeBattle(Actor attacker, Actor defender)
        {
            if (attacker is Unit unit && unit.Owner == Requestee)
            {
                Progresses[Attack].SafeIncrement();
                if (IsTotalProgressFull)
                    Complete();
            }
        }
        public void OnAfterBattle(Actor attacker, Actor defender, Player atkOwner, Player defOwner, BattleResult result) { }

        public void OnSkipFlagChanged(Actor actor, bool prevSkipFlag, bool prevSleepFlag)
        {
            if (actor is Unit unit && unit.Owner == Requestee)
            {
                Progresses[Hold].SafeIncrement();
                if (IsTotalProgressFull)
                    Complete();
            }
        }
    }
}
