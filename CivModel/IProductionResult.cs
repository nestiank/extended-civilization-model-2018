using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The interface represents the result of <see cref="Production"/>
    /// </summary>
    /// <seealso cref="Production"/>
    public interface IProductionResult
    {
        /// <summary>
        /// Called when production is finished, that is, <see cref="Production.Place(Terrain.Point)"/> is succeeded.
        /// </summary>
        /// <param name="production">The <see cref="Production"/> object that produced this object.</param>
        void OnAfterProduce(Production production);
    }
}
