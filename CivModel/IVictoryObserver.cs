using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The interface to observe victories and defeats of <see cref="Player"/>.
    /// </summary>
    public interface IVictoryObserver
    {
        /// <summary>
        /// Called when a player achieves victory.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="victory">The victory that player achieves.</param>
        void OnVictory(Player player, IVictoryCondition victory);

        /// <summary>
        /// Called when a player achieves defeat.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="defeat">The defeat that player achieves.</param>
        void OnDefeat(Player player, IDefeatCondition defeat);

        /// <summary>
        /// Called when a player achieves draw.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="draw">The draw that player achieves.</param>
        void OnDraw(Player player, IDrawCondition draw);
    }
}
