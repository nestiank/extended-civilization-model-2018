using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivModel.Common;

namespace CivModel
{
    /// <summary>
    /// The factory interface of <see cref="InteriorBuildingProduction"/>.
    /// This interface additionally provides <see cref="IsPlacable(InteriorBuildingProduction, CityCenter)"/>
    ///  and <see cref="CreateInteriorBuilding(Player)"/> methods.
    /// </summary>
    /// <seealso cref="CivModel.IProductionFactory" />
    public interface IInteriorBuildingProductionFactory : IProductionFactory
    {
        /// <summary>
        /// Determines whether the production result is placable in the specified city.
        /// </summary>
        /// <param name="production">The production.</param>
        /// <param name="city">The city to test to place the production result.</param>
        /// <returns>
        ///   <c>true</c> if the production is placable; otherwise, <c>false</c>.
        /// </returns>
        bool IsPlacable(InteriorBuildingProduction production, CityCenter city);

        /// <summary>
        /// Creates the <see cref="InteriorBuilding"/> which is the production result.
        /// </summary>
        /// <param name="owner">The <see cref="Player"/> who owns the result.</param>
        /// <returns>the created <see cref="InteriorBuilding"/> result.</returns>
        InteriorBuilding CreateInteriorBuilding(Player owner);
    }

    /// <summary>
    /// The <see cref="Production"/> class for <see cref="InteriorBuilding"/>
    /// </summary>
    /// <seealso cref="CivModel.Production" />
    public class InteriorBuildingProduction : Production
    {
        private readonly IInteriorBuildingProductionFactory _factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteriorBuildingProduction"/> class.
        /// </summary>
        /// <param name="factory">The factory object of this production kind.</param>
        /// <param name="owner">The <see cref="Player"/> who will own the production.</param>
        /// <param name="totalCost"><see cref="Production.TotalCost"/> of the production</param>
        /// <param name="capacityPerTurn"><see cref="Production.CapacityPerTurn"/> of the production.</param>
        /// <exception cref="ArgumentException">totalCost is not positive</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacityPerTurn"/> is not in [0, <see cref="Production.TotalCost"/>]</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory"/> is <c>null</c>
        /// or
        /// <paramref name="owner"/> is <c>null</c>
        /// </exception>
        public InteriorBuildingProduction(
            IInteriorBuildingProductionFactory factory, Player owner,
            double totalCost, double capacityPerTurn)
            : base(factory, owner, totalCost, capacityPerTurn)
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
            if (point.TileBuilding is CityCenter city)
                return _factory.IsPlacable(this, city);
            return false;
        }

        /// <summary>
        /// Places the production result at the specified point.
        /// </summary>
        /// <param name="point">The point to place the production result.</param>
        /// <exception cref="InvalidOperationException">production is not completed yet</exception>
        /// <exception cref="ArgumentException">point is invalid</exception>
        public override void Place(Terrain.Point point)
        {
            if (!Completed)
                throw new InvalidOperationException("production is not completed yet");
            if (!IsPlacable(point))
                throw new ArgumentException("point is invalid");

            var obj = _factory.CreateInteriorBuilding(Owner);
            obj.City = (CityCenter)point.TileBuilding;
        }
    }
}
