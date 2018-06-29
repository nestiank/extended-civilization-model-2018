using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The interface to observe battle of <see cref="Actor"/>.
    /// </summary>
    /// <seealso cref="Game"/>
    /// <seealso cref="Actor"/>
    public interface IBattleObserver
    {
        /// <summary>
        /// Called before battle.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="defender">The defender.</param>
        void OnBeforeBattle(Actor attacker, Actor defender);

        /// <summary>
        /// Called after battle.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="defender">The defender.</param>
        /// <param name="atkOwner">The owner of <paramref name="attacker"/>.</param>
        /// <param name="defOwner">The owner of <paramref name="defender"/>.</param>
        /// <param name="result">The result of battle.</param>
        /// <remarks>
        /// If an <see cref="Actor"/> engaged in battle die, its <see cref="Actor.Owner"/> becomes <c>null</c>.
        /// In this case, <paramref name="atkOwner"/> and <paramref name="defOwner"/> parameters can be used to get their previous value.
        /// </remarks>
        void OnAfterBattle(Actor attacker, Actor defender, Player atkOwner, Player defOwner, BattleResult result);
    }
}
