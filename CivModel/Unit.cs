using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public abstract class Unit : Actor
    {
        private readonly Player _owner;
        public override Player Owner => _owner;

        private readonly IActorAction _moveAct;
        public override IActorAction MoveAct => _moveAct;

        public override IActorAction AttackAct => null;

        public override IReadOnlyList<IActorAction> SpecialActs => null;

        public Unit(Player owner) : base(TileTag.Unit)
        {
            _owner = owner;
            _moveAct = new MoveActorAction(this);

            _owner.Units.Add(this);
        }
    }
}
