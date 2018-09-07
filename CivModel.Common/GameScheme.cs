using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CivModel.Common
{
    public class GameSchemeFactory : IGameSchemeFactory
    {
        public static Guid ClassGuid { get; } = new Guid("AB3CC73A-5756-4266-8DAC-42A610421DDA");
        public Guid Guid => ClassGuid;

        public Type SchemeType => typeof(GameScheme);
        public IEnumerable<Guid> Dependencies => Enumerable.Empty<Guid>();
        public IEnumerable<IGameSchemeFactory> KnownSchemeFactories => Enumerable.Empty<IGameSchemeFactory>();

        public IGameScheme Create()
        {
            return new GameScheme(this);
        }
    }

    public class GameScheme : IGameConstantScheme, IGameStartupScheme
    {
        private static readonly IProductionFactory[] _productions = {
            CityCenterProductionFactory.Instance,
            PioneerProductionFactory.Instance,
            FakeFactoryProductionFactory.Instance,
            FakeLaboratoryProductionFactory.Instance,
            FakeFortressProductionFactory.Instance,
            PioneerProductionFactory.Instance,
        };

        public GameSchemeFactory Factory { get; }
        IGameSchemeFactory IGameScheme.Factory => Factory;

        public bool OnlyDefaultPlayers => false;
        public int DefaultNumberOfPlayers => 9;

        public bool OnlyDefaultTerrain => false;
        public int DefaultTerrainWidth => 42;
        public int DefaultTerrainHeight => 12;

        public double GoldCoefficient => 1;

        public double PopulationConstant => 0.1;
        public double PopulationHappinessCoefficient => 0.01;

        public double HappinessCoefficient => 30;

        public double LaborHappinessCoefficient => 0.008;
        public double ResearchHappinessCoefficient => 0.005;

        public double EconomicRequireCoefficient => 0.2;
        public double EconomicRequireTaxRateConstant => 0.2;

        public double ResearchRequireCoefficient => 0.2;

        public GameScheme(GameSchemeFactory factory)
        {
            Factory = factory ?? throw new ArgumentNullException("factory");
        }

        public void OnAfterInitialized(Game game)
        {
            foreach (var player in game.Players)
            {
                foreach (var p in _productions)
                {
                    player.AvailableProduction.Add(p);
                }
            }
        }

        public void InitializeGame(Game game, bool isNewGame)
        {
            if (game == null)
                throw new ArgumentNullException("game");

            if (isNewGame)
            {
                var random = new Random();

                int count = game.Terrain.Width * game.Terrain.Height;
                var tiles = Enumerable.Repeat(false, count).ToArray();

                foreach (var player in game.Players)
                {
                    int index = random.Next(count);
                    while (tiles[index])
                        ++index;

                    var pt = game.Terrain.GetPoint(index);
                    foreach (var adj in pt.AdjacentsWithinDistance(6))
                    {
                        if (adj.HasValue && !tiles[adj.Value.Index])
                        {
                            tiles[adj.Value.Index] = true;
                            --count;
                        }
                    }

                    new CityCenter(player, pt).OnAfterProduce(null);
                }
            }
        }
    }
}
