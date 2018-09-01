using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The interface to observe game ending of <see cref="Player"/>.
    /// </summary>
    public interface IEndingObserver
    {
        /// <summary>
        /// Called when a player achieves ending.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="victory">The ending that player achieves.</param>
        void OnEnding(Player player, Ending victory);
    }
}
