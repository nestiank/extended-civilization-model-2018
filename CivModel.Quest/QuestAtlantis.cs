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
        
        public QuestAtlantis(Game game)
            : base(game.GetPlayerAtlantis(), game.GetPlayerFinno(), typeof(QuestAtlantis))
        {
        }

        public override void OnQuestDeployTime()
        {
            var hwan = Game.GetPlayerHwan();
            if (hwan.SpecialResource[SpecialResourceAutismBeamReflex.Instance] > 0)
            {
                if (Game.Random.Next(10) < 7)
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
            if (obj is CivModel.Finno.Preternaturality preter && preter.Owner == Requester && preter.Donator == Requestee)
            {
                Status = QuestStatus.Completed;
            }
        }

        public void TileObjectPlaced(TileObject obj) { }
    }
}
