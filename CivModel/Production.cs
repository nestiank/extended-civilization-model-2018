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
        private readonly Player _owner;
        public Player Owner => _owner;

        private readonly double _totalCost;
        public double TotalCost => _totalCost;

        public double CapacityPerTurn { get; private set; }
        public double LaborInputed { get; private set; } = 0;

        public bool Completed { get; private set; } = false;

        public Production(Player owner, double totalCost, double capacityPerTurn)
        {
            if (owner == null)
                throw new ArgumentNullException("Production ctor: owner is null");
            if (totalCost < 0)
                throw new ArgumentException("Production ctor: TotalCost must be positive");
            if (capacityPerTurn < 0 || capacityPerTurn > totalCost)
                throw new ArgumentException("Production ctor: CapacityPerTurn must be positive and less than TotalCost");

            _owner = owner;
            _totalCost = totalCost;
            CapacityPerTurn = capacityPerTurn;
        }

        /// <summary>
        /// input labor into this production
        /// </summary>
        /// <param name="labor">labor amount to input</param>
        /// <returns>labor amount which is really used. it can be different from the parameter.</returns>
        public double InputLabor(double labor)
        {
            if (Completed)
                throw new InvalidOperationException("Production.InputLabor(): production is already done");

            double capacity = Math.Min(CapacityPerTurn, TotalCost - LaborInputed);
            labor = Math.Min(labor, capacity);

            LaborInputed += labor;
            if (LaborInputed >= TotalCost)
                Completed = true;

            return labor;
        }

        public abstract bool IsPlacable(Terrain.Point point);
        public abstract void Place(Terrain.Point point);
    }
}
