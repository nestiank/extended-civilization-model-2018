using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents a default constants starage of <see cref="TileBuilding"/>.
    /// </summary>
    /// <seealso cref="ActorConstants"/>
    /// <seealso cref="TileBuilding"/>
    /// <seealso cref="TileBuilding(Player, IActorConstants, Terrain.Point)"/>
    public class TileBuildingConstants : ActorConstants
    {
        /// <summary>
        /// The maximum AP.
        /// </summary>
        public override double MaxAP => 0;
        /// <summary>
        /// The amount of gold logistics of this actor.
        /// </summary>
        public override double GoldLogistics => 0;
        /// <summary>
        /// The amount of labor logistics of this actor to get the full heal amount of <see cref="Actor.MaxHealPerTurn"/>.
        /// </summary>
        public override double FullLaborLogicstics => 0;
    }

    /// <summary>
    /// Represents a building which is an actor.
    /// </summary>
    /// <seealso cref="CivModel.Actor" />
    /// <seealso cref="InteriorBuilding"/>
    public abstract class TileBuilding : Actor
    {
        /// <summary>
        /// The action performing movement. <c>null</c> by default.
        /// </summary>
        public override IActorAction MoveAct => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileBuilding"/> class.
        /// </summary>
        /// <param name="owner">The player who owns this TileBuilding.</param>
        /// <param name="constants">constants of this actor. if <c>null</c>, use a default <see cref="TileBuildingConstants"/> object.</param>
        /// <param name="point">The tile where the object will be.</param>
        /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <c>null</c>.</exception>
        public TileBuilding(Player owner, IActorConstants constants, Terrain.Point point)
            : base(owner, constants ?? new TileBuildingConstants(), point, TileTag.TileBuilding)
        {
            owner.TryAddTerritory(point);
        }

        /// <summary>
        /// Called after <see cref="TileObject.PlacedPoint" /> is changed.
        /// </summary>
        /// <param name="oldPoint">The old value of <see cref="TileObject.PlacedPoint" />.</param>
        protected override void OnChangePlacedPoint(Terrain.Point? oldPoint)
        {
            base.OnChangePlacedPoint(oldPoint);

            if (PlacedPoint is Terrain.Point pt)
                Owner.TryAddTerritory(pt);
        }
    }
}
