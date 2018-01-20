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
        /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <c>null</c>.</exception>
        public Unit(Player owner) : base(owner, TileTag.Unit)
        {
            Owner.AddUnitToList(this);
            _moveAct = new MoveActorAction(this);
        }

        protected override void OnBeforeChangeOwner(Player newOwner)
        {
            base.OnBeforeChangeOwner(newOwner);
            Owner.RemoveUnitFromList(this);
            newOwner.AddUnitToList(this);
        }

        protected override void OnBeforeDestroy()
        {
            Owner.RemoveUnitFromList(this);
            base.OnBeforeDestroy();
        }
    }
}
