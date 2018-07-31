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
        /// <remarks>
        /// Victory check is done after draw check, before defeat check.
        /// </remarks>
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
        /// <remarks>
        /// Defeat check is done after draw check and victory check.
        /// </remarks>
        bool CheckDefeat(Player player);

        /// <summary>
        /// Called when <see cref="Player"/> achieves this defeat.
        /// </summary>
        /// <param name="player">The player.</param>
        void DoDefeat(Player player);
    }

    /// <summary>
    /// The interface represents a draw condition that <see cref="Player"/> can achieve.
    /// </summary>
    public interface IDrawCondition
    {
        /// <summary>
        /// Checks whether <see cref="Player"/> has achieved this draw condition.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>whether <see cref="Player"/> has achieved this draw condition</returns>
        /// <remarks>
        /// Draw check is done before victory check or defeat check.
        /// If you want to check draw condition when player achieves both victory and defeat simultaneously,
        ///  implements <see cref="OnBothVictoriedAndDefeated(Player, IVictoryCondition, IDefeatCondition)"/>.
        /// </remarks>
        bool CheckDraw(Player player);

        /// <summary>
        /// Checks whether <see cref="Player"/> has achieved this draw condition
        ///  when <paramref name="player"/> achieves both victory and defeat.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="victory">The victory that player achieves.</param>
        /// <param name="defeat">The defeat that player achieves.</param>
        /// <returns>whether <see cref="Player"/> has achieved this draw condition</returns>
        /// <remarks>
        /// This method is called when player achieves both victory and defeat simultaneously.
        /// If you want to check draw condition manually, implements <see cref="CheckDraw(Player)"/>.
        /// </remarks>
        bool OnBothVictoriedAndDefeated(Player player, IVictoryCondition victory, IDefeatCondition defeat);

        /// <summary>
        /// Called when <see cref="Player"/> achieves this draw.
        /// </summary>
        /// <param name="player">The player.</param>
        void DoDraw(Player player);
    }
}
