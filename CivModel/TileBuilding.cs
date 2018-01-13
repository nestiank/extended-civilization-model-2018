using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public abstract class TileBuilding : Actor
    {
        public override int MaxAP => 0;

        public override IActorAction MoveAct => null;
        public override IActorAction AttackAct => null;

        public override IReadOnlyList<IActorAction> SpecialActs => null;

        public TileBuilding() : base(TileTag.TileBuilding)
        {
        }
    }
}
