using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivModel.TileBuildings;

namespace CivModel
{
    public class Player : ITurnObserver
    {
        public double Gold { get; private set; } = 0;
        public double GoldIncome => Game.GoldCoefficient * TaxRate;

        public double Happiness { get; private set; } = 100;
        public double HappinessIncome => 0;

        public double Labor => Game.LaborCoefficient1 * (Game.LaborCoefficient2 + Happiness);

        private double _taxRate = 1;
        public double TaxRate
        {
            get => _taxRate;
            set
            {
                if (value < 0 && value > 1)
                    throw new ArgumentException("Player.TaxRate is not in [0, 1]");
                _taxRate = value;
            }
        }

        public List<Unit> Units { get; } = new List<Unit>();
        public List<CityCenter> Cities { get; } = new List<CityCenter>();

        public LinkedList<Production> Production { get; } = new LinkedList<Production>();
        public LinkedList<Production> Deployment { get; } = new LinkedList<Production>();

        private readonly Game _game;
        public Game Game => _game;

        public Player(Game game)
        {
            _game = game;
        }

        public void PreTurn()
        {
        }

        public void PostTurn()
        {
            var dg = GoldIncome;
            var dh = HappinessIncome;

            productionProcess();

            Gold += dg;
            Happiness += dh;
        }

        public void PrePlayerSubTurn(Player playerInTurn)
        {
        }

        public void PostPlayerSubTurn(Player playerInTurn)
        {
        }

        private void productionProcess()
        {
            var labor = Labor;

            for (var node = Production.First; node != null; )
            {
                labor -= node.Value.InputLabor(labor);
                if (node.Value.Completed)
                {
                    node = node.Next;
                    Production.Remove(node);
                    Deployment.AddLast(node.Value);
                }
                else
                {
                    node = node.Next;
                }
            }
        }
    }
}
