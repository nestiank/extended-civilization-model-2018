using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents a building which is not an actor.
    /// </summary>
    /// <seealso cref="TileBuilding"/>
    public abstract class InteriorBuilding : ITurnObserver
    {
        /// <summary>
        /// Called after a sub turn.
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        public virtual void PostPlayerSubTurn(Player playerInTurn)
        {
        }

        /// <summary>
        /// Called after a turn.
        /// </summary>
        public virtual void PostTurn()
        {
        }

        /// <summary>
        /// Called before a sub turn.
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        public virtual void PrePlayerSubTurn(Player playerInTurn)
        {
        }

        /// <summary>
        /// Called before a turn.
        /// </summary>
        public virtual void PreTurn()
        {
        }
    }
}
