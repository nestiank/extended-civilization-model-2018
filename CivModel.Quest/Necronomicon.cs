using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class Necronomicon : ISpecialResource, Finno.INecro
    {
        public static Necronomicon Instance => _instance.Value;
        private static Lazy<Necronomicon> _instance
            = new Lazy<Necronomicon>(() => new Necronomicon());

        private Necronomicon() { }

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
                player.Game.TileObjectObservable.AddObserver(this, ObserverPriority.Model);
            }

            public void PostTurn()
            {
                // TODO
            }

            public void TileObjectProduced(TileObject obj)
            {
                if (_player.SpecialResource[Necronomicon.Instance] < 1)
                    return;

                if (obj is Unit)
                {
                    if (((Unit)obj).Owner.Team == _player.Team && obj is Finno.AncientSorcerer)
                    {
                        ((Unit)obj).AttackPower = ((Unit)obj).AttackPower * 3;
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
