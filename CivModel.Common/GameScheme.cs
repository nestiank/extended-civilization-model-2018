using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    public class GameSchemeFactory : IGameSchemeFactory
    {
        public Guid Guid { get; } = new Guid("AB3CC73A-5756-4266-8DAC-42A610421DDA");

        public IGameScheme Create()
        {
            return new GameScheme(this);
        }
    }

    public class GameScheme : IGameScheme
    {
        private readonly GameSchemeFactory _factory;
        public IGameSchemeFactory Factory => _factory;

        internal GameScheme(GameSchemeFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException("factory");
        }

        public bool OnlyDefaultPlayers => false;
        public int DefaultNumberOfPlayers => 2;

        public bool OnlyDefaultTerrain => false;
        public int DefaultTerrainWidth => 128;
        public int DefaultTerrainHeight => 80;

        public double GoldCoefficient => 1;

        public double PopulationCoefficient => 0.1;
        public double PopulationHappinessConstant => 0;

        public double HappinessCoefficient => 1;

        public double LaborCoefficient => 0.1;
        public double LaborHappinessConstant => 0;

        public double EconomicRequireCoefficient => 0.2;
        public double EconomicRequireTaxRateConstant => 0.2;

        public double ResearchRequireCoefficient => 0.2;
        public double ResearchRequireTaxRateConstant => 0.2;

        public void Initialize(Game game, bool isNewGame)
        {
            if (game == null)
                throw new ArgumentNullException("game");

            if (isNewGame)
            {
                var random = new Random();
                foreach (var player in game.Players)
                {
                    Terrain.Point pt;
                    do
                    {
                        int x = random.Next((int)Math.Floor(game.Terrain.Width * 0.1),
                            (int)Math.Ceiling(game.Terrain.Width * 0.9));
                        int y = random.Next((int)Math.Floor(game.Terrain.Height * 0.1),
                            (int)Math.Ceiling(game.Terrain.Height * 0.9));

                        pt = game.Terrain.GetPoint(x, y);
                    } while (pt.Unit != null);

                    var pionner = new Pioneer(player);
                    pionner.PlacedPoint = pt;
                }
            }
            game.Players[0].AdditionalAvailableProduction.Add(JediKnightProductionFactory.Instance);
        }
    }
}
