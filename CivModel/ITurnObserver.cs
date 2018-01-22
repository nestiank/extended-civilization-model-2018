using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The interface to observe pre/post turn event.
    /// </summary>
    /// <seealso cref="Game"/>
    public interface ITurnObserver
    {
        /// <summary>
        /// Called before a turn.
        /// </summary>
        void PreTurn();

        /// <summary>
        /// Called after a turn.
        /// </summary>
        void PostTurn();

        /// <summary>
        /// Called before a sub turn.
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        void PrePlayerSubTurn(Player playerInTurn);

        /// <summary>
        /// Called after a sub turn.
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        void PostPlayerSubTurn(Player playerInTurn);
    }
}
