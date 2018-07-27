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
        /// The total labor cost to finish this production.
        /// </summary>
        double TotalLaborCost { get; }
        /// <summary>
        /// The maximum labor which can put into this production per turn.
        /// </summary>
        double LaborCapacityPerTurn { get; }
        /// <summary>
        /// The total gold cost to finish this production.
        /// </summary>
        double TotalGoldCost { get; }
        /// <summary>
        /// The maximum gold which can put into this production per turn.
        /// </summary>
        double GoldCapacityPerTurn { get; }
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
        /// The <see cref="CivModel.Game"/> object.
        /// </summary>
        public Game Game => Owner.Game;

        /// <summary>
        /// The total labor cost to finish this production.
        /// </summary>
        public double TotalLaborCost { get; private set; }

        /// <summary>
        /// The maximum labor which can put into this production per turn.
        /// </summary>
        public double LaborCapacityPerTurn { get; private set; }

        /// <summary>
        /// The total gold cost to finish this production.
        /// </summary>
        public double TotalGoldCost { get; private set; }

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
        /// The amount of labor to be inputed, estimated by <see cref="Player.EstimateResourceInputs"/>.
        /// </summary>
        /// <remarks>
        /// This property is updated by <see cref="Player.EstimateResourceInputs"/>.
        /// You must call that function before use this property.
        /// </remarks>
        /// <seealso cref="Player.EstimateResourceInputs"/>
        public double EstimatedLaborInputing { get; internal set; }

        /// <summary>
        /// The amount of gold to be inputed, estimated by <see cref="Player.EstimateResourceInputs"/>.
        /// </summary>
        /// <remarks>
        /// This property is updated by <see cref="Player.EstimateResourceInputs"/>.
        /// You must call that function before use this property.
        /// </remarks>
        /// <seealso cref="Player.EstimateResourceInputs"/>
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
        /// <exception cref="ArgumentException">
        /// <see cref="IProductionFactory.TotalLaborCost"/> is negative
        /// or
        /// <see cref="IProductionFactory.TotalGoldCost"/> is negative
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <see cref="IProductionFactory.LaborCapacityPerTurn"/> is not in [0, <see cref="Production.TotalLaborCost"/>]
        /// or
        /// <see cref="IProductionFactory.GoldCapacityPerTurn"/> is not in [0, <see cref="Production.TotalGoldCost"/>]
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory"/> is <c>null</c>
        /// or
        /// <paramref name="owner"/> is <c>null</c>
        /// </exception>
        public Production(IProductionFactory factory, Player owner)
        {
            _factory = factory ?? throw new ArgumentNullException("factory");
            _owner = owner ?? throw new ArgumentNullException("owner");

            if (factory.TotalLaborCost < 0)
                throw new ArgumentException("TotalLaborCost is negative", nameof(factory));
            if (factory.LaborCapacityPerTurn < 0 || factory.LaborCapacityPerTurn > factory.TotalLaborCost)
                throw new ArgumentOutOfRangeException(nameof(factory), factory.LaborCapacityPerTurn, "LaborCapacityPerTurn is not in [0, TotalLaborCost]");

            if (factory.TotalGoldCost < 0)
                throw new ArgumentException("TotalGoldCost is negative", nameof(factory));
            if (factory.GoldCapacityPerTurn < 0 || factory.GoldCapacityPerTurn > factory.TotalGoldCost)
                throw new ArgumentOutOfRangeException(nameof(factory), factory.GoldCapacityPerTurn, "GoldCapacityPerTurn is not in [0, TotalGoldCost]");

            TotalLaborCost = factory.TotalLaborCost;
            LaborCapacityPerTurn = factory.LaborCapacityPerTurn;
            TotalGoldCost = factory.TotalGoldCost;
            GoldCapacityPerTurn = factory.GoldCapacityPerTurn;
        }

        /// <summary>
        /// check how much labor can be inputed into this production in this turn
        /// </summary>
        /// <param name="labor">labor amount which you want to put</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="labor"/> is negative</exception>
        /// <exception cref="InvalidOperationException">production is already completed</exception>
        /// <returns>maximum labor amount possible to put, less than <paramref name="labor"/></returns>
        /// <seealso cref="InputResources(double, double)"/>
        /// <seealso cref="GetAvailableInputGold(double)"/>
        public double GetAvailableInputLabor(double labor)
        {
            if (labor < 0)
                throw new ArgumentOutOfRangeException(nameof(labor), labor, "labor is negative");
            if (IsCompleted)
                throw new InvalidOperationException("production is already completed");

            double capacity = Math.Min(LaborCapacityPerTurn, TotalLaborCost - LaborInputed);
            return Math.Min(labor, capacity);
        }

        /// <summary>
        /// check how much gold can be inputed into this production in this turn
        /// </summary>
        /// <param name="gold">gold amount which you want to put</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="gold"/> is negative</exception>
        /// <exception cref="InvalidOperationException">production is already completed</exception>
        /// <returns>maximum gold amount possible to put, less than <paramref name="gold"/></returns>
        /// <seealso cref="InputResources(double, double)"/>
        /// <seealso cref="GetAvailableInputLabor(double)"/>
        public double GetAvailableInputGold(double gold)
        {
            if (gold < 0)
                throw new ArgumentOutOfRangeException(nameof(gold), gold, "gold is negative");
            if (IsCompleted)
                throw new InvalidOperationException("production is already completed");

            double capacity = Math.Min(GoldCapacityPerTurn, TotalGoldCost - GoldInputed);
            return Math.Min(gold, capacity);
        }

        /// <summary>
        /// Inputs resources into this production
        /// </summary>
        /// <remarks>
        /// The return type of this method is <see cref="ValueTuple{T1, T2}"/> which Unity does not support.<br/>
        /// Use <c>Item1</c> and <c>Item2</c> if explicit tuple names is unavailable.
        /// </remarks>
        /// <param name="labor">labor amount to input</param>
        /// <param name="gold">gold amount to input</param>
        /// <returns>The amount which is really inputed. It can be different from the argument.</returns>
        /// <exception cref="InvalidOperationException">production is already completed</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="labor"/> is negative
        /// or
        /// <paramref name="gold"/> is negative
        /// </exception>
        /// <seealso cref="GetAvailableInputLabor(double)"/>
        /// <seealso cref="GetAvailableInputGold(double)"/>
        public (double inputedLabor, double inputedGold) InputResources(double labor, double gold)
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

            return (labor, gold);
        }

        /// <summary>
        /// Determines whether the production result is placable at the specified point.
        /// </summary>
        /// <param name="point">The point to test to place the production result.</param>
        /// <returns>
        ///   <c>true</c> if the production is placable; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// To implement this method, derived class must override <see cref="CoreIsPlacable(Terrain.Point)"/> method.
        /// </remarks>
        /// <seealso cref="CoreIsPlacable(Terrain.Point)"/>
        public bool IsPlacable(Terrain.Point point)
        {
            return CoreIsPlacable(point);
        }

        /// <summary>
        /// Places the production result at the specified point.
        /// </summary>
        /// <param name="point">The point to place the production result.</param>
        /// <exception cref="InvalidOperationException">production is not completed yet</exception>
        /// <exception cref="ArgumentException">point is invalid</exception>
        /// <returns>The production result.</returns>
        /// <remarks>
        /// To implement this method, derived class must override <see cref="CorePlace(Terrain.Point)"/> method.
        /// </remarks>
        /// <seealso cref="CorePlace(Terrain.Point)"/>
        public IProductionResult Place(Terrain.Point point)
        {
            var result = CorePlace(point);
            result.OnAfterProduce(this);
            Game.ProductionObservable.IterateObserver(o => o.OnProductionDeploy(point, this, result));
            return result;
        }

        /// <summary>
        /// Determines whether the production result is placable at the specified point.
        /// Override this method to implement <see cref="IsPlacable(Terrain.Point)"/>.
        /// </summary>
        /// <param name="point">The point to test to place the production result.</param>
        /// <returns>
        ///   <c>true</c> if the production is placable; otherwise, <c>false</c>.
        /// </returns>
        /// <seealso cref="IsPlacable(Terrain.Point)"/>
        protected abstract bool CoreIsPlacable(Terrain.Point point);

        /// <summary>
        /// Places the production result at the specified point.
        /// Override this method to implement <see cref="Place(Terrain.Point)"/>.
        /// </summary>
        /// <param name="point">The point to place the production result.</param>
        /// <returns>The production result.</returns>
        /// <exception cref="InvalidOperationException">production is not completed yet</exception>
        /// <exception cref="ArgumentException">point is invalid</exception>
        /// <seealso cref="Place(Terrain.Point)"/>
        protected abstract IProductionResult CorePlace(Terrain.Point point);
    }
}
