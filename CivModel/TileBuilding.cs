using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents a building which is an actor.
    /// </summary>
    /// <seealso cref="CivModel.Actor" />
    /// <seealso cref="InteriorBuilding"/>
    public abstract class TileBuilding : Actor
    {
        /// <summary>
        /// The action performing movement. <c>null</c> by default.
        /// </summary>
        public override IActorAction MoveAct => null;

        /// <summary>
        /// The amount of gold this building provides.
        /// </summary>
        /// <seealso cref="InteriorBuilding.ProvidedGold"/>
        /// <seealso cref="Player.GoldIncome"/>
        public virtual double ProvidedGold => 0;

        /// <summary>
        /// The amount of happiness this building provides.
        /// </summary>
        /// <seealso cref="InteriorBuilding.ProvidedHappiness"/>
        /// <seealso cref="Player.HappinessIncome"/>
        public virtual double ProvidedHappiness => 0;

        /// <summary>
        /// The labor which this tile building offers.
        /// </summary>
        /// <seealso cref="InteriorBuilding.ProvidedLabor"/>
        /// <seealso cref="Player.Labor"/>
        public virtual double ProvidedLabor => 0;

        /// <summary>
        /// Whether this TileBuilding is given to <see cref="Actor.Owner"/> by donation or not.
        /// </summary>
        /// <seealso cref="Donator"/>
        public bool IsDonated => Donator != null;

        /// <summary>
        /// The player donated this TileBuilding. If this TileBuilding is not donated, <c>null</c>.
        /// </summary>
        /// <seealso cref="IsDonated"/>
        public Player Donator { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TileBuilding"/> class.
        /// </summary>
        /// <param name="owner">The player who owns this TileBuilding.</param>
        /// <param name="constants">constants of this actor.</param>
        /// <param name="point">The tile where the object will be.</param>
        /// <param name="donator">The player donated this TileBuilding. If this TileBuilding is not donated, <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="owner"/> is <c>null</c>.
        /// or
        /// <paramref name="constants"/> is <c>null</c>.
        /// </exception>
        public TileBuilding(Player owner, ActorConstants constants, Terrain.Point point, Player donator)
            : base(owner, constants, point, TileTag.TileBuilding)
        {
            Donator = donator;

            owner.TryAddTerritory(point);
            Owner.AddTileBuildingToList(this);
        }

        /// <summary>
        /// Called after <see cref="TileObject.PlacedPoint" /> is changed.
        /// </summary>
        /// <param name="oldPoint">The old value of <see cref="TileObject.PlacedPoint" />.</param>
        protected override void OnChangePlacedPoint(Terrain.Point? oldPoint)
        {
            base.OnChangePlacedPoint(oldPoint);

            if (PlacedPoint is Terrain.Point pt)
                Owner.TryAddTerritory(pt);
        }

        /// <summary>
        /// Called before [change owner], by <see cref="Actor.ChangeOwner" />.
        /// </summary>
        /// <param name="newOwner">The new owner.</param>
        protected override void OnBeforeChangeOwner(Player newOwner)
        {
            base.OnBeforeChangeOwner(newOwner);
            Owner.RemoveTileBuildingFromList(this);
            newOwner.AddTileBuildingToList(this);
        }

        /// <summary>
        /// Called after [change owner], by <see cref="Actor.ChangeOwner" />.
        /// </summary>
        /// <param name="prevOwner">The previous owner.</param>
        protected override void OnAfterChangeOwner(Player prevOwner)
        {
            base.OnAfterChangeOwner(prevOwner);
            if (PlacedPoint is Terrain.Point pt)
            {
                // Player.RemoveTerritory 코드 내 주석 참조
                prevOwner.RemoveTerritory(pt);

                Owner.AddTerritory(pt);
            }
        }

        /// <summary>
        /// Called before [destroy], by <see cref="Actor.Destroy" />
        /// </summary>
        protected override void OnBeforeDestroy()
        {
            Owner.RemoveTileBuildingFromList(this);
            base.OnBeforeDestroy();
        }
    }
}
