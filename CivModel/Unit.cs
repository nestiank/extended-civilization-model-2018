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
        public override IActorAction MoveAct { get; }

        /// <summary>
        /// The action performing pillage. A <see cref="MoveActorAction"/> object by default.
        /// </summary>
        public override IActorAction PillageAct { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Unit"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="Player"/> who owns this unit.</param>
        /// <param name="type">The concrete type of this object.</param>
        /// <param name="point">The tile where the object will be.</param>
        /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <c>null</c>.</exception>
        public Unit(Player owner, Type type, Terrain.Point point)
            : base(owner, type, point, TileTag.Unit)
        {
            Owner.AddUnitToList(this);
            MoveAct = new MoveActorAction(this);
            PillageAct = new PillageActorAction(this);

            ApplyPrototype(Game.GetPrototype<UnitPrototype>(type));
        }

        private void ApplyPrototype(UnitPrototype proto)
        {
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
                if (pt.TileBuilding is CityBase city && city.Owner != Owner)
                {
                    throw new InvalidOperationException("unit cannot be placed on a city of other players");
                }
            }
        }
    }
}
