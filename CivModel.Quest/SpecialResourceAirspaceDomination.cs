using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class SpecialResourceAirspaceDomination : ISpecialResource
    {
        public static SpecialResourceAirspaceDomination Instance => _instance.Value;
        private static Lazy<SpecialResourceAirspaceDomination> _instance
            = new Lazy<SpecialResourceAirspaceDomination>(() => new SpecialResourceAirspaceDomination());
        private SpecialResourceAirspaceDomination()
        {
        }

        public int MaxCount => 1;

        public object EnablePlayer(Player player)
        {
            return new DataObject(player);
        }

        private class DataObject : ITurnObserver, ITileObjectObserver
        {
            private Player _player;

            public DataObject(Player player)
            {
                _player = player;

                player.Game.TurnObservable.AddObserver(this, ObserverPriority.Model);
            }

            public void PostTurn()
            {
                // TODO
            }

            public void TileObjectProduced(TileObject obj)
            {
                if (obj is Unit)
                {
                    if (((Unit)obj).Owner.Team == _player.Team && (((Unit)obj) is Hwan.LEOSpaceArmada || ((Unit)obj) is Zap.LEOSpaceArmada))
                    {
                        ((Unit)obj).AttackPower = ((Unit)obj).AttackPower * 3;
                        ((Unit)obj).MaxAP = 4;
                    }
                }
            }
            public void TileObjectPlaced(TileObject obj) { }

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
