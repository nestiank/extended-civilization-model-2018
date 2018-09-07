using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The interface to observe <see cref="Production"/> related events.
    /// </summary>
    /// <seealso cref="Game"/>
    /// <seealso cref="Production"/>
    public interface IProductionObserver
    {
        /// <summary>
        /// Called when <see cref="Player.Production"/> is changed.
        /// </summary>
        /// <param name="player">The player of production list.</param>
        void OnProductionListChanged(Player player);

        /// <summary>
        /// Called when <see cref="Player.Deployment"/> is changed.
        /// </summary>
        /// <param name="player">The player of production list.</param>
        void OnDeploymentListChanged(Player player);

        /// <summary>
        /// Called when the result of <see cref="Production"/> is deployed, that is, <see cref="Production.Place(Terrain.Point)"/> is succeeded.
        /// </summary>
        /// <param name="point">The point to place the production result.</param>
        /// <param name="production">The <see cref="Production"/> object.</param>
        /// <param name="result">The production result object</param>
        void OnProductionDeploy(Terrain.Point point, Production production, object result);
    }
}
