using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    /// <summary>
    /// Represents [factory] interior building.
    /// </summary>
    /// <seealso cref="InteriorBuilding"/>
    public sealed class FactoryBuilding : InteriorBuilding
    {
        /// <summary>
        /// The unique identifier of this class.
        /// </summary>
        public static Guid ClassGuid { get; } = new Guid("A2AE33B4-5543-4751-8681-E958DFC1A511");

        /// <summary>
        /// The unique identifier of this class.
        /// </summary>
        public override Guid Guid => ClassGuid;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteriorBuilding"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="Player"/> who will own the building.</param>
        /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <c>null</c>.</exception>
        public FactoryBuilding(Player owner) : base(owner) { }
    }

    /// <summary>
    /// The factory interface for <see cref="FactoryBuilding"/>.
    /// </summary>
    /// <seealso cref="CivModel.IInteriorBuildingProductionFactory" />
    public class FactoryBuildingProductionFactory : IInteriorBuildingProductionFactory
    {
        /// <summary>
        /// The singleton instance.of <see cref="FactoryBuildingProductionFactory"/>
        /// </summary>
        public static FactoryBuildingProductionFactory Instance => _instance.Value;
        private static Lazy<FactoryBuildingProductionFactory> _instance
            = new Lazy<FactoryBuildingProductionFactory>(() => new FactoryBuildingProductionFactory());

        private FactoryBuildingProductionFactory()
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
            return new InteriorBuildingProduction(this, owner, 5, 2);
        }

        /// <summary>
        /// Determines whether the production result is placable in the specified city.
        /// </summary>
        /// <param name="production">The production.</param>
        /// <param name="city">The city to test to place the production result.</param>
        /// <returns>
        ///   <c>true</c> if the production is placable; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPlacable(InteriorBuildingProduction production, CityCenter city)
        {
            return city.Owner == production.Owner;
        }

        /// <summary>
        /// Creates the <see cref="InteriorBuilding"/> which is the production result.
        /// </summary>
        /// <param name="owner">The <see cref="Player"/> who owns the result.</param>
        /// <returns>the created <see cref="InteriorBuilding"/> result.</returns>
        public InteriorBuilding CreateInteriorBuilding(Player owner)
        {
            return new FactoryBuilding(owner);
        }
    }
}
