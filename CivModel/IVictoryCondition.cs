using System;
using System.Collections.Generic;
using System.Linq;

namespace CivModel
{
    /// <summary>
    /// The interface represents a victory condition that <see cref="Player"/> can achieve.
    /// </summary>
    public interface IVictoryCondition
    {
        /// <summary>
        /// Checks whether <see cref="Player"/> has achieved this victory condition.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>whether <see cref="Player"/> has achieved this victory condition</returns>
        bool CheckVictory(Player player);

        /// <summary>
        /// Called when <see cref="Player"/> achieves this victory.
        /// </summary>
        /// <param name="player">The player.</param>
        void DoVictory(Player player);
    }

    /// <summary>
    /// The interface represents a defeat condition that <see cref="Player"/> can achieve.
    /// </summary>
    public interface IDefeatCondition
    {
        /// <summary>
        /// Checks whether <see cref="Player"/> has achieved this defeat condition.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>whether <see cref="Player"/> has achieved this defeat condition</returns>
        bool CheckDefeat(Player player);

        /// <summary>
        /// Called when <see cref="Player"/> achieves this defeat.
        /// </summary>
        /// <param name="player">The player.</param>
        void DoDefeat(Player player);
    }
}
