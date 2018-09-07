using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CivModel
{
    /// <summary>
    /// Represents a game constants storage. Constant values are copied from an applied <seealso cref="IGameConstants"/>.
    /// </summary>
    /// <seealso cref="IGameConstants"/>
    /// <seealso cref="Game.Constants"/>
    public sealed class GameConstants : IGameConstants
    {
        /// <summary>
        /// Coefficient for <see cref="Player.GoldIncome" />.
        /// </summary>
        public double GoldCoefficient { get; }

        /// <summary>
        /// Constant amount of <see cref="CityBase.Population" />.
        /// </summary>
        public double PopulationConstant { get; }

        /// <summary>
        /// Coefficient of <see cref="Player.Happiness" /> for <see cref="Player.Population" />.
        /// </summary>
        public double PopulationHappinessCoefficient { get; }

        /// <summary>
        /// Coefficient for <see cref="Player.HappinessIncome" />.
        /// </summary>
        public double HappinessCoefficient { get; }

        /// <summary>
        /// Coefficient of <see cref="Player.Happiness" /> for <see cref="Player.Labor" />.
        /// </summary>
        public double LaborHappinessCoefficient { get; }

        /// <summary>
        /// Coefficient of <see cref="Player.Happiness" /> for <see cref="Player.ResearchIncome" />.
        /// </summary>
        public double ResearchHappinessCoefficient { get; }

        /// <summary>
        /// Coefficient for <see cref="Player.BasicEconomicRequire" />.
        /// </summary>
        public double EconomicRequireCoefficient { get; }

        /// <summary>
        /// Constant amount of <see cref="Player.TaxRate" /> for <see cref="Player.BasicEconomicRequire" />.
        /// </summary>
        public double EconomicRequireTaxRateConstant { get; }

        /// <summary>
        /// Coefficient for <see cref="Player.BasicResearchRequire" />.
        /// </summary>
        public double ResearchRequireCoefficient { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameConstants"/> class.
        /// </summary>
        /// <param name="constants">The <see cref="IGameConstants"/> object holding constant values.</param>
        public GameConstants(IGameConstants constants)
        {
            GoldCoefficient = constants.GoldCoefficient;
            PopulationConstant = constants.PopulationConstant;
            PopulationHappinessCoefficient = constants.PopulationHappinessCoefficient;
            HappinessCoefficient = constants.HappinessCoefficient;
            LaborHappinessCoefficient = constants.LaborHappinessCoefficient;
            ResearchHappinessCoefficient = constants.ResearchHappinessCoefficient;
            EconomicRequireCoefficient = constants.EconomicRequireCoefficient;
            EconomicRequireTaxRateConstant = constants.EconomicRequireTaxRateConstant;
            ResearchRequireCoefficient = constants.ResearchRequireCoefficient;
        }

        internal GameConstants(XElement node)
        {
            var xmlns = PrototypeLoader.Xmlns;
            GoldCoefficient = Convert.ToDouble(node.Element(xmlns + "GoldCoefficient").Value);
            PopulationConstant = Convert.ToDouble(node.Element(xmlns + "PopulationConstant").Value);
            PopulationHappinessCoefficient = Convert.ToDouble(node.Element(xmlns + "PopulationHappinessCoefficient").Value);
            HappinessCoefficient = Convert.ToDouble(node.Element(xmlns + "HappinessCoefficient").Value);
            LaborHappinessCoefficient = Convert.ToDouble(node.Element(xmlns + "LaborHappinessCoefficient").Value);
            ResearchHappinessCoefficient = Convert.ToDouble(node.Element(xmlns + "ResearchHappinessCoefficient").Value);
            EconomicRequireCoefficient = Convert.ToDouble(node.Element(xmlns + "EconomicRequireCoefficient").Value);
            EconomicRequireTaxRateConstant = Convert.ToDouble(node.Element(xmlns + "EconomicRequireTaxRateConstant").Value);
            ResearchRequireCoefficient = Convert.ToDouble(node.Element(xmlns + "ResearchRequireCoefficient").Value);
        }
    }
}
