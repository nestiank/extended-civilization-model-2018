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
        /// Creates the <see cref="Production"/> object
        /// </summary>
        /// <param name="owner">The player who owns the <see cref="Production"/> object.</param>
        /// <returns>the created <see cref="Production"/> object</returns>
        Production Create(Player owner);
    }

    /// <summary>
    /// An abstract class represents a production.
    /// </summary>
    public abstract class Production
    {
        /// <summary>
        /// The factory object of this production kind.
        /// </summary>
        public IProductionFactory Factory => _factory;
        private readonly IProductionFactory _factory;

        /// <summary>
        /// The player who owns this production.
        /// </summary>
        public Player Owner => _owner;
        private readonly Player _owner;

        /// <summary>
        /// The total labor cost to finish this production.
        /// </summary>
        public double TotalLaborCost => _totalLaborCost;
        private readonly double _totalLaborCost;

        /// <summary>
        /// The total gold cost to finish this production.
        /// </summary>
        public double TotalGoldCost => _totalGoldCost;
        private readonly double _totalGoldCost;

        /// <summary>
        /// The maximum labor which can put into this production per turn.
        /// </summary>
        public double LaborCapacityPerTurn { get; private set; }

        /// <summary>
        /// The maximum gold which can put into this production per turn.
        /// </summary>
        public double GoldCapacityPerTurn { get; private set; }

        /// <summary>
        /// The total labor inputed so far.
        /// </summary>
        public double LaborInputed { get; private set; } = 0;

        /// <summary>
        /// The total gold inputed so far.
        /// </summary>
        public double GoldInputed { get; private set; } = 0;

        /// <summary>
        /// This property is updated by <see cref="Player.EstimateInputsForProduction"/>.
        /// You must call that function before use this property.
        /// </summary>
        /// <seealso cref="Player.EstimateInputsForProduction"/>
        public double EstimatedLaborInputing { get; internal set; }

        /// <summary>
        /// This property is updated by <see cref="Player.EstimateInputsForProduction"/>.
        /// You must call that function before use this property.
        /// </summary>
        /// <seealso cref="Player.EstimateInputsForProduction"/>
        public double EstimatedGoldInputing { get; internal set; }

        /// <summary>
        /// Whether this production is completed.
        /// </summary>
        /// <remarks>
        /// You can mark a production not completed as completed, but cannot do opposite.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Cannot mark a production completed as not completed
        /// </exception>
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted && !value)
                    throw new InvalidOperationException("Cannot mark a production completed as not completed");

                _isCompleted = value;
            }
        }
        private bool _isCompleted = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Production"/> class.
        /// </summary>
        /// <param name="factory">The factory object of this production kind.</param>
        /// <param name="owner">The <see cref="Player"/> who will own the production.</param>
        /// <param name="totalLaborCost"><see cref="TotalLaborCost"/> of the production</param>
        /// <param name="laborCapacityPerTurn"><see cref="LaborCapacityPerTurn"/> of the production.</param>
        /// <param name="totalGoldCost"><see cref="TotalGoldCost"/> of the production</param>
        /// <param name="goldCapacityPerTurn"><see cref="GoldCapacityPerTurn"/> of the production.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="totalLaborCost"/> is negative
        /// or
        /// <paramref name="totalGoldCost"/> is negative
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="laborCapacityPerTurn"/> is not in [0, <see cref="Production.TotalLaborCost"/>]
        /// or
        /// <paramref name="goldCapacityPerTurn"/> is not in [0, <see cref="Production.TotalGoldCost"/>]
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory"/> is <c>null</c>
        /// or
        /// <paramref name="owner"/> is <c>null</c>
        /// </exception>
        public Production(
            IProductionFactory factory, Player owner,
            double totalLaborCost, double laborCapacityPerTurn,
            double totalGoldCost, double goldCapacityPerTurn)
        {
            if (totalLaborCost < 0)
                throw new ArgumentException("totalLaborCost is negative", "totalLaborCost");
            if (laborCapacityPerTurn < 0 || laborCapacityPerTurn > totalLaborCost)
                throw new ArgumentOutOfRangeException("laborCapacityPerTurn", laborCapacityPerTurn, 
                    "laborCapacityPerTurn is not in [0, TotalLaborCost]");

            if (totalGoldCost < 0)
                throw new ArgumentException("totalGoldCost is negative", "totalGoldCost");
            if (goldCapacityPerTurn < 0 || goldCapacityPerTurn > totalGoldCost)
                throw new ArgumentOutOfRangeException("goldCapacityPerTurn", goldCapacityPerTurn,
                    "goldCapacityPerTurn is not in [0, TotalGoldCost]");

            _factory = factory ?? throw new ArgumentNullException("factory");
            _owner = owner ?? throw new ArgumentNullException("owner");
            _totalLaborCost = totalLaborCost;
            LaborCapacityPerTurn = laborCapacityPerTurn;
            _totalGoldCost = totalGoldCost;
            GoldCapacityPerTurn = goldCapacityPerTurn;
        }

        /// <summary>
        /// check how much labor can be inputed into this production in this turn
        /// </summary>
        /// <param name="labor">labor amount which you want to put</param>
        /// <exception cref="InvalidOperationException">production is already completed</exception>
        /// <returns>maximum labor amount possible to put, less than <paramref name="labor"/></returns>
        public double GetAvailableInputLabor(double labor)
        {
            if (IsCompleted)
                throw new InvalidOperationException("production is already completed");

            double capacity = Math.Min(LaborCapacityPerTurn, TotalLaborCost - LaborInputed);
            return Math.Min(labor, capacity);
        }

        /// <summary>
        /// check how much gold can be inputed into this production in this turn
        /// </summary>
        /// <param name="gold">gold amount which you want to put</param>
        /// <exception cref="InvalidOperationException">production is already completed</exception>
        /// <returns>maximum gold amount possible to put, less than <paramref name="gold"/></returns>
        public double GetAvailableInputGold(double gold)
        {
            if (IsCompleted)
                throw new InvalidOperationException("production is already completed");

            double capacity = Math.Min(GoldCapacityPerTurn, TotalGoldCost - GoldInputed);
            return Math.Min(gold, capacity);
        }

        /// <summary>
        /// Inputs resources into this production
        /// </summary>
        /// <param name="labor">labor amount to input</param>
        /// <param name="gold">gold amount to input</param>
        /// <exception cref="InvalidOperationException">production is already completed</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="labor"/> is negative
        /// or
        /// <paramref name="gold"/> is negative
        /// </exception>
        /// <returns>The amount which is really inputed. It can be different from the parameter.</returns>
        public Tuple<double, double> InputResources(double labor, double gold)
        {
            if (IsCompleted)
                throw new InvalidOperationException("production is already completed");

            if (labor < 0)
                throw new ArgumentOutOfRangeException(nameof(labor), labor, "labor is negative");
            if (gold < 0)
                throw new ArgumentOutOfRangeException(nameof(gold), gold, "gold is negative");

            labor = GetAvailableInputLabor(labor);
            gold = GetAvailableInputGold(gold);

            LaborInputed += labor;
            GoldInputed += gold;

            if (LaborInputed >= TotalLaborCost && GoldInputed >= TotalGoldCost)
                IsCompleted = true;

            return new Tuple<double, double>(labor, gold);
        }

        /// <summary>
        /// Determines whether the production result is placable at the specified point.
        /// </summary>
        /// <param name="point">The point to test to place the production result.</param>
        /// <returns>
        ///   <c>true</c> if the production is placable; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsPlacable(Terrain.Point point);

        /// <summary>
        /// Places the production result at the specified point.
        /// </summary>
        /// <param name="point">The point to place the production result.</param>
        /// <exception cref="InvalidOperationException">production is not completed yet</exception>
        /// <exception cref="ArgumentException">point is invalid</exception>
        public abstract void Place(Terrain.Point point);
    }
}
