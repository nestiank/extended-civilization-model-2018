using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents a building which is an actor.
    /// </summary>
    /// <seealso cref="CivModel.Actor" />
    /// <seealso cref="InteriorBuilding"/>
    public abstract class TileBuilding : Actor
    {
        /// <summary>
        /// The maximum AP. <c>0</c> by default.
        /// </summary>
        public override int MaxAP => 0;

        /// <summary>
        /// The action performing movement. <c>null</c> by default.
        /// </summary>
        public override IActorAction MoveAct => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileBuilding"/> class.
        /// </summary>
        /// <param name="owner">The player who owns this TileBuilding.</param>
        /// <param name="point">The tile where the object will be.</param>
        /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <c>null</c>.</exception>
        public TileBuilding(Player owner, Terrain.Point point) : base(owner, point, TileTag.TileBuilding)
        {
        }
    }
}
