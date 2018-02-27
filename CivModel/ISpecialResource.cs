using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The interface represents a special resource
    /// </summary>
    public interface ISpecialResource
    {
        /// <summary>
        /// The maximum amount of this resource.
        /// <c>-1</c> if there is no maximum amount.
        /// </summary>
        int MaxCount { get; }

        /// <summary>
        /// Enables this resource for the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>
        /// User-defined per-player storage object. <paramref name="player"/> object stores this value.
        /// This value can be <c>null</c>.
        /// </returns>
        object EnablePlayer(Player player);
    }
}
