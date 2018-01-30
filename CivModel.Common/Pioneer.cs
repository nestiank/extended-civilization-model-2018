using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public sealed class Pioneer : Unit
    {
        public static Guid ClassGuid { get; } = new Guid("487BBF97-538A-45CB-A62D-B33E173F8E6F");
        public override Guid Guid => ClassGuid;

        public override int MaxAP => 2;

        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        public Pioneer(Player owner) : base(owner)
        {
            _specialActs[0] = new PioneerAction(this);
        }

        private class PioneerAction : IActorAction
        {
            private readonly Pioneer _owner;
            public Actor Owner => _owner;

            public bool IsParametered => false;

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

                return 1;
            }

            public void Act(Terrain.Point? pt)
            {
                if (pt != null)
                    throw new ArgumentException("pt is invalid");
                if (!_owner.PlacedPoint.HasValue)
                    throw new InvalidOperationException("Actor is not placed yet");

                var ownerpt = _owner.PlacedPoint.Value;
                var player = Owner.Owner;
                Owner.Destroy();

                var city = new CityCenter(player);
                city.PlacedPoint = ownerpt;
                city.Owner.Game.Scheme.InitializeCity(city, true);
            }
        }
    }

    public class PioneerProductionFactory : ITileObjectProductionFactory
    {
        public static PioneerProductionFactory Instance => _instance.Value;
        private static Lazy<PioneerProductionFactory> _instance
            = new Lazy<PioneerProductionFactory>(() => new PioneerProductionFactory());
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
