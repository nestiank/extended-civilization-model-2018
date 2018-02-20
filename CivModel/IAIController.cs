using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Fuzzy;

namespace CivModel
{
    /// <summary>
    /// The interface represents AI controlling a player.
    /// </summary>
    public interface IAIController
    {
        /// <summary>
        /// Do jobs of AI. This method can be asynchronous.
        /// </summary>
        /// <remarks>
        /// Since this method can be asynchronous, the model <strong>must not</strong> changed until the task is completed.
        /// </remarks>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DoAction();

        /// <summary>
        /// Destroys this instance.
        /// </summary>
        void Destroy();
    }
}
