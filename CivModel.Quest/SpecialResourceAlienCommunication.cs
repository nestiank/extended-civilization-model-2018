using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class SpecialResourceAlienCommunication : ISpecialResource
    {
        public static SpecialResourceAlienCommunication Instance => _instance.Value;
        private static Lazy<SpecialResourceAlienCommunication> _instance
            = new Lazy<SpecialResourceAlienCommunication>(() => new SpecialResourceAlienCommunication());

        private SpecialResourceAlienCommunication() { }

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
                if (_player.SpecialResource[SpecialResourceAlienCommunication.Instance] < 1)
                    return;

                for (int i = 0; i < 2; i++)
                {
                    var LEO = Hwan.LEOSpaceArmadaProductionFactory.Instance.Create(_player);
                    LEO.IsCompleted = true;
                    _player.Deployment.AddLast(LEO);
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
