using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public interface IProductionFactory
    {
        Production Create(Player owner);
    }

    public abstract class Production
    {
        private readonly IProductionFactory _factory;
        public IProductionFactory Factory => _factory;

        private readonly Player _owner;
        public Player Owner => _owner;

        private readonly double _totalCost;
        public double TotalCost => _totalCost;

        public double CapacityPerTurn { get; private set; }
        public double LaborInputed { get; private set; } = 0;

        /// <summary>
        /// This property is updated by <see cref="Player.EstimateLaborInputing"/>.
        /// You must call that function before use this property.
        /// </summary>
        public double EstimatedLaborInputing { get; set; }

        public bool Completed { get; private set; } = false;

        public Production(
            IProductionFactory factory, Player owner,
            double totalCost, double capacityPerTurn)
        {
            if (totalCost < 0)
                throw new ArgumentException("totalCost must be positive", "totalCost");
            if (capacityPerTurn < 0 || capacityPerTurn > totalCost)
                throw new ArgumentException("CapacityPerTurn must be in [0, TotalCost]", "CapacityPerTurn");

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

        public abstract bool IsPlacable(Terrain.Point point);
        public abstract void Place(Terrain.Point point);
    }
}
