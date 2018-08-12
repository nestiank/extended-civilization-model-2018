using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The factory interface of <see cref="Production"/>
    /// </summary>
    /// <seealso cref="Player.AvailableProduction"/>
    public interface IProductionFactory
    {
        /// <summary>
        /// The type of production result object. <c>null</c> if the production result is not an object.
        /// </summary>
        Type ResultType { get; }

        /// <summary>
        /// Creates the <see cref="Production"/> object
        /// </summary>
        /// <param name="owner">The player who owns the <see cref="Production"/> object.</param>
        /// <returns>the created <see cref="Production"/> object</returns>
        Production Create(Player owner);
    }

    static class ProductionFactoryHelper
    {
        public static ProductionResultPrototype GetResultPrototype(this IProductionFactory factory, Game game)
        {
            return game.GetPrototype<ProductionResultPrototype>(factory.ResultType);
        }

        public static double GetTotalLaborCost(this IProductionFactory factory, Game game)
        {
            return factory.GetResultPrototype(game).TotalLaborCost;
        }

        public static double GetLaborCapacityPerTurn(this IProductionFactory factory, Game game)
        {
            return factory.GetResultPrototype(game).LaborCapacityPerTurn;
        }

        public static double GetTotalGoldCost(this IProductionFactory factory, Game game)
        {
            return factory.GetResultPrototype(game).TotalGoldCost;
        }

        public static double GetGoldCapacityPerTurn(this IProductionFactory factory, Game game)
        {
            return factory.GetResultPrototype(game).GoldCapacityPerTurn;
        }
    }
}
