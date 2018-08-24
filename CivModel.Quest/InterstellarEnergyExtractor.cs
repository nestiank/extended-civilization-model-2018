using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class InterstellarEnergyExtractor : ISpecialResource, Finno.IInterstellarE
    {
        public static InterstellarEnergyExtractor Instance => _instance.Value;
        private static Lazy<InterstellarEnergyExtractor> _instance
            = new Lazy<InterstellarEnergyExtractor>(() => new InterstellarEnergyExtractor());
        private InterstellarEnergyExtractor()
        {
        }

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
