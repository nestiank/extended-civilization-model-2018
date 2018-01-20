using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public abstract class Unit : Actor
    {
        private readonly IActorAction _moveAct;
        public override IActorAction MoveAct => _moveAct;

        public Unit(Player owner) : base(owner, TileTag.Unit)
        {
            Owner.AddUnitToList(this);
            _moveAct = new MoveActorAction(this);
        }

        protected override void OnChangeOwner(Player newOwner)
        {
            base.OnChangeOwner(newOwner);
            Owner.RemoveUnitFromList(this);
            newOwner.AddUnitToList(this);
        }

        protected override void OnDestroy()
        {
            Owner.RemoveUnitFromList(this);
            base.OnDestroy();
        }
    }
}
