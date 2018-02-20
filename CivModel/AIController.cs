using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Fuzzy;

namespace CivModel
{
    sealed class AIController
    {
        private readonly Player _player;

        public AIController(Player player)
        {
            _player = player;

            // fuzzy set init
        }

        public void Destroy()
        {
        }

        public async Task DoAction()
        {
            // do CivModel read

            await Task.Run(() => {
                // do AI thinking
                System.Threading.Thread.Sleep(3000);
            });

            // do CivModel access
        }
    }
}
