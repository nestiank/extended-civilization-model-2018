using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class GatesOfRlyeh : ISpecialResource
    {
        public static GatesOfRlyeh Instance => _instance.Value;
        private static Lazy<GatesOfRlyeh> _instance
            = new Lazy<GatesOfRlyeh>(() => new GatesOfRlyeh());

        private GatesOfRlyeh() { }

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
                if (_player.SpecialResource[GatesOfRlyeh.Instance] < 1)
                    return;

                int CountFinno = 0;
                int CountHwan = 0;
                int UnitCount = 0;

                while (CountFinno < 3)
                {
                    foreach (Player i in _player.Game.Players)
                    {
                        if (i.Team == _player.Team)
                        {
                            UnitCount = UnitCount + i.Units.Count();
                        }
                    }

                    if(UnitCount < 1)
                    {
                        break;
                    }

                    Unit UnitToDieF = null;
                    foreach (Player CalledByOrigin in _player.Game.Players)
                    {
                        if(CalledByOrigin.Team == _player.Team && UnitToDieF == null)
                        {
                            foreach (Unit called in CalledByOrigin.Units)
                            {
                                if (_player.Game.Random.Next(Math.Min(UnitCount * 10, 150)) < 5)
                                {
                                    UnitToDieF = called;
                                }
                            }
                        }
                    }

                    if(UnitToDieF != null)
                    {
                        UnitToDieF.Destroy();
                        CountFinno++;
                    }
                }

                while (CountHwan < 7)
                {
                    foreach (Player i in _player.Game.Players)
                    {
                        if (i.Team == _player.Team)
                        {
                            UnitCount = UnitCount + i.Units.Count();
                        }
                    }

                    if (UnitCount < 1)
                    {
                        break;
                    }

                    Unit UnitToDieH = null;
                    foreach (Player CalledByOrigin in _player.Game.Players)
                    {
                        if (CalledByOrigin.Team == _player.Team && UnitToDieH == null)
                        {
                            foreach (Unit called in CalledByOrigin.Units)
                            {
                                if (_player.Game.Random.Next(Math.Min(UnitCount * 10, 150)) < 4)
                                {
                                    UnitToDieH = called;
                                }
                            }
                        }
                    }

                    if (UnitToDieH != null)
                    {
                        UnitToDieH.Destroy();
                        CountHwan++;
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
