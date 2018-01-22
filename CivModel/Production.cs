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
    /// <seealso cref="Player.GetAvailableProduction"/>
    /// <seealso cref="Common.CityCenter.AvailableProduction"/>
    /// <seealso cref="Player.AdditionalAvailableProduction"/>
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
        /// The total cost to finish this production.
        /// </summary>
        public double TotalCost => _totalCost;
        private readonly double _totalCost;

        /// <summary>
        /// The maximum labor which can put into this production per turn.
        /// </summary>
        public double CapacityPerTurn { get; private set; }

        /// <summary>
        /// The total labor inputed so far.
        /// </summary>
        public double LaborInputed { get; private set; } = 0;

        /// <summary>
        /// This property is updated by <see cref="Player.EstimateLaborInputing"/>.
        /// You must call that function before use this property.
        /// </summary>
        public double EstimatedLaborInputing { get; internal set; }

        /// <summary>
        /// Whether this production is completed.
        /// </summary>
        public bool Completed { get; private set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Production"/> class.
        /// </summary>
        /// <param name="factory">The factory object of this production kind.</param>
        /// <param name="owner">The <see cref="Player"/> who will own the production.</param>
        /// <param name="totalCost"><see cref="TotalCost"/> of the production</param>
        /// <param name="capacityPerTurn"><see cref="CapacityPerTurn"/> of the production.</param>
        /// <exception cref="ArgumentException">totalCost is not positive</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacityPerTurn"/> is not in [0, <see cref="TotalCost"/>]</exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory"/> is <c>null</c>
        /// or
        /// <paramref name="owner"/> is <c>null</c>
        /// </exception>
        public Production(
            IProductionFactory factory, Player owner,
            double totalCost, double capacityPerTurn)
        {
            if (totalCost < 0)
                throw new ArgumentException("totalCost is not positive", "totalCost");
            if (capacityPerTurn < 0 || capacityPerTurn > totalCost)
                throw new ArgumentOutOfRangeException("capacityPerTurn", capacityPerTurn, 
                    "capacityPerTurn is not in [0, TotalCost]");

            _factory = factory ?? throw new ArgumentNullException("factory");
            _owner = owner ?? throw new ArgumentNullException("owner");
            _totalCost = totalCost;
            CapacityPerTurn = capacityPerTurn;
        }

        /// <summary>
        /// check how much labor is inputed into this production in this turn
        /// </summary>
        /// <param name="labor">labor amount which you want to put</param>
        /// <returns>maximum labor amount possible to put, less than <paramref name="labor"/></returns>
        public double GetAvailableInputLabor(double labor)
        {
            if (Completed)
                throw new InvalidOperationException("Production.InputLabor(): production is already done");

            double capacity = Math.Min(CapacityPerTurn, TotalCost - LaborInputed);
            return Math.Min(labor, capacity);
        }

        /// <summary>
        /// input labor into this production
        /// </summary>
        /// <param name="labor">labor amount to input</param>
        /// <returns>labor amount which is really used. it can be different from the parameter.</returns>
        public double InputLabor(double labor)
        {
            labor = GetAvailableInputLabor(labor);

            LaborInputed += labor;
            if (LaborInputed >= TotalCost)
                Completed = true;

            return labor;
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
