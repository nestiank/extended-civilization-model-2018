using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class Pioneer : Unit
    {
        public override int MaxAP => 2;

        private readonly IActorAction[] _specialActs = new IActorAction[1];
        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;

        public Pioneer(Player owner) : base(owner)
        {
            _specialActs[0] = new PioneerAction(this);
        }

        private class PioneerAction : IActorAction
        {
            public bool IsParametered => false;
            private readonly Pioneer _owner;

            public PioneerAction(Pioneer owner)
            {
                _owner = owner;
            }

            public int GetRequiredAP(Terrain.Point? pt)
            {
                if (pt != null)
                    return -1;
                if (!_owner.PlacedPoint.HasValue)
                    return -1;

                if (_owner.PlacedPoint.Value.TileBuilding != null)
                    return -1;

                return 0;
            }

            public void Act(Terrain.Point? pt)
            {
                if (pt != null)
                    throw new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");

                var ownerpt = _owner.PlacedPoint.Value;
                _owner.PlacedPoint = null;

                var city = new CityCenter(_owner.Owner);
                city.PlacedPoint = ownerpt;
            }
        }
    }

    public class PioneerProductionFactory : ITileObjectProductionFactory
    {
        private static Lazy<PioneerProductionFactory> _instance
            = new Lazy<PioneerProductionFactory>(() => new PioneerProductionFactory());
        public static PioneerProductionFactory Instance => _instance.Value;
        private PioneerProductionFactory()
        {
        }
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 5, 2);
        }
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }
        public TileObject CreateTileObject(Player owner)
        {
            return new Pioneer(owner);
        }
    }
}
