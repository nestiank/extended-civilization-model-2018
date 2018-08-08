using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The interface to observe <see cref="TileObject"/> related events.
    /// </summary>
    /// <seealso cref="Game"/>
    /// <seealso cref="TileObject"/>
    public interface ITileObjectObserver
    {
        /// <summary>
        /// Called when <see cref="TileObject"/> is produced.
        /// </summary>
        /// <param name="obj">The <see cref="TileObject"/>.</param>
        void TileObjectProduced(TileObject obj);

        /// <summary>
        /// Called when <see cref="TileObject.PlacedPoint"/> is changed or initially set.
        /// </summary>
        /// <param name="obj">The <see cref="TileObject"/>.</param>
        void TileObjectPlaced(TileObject obj);
    }
}
