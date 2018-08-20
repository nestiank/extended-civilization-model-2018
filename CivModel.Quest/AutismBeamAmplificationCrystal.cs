using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class AutismBeamAmplificationCrystal : ISpecialResource
    {
        public static AutismBeamAmplificationCrystal Instance => _instance.Value;
        private static Lazy<AutismBeamAmplificationCrystal> _instance
            = new Lazy<AutismBeamAmplificationCrystal>(() => new AutismBeamAmplificationCrystal());

        private AutismBeamAmplificationCrystal() { }

        public int MaxCount => 1;

        public object EnablePlayer(Player player)
        {
            return new DataObject(player);
        }

        private class DataObject : ITurnObserver
        {
            private Player _player;

            public DataObject(Player player)
            {
                _player = player;

                player.Game.TurnObservable.AddObserver(this, ObserverPriority.Model);
            }

            public void PostTurn()
            {
                if (_player.SpecialResource[AutismBeamAmplificationCrystal.Instance] < 1)
                    return;

                if (_player.Game.Players[1].SpecialResource[AutismBeamAmplificationCrystal.Instance] > 0)
                {
                    if (_player.Game.Players[0].SpecialResource[SpecialResourceAutismBeamReflex.Instance] > 0)
                    {
                        return;
                    }

                    if(_player.Game.Random.Next(10) > 1)
                    {
                        Player Selected = null;
                        while (Selected == null)
                        {
                            foreach (Player i in _player.Game.Players)
                            {
                                if(i.Team != _player.Team && Selected == null)
                                {
                                    if (_player.Game.Random.Next(100) < 4)
                                    {
                                        Selected = i;
                                    }
                                }
                            }
                        }

                        CityBase city = null;
                        while (city == null)
                        {
                            if (Selected.Cities.Count() <= 1)
                            {
                                break;
                            }
                            foreach (CityBase c in Selected.Cities)
                            {
                                if (c != Selected.Cities.First() && city == null)
                                {
                                    if (_player.Game.Random.Next(100) == 4)
                                    {
                                        city = c;
                                    }
                                }
                            }
                        }

                        if (city != null)
                        {
                            foreach (InteriorBuilding Interior in city.InteriorBuildings)
                            {
                                Interior.Destroy();
                            }

                            Terrain.Point point = city.PlacedPoint.Value;
                            city.Destroy();

                            point.TileOwner = _player;
                        }
                    }
                }
                // TODO
            }

            public void PreTurn() { }
            public void AfterPreTurn() { }
            public void AfterPostTurn() { }
            public void PreSubTurn(Player playerInTurn) { }
            public void AfterPreSubTurn(Player playerInTurn) { }
            public void PostSubTurn(Player playerInTurn) { }
            public void AfterPostSubTurn(Player playerInTurn) { }
        }
    }
}
