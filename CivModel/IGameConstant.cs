using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The interface represents the constants for <see cref="Game"/>.
    /// </summary>
    /// <seealso cref="IGameConstantsScheme"/>
    public interface IGameConstants
    {
        /// <summary>
        /// Coefficient for <see cref="Player.GoldIncome"/>.
        /// </summary>
        double GoldCoefficient { get; }

        /// <summary>
        /// Constant amount of <see cref="CityBase.Population"/>.
        /// </summary>
        double PopulationConstant { get; }

        /// <summary>
        /// Coefficient of <see cref="Player.Happiness"/> for <see cref="Player.Population"/>.
        /// </summary>
        double PopulationHappinessCoefficient { get; }

        /// <summary>
        /// Coefficient for <see cref="Player.HappinessIncome"/>.
        /// </summary>
        double HappinessCoefficient { get; }

        /// <summary>
        /// Coefficient of <see cref="Player.Happiness"/> for <see cref="Player.Labor"/>.
        /// </summary>
        double LaborHappinessCoefficient { get; }

        /// <summary>
        /// Coefficient of <see cref="Player.Happiness"/> for <see cref="Player.ResearchIncome"/>.
        /// </summary>
        double ResearchHappinessCoefficient { get; }

        /// <summary>
        /// Coefficient for <see cref="Player.BasicEconomicRequire"/>.
        /// </summary>
        double EconomicRequireCoefficient { get; }

        /// <summary>
        /// Constant amount of <see cref="Player.TaxRate"/> for <see cref="Player.BasicEconomicRequire"/>.
        /// </summary>
        double EconomicRequireTaxRateConstant { get; }

        /// <summary>
        /// Coefficient for <see cref="Player.BasicResearchRequire"/>.
        /// </summary>
        double ResearchRequireCoefficient { get; }
    }
}
