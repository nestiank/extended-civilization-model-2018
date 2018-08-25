using System;
using System.Collections.Generic;
using System.IO;

namespace CivModel
{
    /// <summary>
    /// Represents a game constants storage. Constant values are copied from an applied <seealso cref="CivModel.IGameConstantScheme"/>.
    /// </summary>
    /// <seealso cref="CivModel.IGameConstantScheme"/>
    /// <seealso cref="Game.Constants"/>
    public sealed class GameConstants : IGameConstantScheme
    {
        // These member are not used.
        IGameSchemeFactory IGameScheme.Factory => null;
        void IGameScheme.OnAfterInitialized(Game game) { }

        /// <summary>
        /// Coefficient for <see cref="Player.GoldIncome" />.
        /// </summary>
        public double GoldCoefficient { get; private set; }

        /// <summary>
        /// Constant amount of <see cref="CityBase.Population" />.
        /// </summary>
        public double PopulationConstant { get; private set; }

        /// <summary>
        /// Coefficient of <see cref="Player.Happiness" /> for <see cref="Player.Population" />.
        /// </summary>
        public double PopulationHappinessCoefficient { get; private set; }

        /// <summary>
        /// Coefficient for <see cref="Player.HappinessIncome" />.
        /// </summary>
        public double HappinessCoefficient { get; private set; }

        /// <summary>
        /// Coefficient of <see cref="Player.Happiness" /> for <see cref="Player.Labor" />.
        /// </summary>
        public double LaborHappinessCoefficient { get; private set; }

        /// <summary>
        /// Coefficient of <see cref="Player.Happiness" /> for <see cref="Player.ResearchIncome" />.
        /// </summary>
        public double ResearchHappinessCoefficient { get; private set; }

        /// <summary>
        /// Coefficient for <see cref="Player.BasicEconomicRequire" />.
        /// </summary>
        public double EconomicRequireCoefficient { get; private set; }

        /// <summary>
        /// Constant amount of <see cref="Player.TaxRate" /> for <see cref="Player.BasicEconomicRequire" />.
        /// </summary>
        public double EconomicRequireTaxRateConstant { get; private set; }

        /// <summary>
        /// Coefficient for <see cref="Player.BasicResearchRequire" />.
        /// </summary>
        public double ResearchRequireCoefficient { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameConstants"/> class.
        /// </summary>
        /// <param name="scheme">The <see cref="IGameConstantScheme"/> object holding constant values.</param>
        public GameConstants(IGameConstantScheme scheme)
        {
            GoldCoefficient = scheme.GoldCoefficient;
            PopulationConstant = scheme.PopulationConstant;
            PopulationHappinessCoefficient = scheme.PopulationHappinessCoefficient;
            HappinessCoefficient = scheme.HappinessCoefficient;
            LaborHappinessCoefficient = scheme.LaborHappinessCoefficient;
            ResearchHappinessCoefficient = scheme.ResearchHappinessCoefficient;
            EconomicRequireCoefficient = scheme.EconomicRequireCoefficient;
            EconomicRequireTaxRateConstant = scheme.EconomicRequireTaxRateConstant;
            ResearchRequireCoefficient = scheme.ResearchRequireCoefficient;
        }
    }
}
