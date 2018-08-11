using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The factory interface of <see cref="TileBuildingProduction"/>.
    /// This interface additionally provides <see cref="CreateDonation(Player, Terrain.Point, Player)"/> methods.
    /// If donation is not wanted, use <see cref="ITileObjectProductionFactory"/>
    /// </summary>
    /// <remarks>
    /// In normal case, <see cref="ITileObjectProductionFactory.CreateTileObject(Player, Terrain.Point)"/> is called to make production.
    /// <br/>
    /// In the case of donation, <see cref="CreateDonation(Player, Terrain.Point, Player)"/> is called instead.
    /// </remarks>
    /// <seealso cref="CivModel.ITileObjectProductionFactory" />
    public interface ITileBuildingProductionFactory : ITileObjectProductionFactory
    {
        /// <summary>
        /// Donates the <see cref="TileBuilding"/> which is the production result.
        /// </summary>
        /// <param name="owner">The <see cref="Player"/> who owns the donation result.</param>
        /// <param name="point">The tile where the object will be.</param>
        /// <param name="donator">The player donated this TileBuilding.</param>
        /// <returns>the created <see cref="TileBuilding"/> result.</returns>
        TileBuilding CreateDonation(Player owner, Terrain.Point point, Player donator);
    }

    /// <summary>
    /// The <see cref="Production"/> class for <see cref="TileBuilding"/>.
    /// This class enables donation of <see cref="TileBuilding"/>.
    /// If donation is not wanted, use <see cref="TileObjectProduction"/>
    /// </summary>
    /// <seealso cref="TileObjectProduction"/>
    /// <seealso cref="Production"/>
    public class TileBuildingProduction : TileObjectProduction
    {
        private readonly ITileBuildingProductionFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileBuildingProduction"/> class.
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
        public TileBuildingProduction(ITileBuildingProductionFactory factory, Player owner) : base(factory, owner)
        {
            _factory = factory;
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

            if (point.TileOwner == null || point.TileOwner == Owner)
            {
                return _factory.CreateTileObject(Owner, point);
            }
            else
            {
                return _factory.CreateDonation(point.TileOwner, point, Owner);
            }
        }
    }
}
