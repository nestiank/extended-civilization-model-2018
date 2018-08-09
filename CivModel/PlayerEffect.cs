using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivObservable;

namespace CivModel
{
    /// <summary>
    /// Represents an effect to <see cref="Player"/>.
    /// </summary>
    public abstract class PlayerEffect : Effect
    {
        /// <summary>
        /// The target <see cref="Player"/> of this effect.
        /// </summary>
        public new Player Target => (Player)base.Target;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorEffect"/> class.
        /// </summary>
        /// <param name="target">The target of the effect.</param>
        /// <param name="duration">The duration of the effect. <c>-1</c> if forever.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is negative and not -1</exception>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <c>null</c>.</exception>
        public PlayerEffect(Player target, int duration)
            : base(target, duration)
        {
        }
    }
}
