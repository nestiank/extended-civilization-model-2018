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
    /// The <see cref="Production"/> class for <see cref="TileObject"/>
    /// </summary>
    /// <seealso cref="Production" />
    public class TileObjectProduction : Production
    {
        private readonly ITileObjectProductionFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileObjectProduction"/> class.
        /// </summary>
        /// <param name="factory">The factory object of this production kind.</param>
        /// <param name="owner">The <see cref="Player"/> who will own the production.</param>
        /// <exception cref="ArgumentException">
        /// TotalLaborCost is negative
        /// or
        /// TotalGoldCost is negative
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// LaborCapacityPerTurn is not in [0, TotalLaborCost]
        /// or
        /// GoldCapacityPerTurn is not in [0, TotalGoldCost]
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
        protected override bool CoreIsPlacable(Terrain.Point point)
        {
            return _factory.IsPlacable(this, point);
        }

        /// <summary>
        /// Places the production result at the specified point.
        /// </summary>
        /// <param name="point">The point to place the production result.</param>
        /// <exception cref="InvalidOperationException">production is not completed yet</exception>
        /// <exception cref="ArgumentException">point is invalid</exception>
        /// <returns>The production result.</returns>
        protected override IProductionResult CorePlace(Terrain.Point point)
        {
            if (!IsCompleted)
                throw new InvalidOperationException("production is not completed yet");
            if (!IsPlacable(point))
                throw new ArgumentException("point is invalid");

            return _factory.CreateTileObject(Owner, point);
        }
    }
}
