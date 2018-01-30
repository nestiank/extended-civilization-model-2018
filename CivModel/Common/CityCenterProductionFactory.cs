using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    /// <summary>
    /// The factory interface for <see cref="CityCenter"/>.
    /// </summary>
    /// <seealso cref="CivModel.ITileObjectProductionFactory" />
    public class CityCenterProductionFactory : ITileObjectProductionFactory
    {
        /// <summary>
        /// The singleton instance.of <see cref="CityCenterProductionFactory"/>
        /// </summary>
        public static CityCenterProductionFactory Instance => _instance.Value;
        private static Lazy<CityCenterProductionFactory> _instance
            = new Lazy<CityCenterProductionFactory>(() => new CityCenterProductionFactory());

        private CityCenterProductionFactory()
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
            return point.TileBuilding == null;
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
            return new CityCenter(owner);
        }
    }
}
