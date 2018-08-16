using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class SpecialResourceCthulhuProjectInfo : ISpecialResource
    {
        public static SpecialResourceCthulhuProjectInfo Instance => _instance.Value;
        private static Lazy<SpecialResourceCthulhuProjectInfo> _instance
            = new Lazy<SpecialResourceCthulhuProjectInfo>(() => new SpecialResourceCthulhuProjectInfo());

        private SpecialResourceCthulhuProjectInfo() { }

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
                if(obj is Unit)
                {
                    if(((Unit)obj).Owner.Team == _player.Team)
                    {
                        ((Unit)obj).DefencePower = ((Unit)obj).DefencePower * 2;
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
