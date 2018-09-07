using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The interface to observe <see cref="Actor"/> related events.
    /// </summary>
    /// <seealso cref="Actor"/>
    public interface IActorObserver
    {
        /// <summary>
        /// Called when either <see cref="Actor.SkipFlag"/> or <see cref="Actor.SleepFlag"/> is changed.
        /// </summary>
        /// <param name="actor">the <see cref="Actor"/> object that raised event.</param>
        /// <param name="prevSkipFlag">the previous <see cref="Actor.SkipFlag"/>.</param>
        /// <param name="prevSleepFlag">the previous <see cref="Actor.SleepFlag"/>.</param>
        void OnSkipFlagChanged(Actor actor, bool prevSkipFlag, bool prevSleepFlag);
    }
}
