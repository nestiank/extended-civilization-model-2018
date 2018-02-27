using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class TestResource : ISpecialResource
    {
        public static TestResource Instance => _instance.Value;
        private static Lazy<TestResource> _instance
            = new Lazy<TestResource>(() => new TestResource());
        private TestResource()
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

                player.Game.TurnObservable.AddObserver(this);
            }

            public void PostTurn()
            {
                System.Diagnostics.Debug.WriteLine("TestResource: " + _player.SpecialResource[Instance]);
            }

            public void PostPlayerSubTurn(Player playerInTurn) { }
            public void PrePlayerSubTurn(Player playerInTurn) { }
            public void PreTurn() { }
        }
    }
}
