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
        /// <param name="point">The tile where the object will be.</param>
        /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <c>null</c>.</exception>
        public Unit(Player owner, Terrain.Point point) : base(owner, point, TileTag.Unit)
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
    }
}
