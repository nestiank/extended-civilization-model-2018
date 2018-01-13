using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public interface ITurnObserver
    {
        void PreTurn();
        void PostTurn();

        void PrePlayerSubTurn(Player playerInTurn);
        void PostPlayerSubTurn(Player playerInTurn);
    }
}
