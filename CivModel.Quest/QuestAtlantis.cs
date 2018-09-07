using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;
using static CivModel.Zap.AtlantisPlayerNumber;

namespace CivModel.Quests
{
    public class QuestAtlantis : Quest, ITileObjectObserver
    {
        private const string ToDoCount = "ToDoCount";
        private int SorcererCount = 0;
        private int DustCount = 0;

        public QuestAtlantis(Game game)
            : base(game.GetPlayerAtlantis(), game.GetPlayerFinno(), typeof(QuestAtlantis))
        {
        }

        public override void OnQuestDeployTime()
        {
            var hwan = Game.GetPlayerHwan();
            if (hwan.SpecialResource[SpecialResourceAutismBeamReflex.Instance] > 0)
            {
                if (Game.Random.Next(10) < 5)
                    Deploy();
            }
        }

        protected override void OnAccept()
        {
            Game.TileObjectObservable.AddObserver(this, ObserverPriority.Model);
        }

        private void Cleanup()
        {
            Game.TileObjectObservable.RemoveObserver(this);

            Progresses[ToDoCount].Value = 0;
            SorcererCount = 0;
        }

        protected override void OnGiveup()
        {
            Cleanup();
        }

        protected override void OnComplete()
        {
            Requestee.SpecialResource[Necronomicon.Instance] = 1;

            foreach (Player player in Game.Players)
            {
                if (player.Team == Game.Players[1].Team)
                {
                    foreach (Unit unit in player.Units)
                    {
                        if (unit is Finno.AncientSorcerer)
                        {
                            unit.AttackPower = unit.AttackPower * 3;
                        }
                    }
                }
            }

            foreach (CityBase city in (Game.GetPlayerAtlantis()).Cities)
            {
                if(city != (Game.GetPlayerAtlantis()).Cities.First())
                {
                    city.Destroy();
                }
            }

            foreach (Terrain.Point DrownPoint in (Game.GetPlayerAtlantis()).Territory)
            {
                if(DrownPoint.Unit != null)
                    DrownPoint.Unit.Destroy();
            }

            Cleanup();
        }

        public void TileObjectProduced(TileObject obj)
        {
            if (obj is CivModel.Finno.AncientFinnoFineDustFactory Dust && Dust.Owner == Requester && Dust.Donator == Requestee)
            {
                if(DustCount < 1)
                {
                    DustCount += 1;
                    Progresses[ToDoCount].Value += 1;
                    if(Progresses[ToDoCount].IsFull)
                    {
                        Status = QuestStatus.Completed;
                    }
                }
            }

            if(obj is CivModel.Finno.AncientSorcerer Sorcerer && Sorcerer.Owner == Requestee)
            {
                if(SorcererCount < 3)
                {
                    SorcererCount += 1;
                    Progresses[ToDoCount].Value += 1;
                    if (Progresses[ToDoCount].IsFull)
                    {
                        Status = QuestStatus.Completed;
                    }
                }
            }
        }

        public void TileObjectPlaced(TileObject obj) { }
    }
}
