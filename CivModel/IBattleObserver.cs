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
        /// Called after battle.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="defender">The defender.</param>
        /// <param name="result">The result of battle.</param>
        void OnBattle(Actor attacker, Actor defender, BattleResult result);
    }
}
