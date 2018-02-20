using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The factory interface of <see cref="TileObjectProduction"/>.
    /// This interface additionally provides <see cref="IsPlacable(TileObjectProduction, Terrain.Point)"/>
    ///  and <see cref="CreateTileObject(Player, Terrain.Point)"/> methods.
    /// </summary>
    /// <seealso cref="CivModel.IProductionFactory" />
    public interface ITileObjectProductionFactory : IProductionFactory
    {
        /// <summary>
        /// Determines whether the production result is placable at the specified point.
        /// </summary>
        /// <param name="production">The production.</param>
        /// <param name="point">The point to test to place the production result.</param>
        /// <returns>
        ///   <c>true</c> if the production is placable; otherwise, <c>false</c>.
        /// </returns>
        bool IsPlacable(TileObjectProduction production, Terrain.Point point);

        /// <summary>
        /// Creates the <see cref="TileObject"/> which is the production result.
        /// </summary>
        /// <param name="owner">The <see cref="Player"/> who owns the result.</param>
        /// <param name="point">The tile where the object will be.</param>
        /// <returns>the created <see cref="TileObject"/> result.</returns>
        TileObject CreateTileObject(Player owner, Terrain.Point point);
    }

    /// <summary>
    /// The factory interface of <see cref="Production"/> which products <see cref="Actor"/> objects.
    /// This interface also provides <see cref="IActorConstants"/> of production results.
    /// </summary>
    /// <seealso cref="IActorConstants"/>
    /// <seealso cref="ITileObjectProductionFactory"/>
    public interface IActorProductionFactory : ITileObjectProductionFactory
    {
        /// <summary>
        /// The constants of production result <see cref="Actor"/>.
        /// </summary>
        /// <seealso cref="IActorConstants"/>
        IActorConstants Constants { get; }
    }

    /// <summary>
    /// The <see cref="Production"/> class for <see cref="TileObject"/>
    /// </summary>
    /// <seealso cref="CivModel.Production" />
    public class TileObjectProduction : Production
    {
        private readonly ITileObjectProductionFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileObjectProduction"/> class.
        /// </summary>
        /// <param name="factory">The factory object of this production kind.</param>
        /// <param name="owner">The <see cref="Player"/> who will own the production.</param>
        /// <exception cref="ArgumentException">
        /// <see cref="IProductionFactory.TotalLaborCost"/> is negative
        /// or
        /// <see cref="IProductionFactory.TotalGoldCost"/> is negative
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="IProductionFactory.LaborCapacityPerTurn"/> is not in [0, <see cref="Production.TotalLaborCost"/>]
        /// or
        /// <see cref="IProductionFactory.GoldCapacityPerTurn"/> is not in [0, <see cref="Production.TotalGoldCost"/>]
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory"/> is <c>null</c>
        /// or
        /// <paramref name="owner"/> is <c>null</c>
        /// </exception>
        public TileObjectProduction(ITileObjectProductionFactory factory, Player owner) : base(factory, owner)
        {
            _factory = factory;
        }

        /// <summary>
        /// Determines whether the production result is placable at the specified point.
        /// </summary>
        /// <param name="point">The point to test to place the production result.</param>
        /// <returns>
        ///   <c>true</c> if the production is placable; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsPlacable(Terrain.Point point)
        {
            return _factory.IsPlacable(this, point);
        }

        /// <summary>
        /// Places the production result at the specified point.
        /// </summary>
        /// <param name="point">The point to place the production result.</param>
        /// <exception cref="InvalidOperationException">production is not completed yet</exception>
        /// <exception cref="ArgumentException">point is invalid</exception>
        public override void Place(Terrain.Point point)
        {
            if (!IsCompleted)
                throw new InvalidOperationException("production is not completed yet");
            if (!IsPlacable(point))
                throw new ArgumentException("point is invalid");

            var obj = _factory.CreateTileObject(Owner, point);
            obj.ProcessCreation();
        }
    }
}
