using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents an unit.
    /// </summary>
    /// <seealso cref="CivModel.Actor" />
    public abstract class Unit : Actor
    {
        /// <summary>
        /// The action performing movement. A <see cref="MoveActorAction"/> object by default.
        /// </summary>
        public override IActorAction MoveAct => _moveAct;
        private readonly IActorAction _moveAct;

        /// <summary>
        /// Initializes a new instance of the <see cref="Unit"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="Player"/> who owns this unit.</param>
        /// <param name="constants">constants of this actor.</param>
        /// <param name="point">The tile where the object will be.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="owner"/> is <c>null</c>.
        /// or
        /// <paramref name="constants"/> is <c>null</c>.
        /// </exception>
        public Unit(Player owner, ActorConstants constants, Terrain.Point point)
            : base(owner, constants, point, TileTag.Unit)
        {
            Owner.AddUnitToList(this);
            _moveAct = new MoveActorAction(this);
        }

        /// <summary>
        /// Called before [change owner], by <see cref="Actor.ChangeOwner" />.
        /// </summary>
        /// <param name="newOwner">The new owner.</param>
        protected override void OnBeforeChangeOwner(Player newOwner)
        {
            base.OnBeforeChangeOwner(newOwner);
            Owner.RemoveUnitFromList(this);
            newOwner.AddUnitToList(this);
        }

        /// <summary>
        /// Called before [destroy], by <see cref="Actor.Destroy" />
        /// </summary>
        protected override void OnBeforeDestroy()
        {
            Owner.RemoveUnitFromList(this);
            base.OnBeforeDestroy();
        }

        /// <summary>
        /// Called after <see cref="TileObject.PlacedPoint"/> is changed.
        /// </summary>
        /// <param name="oldPoint">The old value of <see cref="TileObject.PlacedPoint"/>.</param>
        /// <exception cref="InvalidOperationException"></exception>
        protected override void OnChangePlacedPoint(Terrain.Point? oldPoint)
        {
            base.OnChangePlacedPoint(oldPoint);
            if (PlacedPoint is Terrain.Point pt)
            {
                if (pt.TileBuilding is TileBuilding building && building.Owner != Owner)
                {
                    throw new InvalidOperationException("unit cannot be placed on a TileBuilding of other players");
                }
            }
        }
    }
}
