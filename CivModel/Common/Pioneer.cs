using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    /// <summary>
    /// A pionner unit who can make city.
    /// </summary>
    /// <seealso cref="CivModel.Unit" />
    public class Pioneer : Unit
    {
        /// <summary>
        /// The maximum AP.
        /// </summary>
        public override int MaxAP => 2;

        /// <summary>
        /// The list of special actions.
        /// <see cref="Pioneer"/> have one special action "pionnering".
        /// </summary>
        public override IReadOnlyList<IActorAction> SpecialActs => _specialActs;
        private readonly IActorAction[] _specialActs = new IActorAction[1];

        /// <summary>
        /// Initializes a new instance of the <see cref="Pioneer"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="Player"/> who owns this unit.</param>
        /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <c>null</c>.</exception>
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
            }
        }
    }

    /// <summary>
    /// The factory interface for <see cref="Pioneer"/>.
    /// </summary>
    /// <seealso cref="CivModel.ITileObjectProductionFactory" />
    public class PioneerProductionFactory : ITileObjectProductionFactory
    {
        /// <summary>
        /// The singleton instance.of <see cref="PioneerProductionFactory"/>
        /// </summary>
        public static PioneerProductionFactory Instance => _instance.Value;
        private static Lazy<PioneerProductionFactory> _instance
            = new Lazy<PioneerProductionFactory>(() => new PioneerProductionFactory());

        private PioneerProductionFactory()
        {
        }

        /// <summary>
        /// Creates the <see cref="Production" /> object
        /// </summary>
        /// <param name="owner">The player who owns the <see cref="Production" /> object.</param>
        /// <returns>
        /// the created <see cref="Production" /> object
        /// </returns>
        public Production Create(Player owner)
        {
            return new TileObjectProduction(this, owner, 5, 2);
        }

        /// <summary>
        /// Determines whether the production result is placable at the specified point.
        /// </summary>
        /// <param name="production">The production.</param>
        /// <param name="point">The point to test to place the production result.</param>
        /// <returns>
        ///   <c>true</c> if the production is placable; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPlacable(TileObjectProduction production, Terrain.Point point)
        {
            return point.Unit == null
                && point.TileBuilding is CityCenter
                && point.TileBuilding.Owner == production.Owner;
        }

        /// <summary>
        /// Creates the <see cref="TileObject" /> which is the production result.
        /// </summary>
        /// <param name="owner">The <see cref="Player" /> who owns the result.</param>
        /// <returns>
        /// the created <see cref="TileObject" /> result.
        /// </returns>
        public TileObject CreateTileObject(Player owner)
        {
            return new Pioneer(owner);
        }
    }
}
