using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public class GameScheme : IGameConstantScheme, IGameStartupScheme, IGameAdditionScheme
    {
        public IGameSchemeFactory Factory => _factory;
        private readonly GameSchemeFactory _factory;

        public bool OnlyDefaultPlayers => false;
        public int DefaultNumberOfPlayers => 2;

        public bool OnlyDefaultTerrain => false;
        public int DefaultTerrainWidth => 128;
        public int DefaultTerrainHeight => 80;

        public double GoldCoefficient => 1;

        public double PopulationConstant => 0.1;
        public double PopulationHappinessCoefficient => 0.01;

        public double HappinessCoefficient => 1;

        public double LaborHappinessCoefficient => 0.008;
        public double ResearchHappinessCoefficient => 0.005;

        public double EconomicRequireCoefficient => 0.2;
        public double EconomicRequireTaxRateConstant => 0.2;

        public double ResearchRequireCoefficient => 0.2;

        public IEnumerable<IProductionFactory> AdditionalProductionFactory
        {
            get
            {
                yield return CityCenterProductionFactory.Instance;
                yield return PioneerProductionFactory.Instance;
                yield return FakeKnightProductionFactory.Instance;
                yield return FactoryBuildingProductionFactory.Instance;
                yield return LaboratoryBuildingProductionFactory.Instance;
            }
        }

        public GameScheme(GameSchemeFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException("factory");
        }

        public void RegisterGuid(Game game)
        {
            game.GuidManager.RegisterGuid(CityCenter.ClassGuid, (p, t) => new CityCenter(p, t));
            game.GuidManager.RegisterGuid(Pioneer.ClassGuid, (p, t) => new Pioneer(p, t));
            game.GuidManager.RegisterGuid(FakeKnight.ClassGuid, (p, t) => new FakeKnight(p, t));
            game.GuidManager.RegisterGuid(FactoryBuilding.ClassGuid, city => new FactoryBuilding(city));
            game.GuidManager.RegisterGuid(LaboratoryBuilding.ClassGuid, city => new LaboratoryBuilding(city));
        }

        public void InitializeGame(Game game, bool isNewGame)
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
                    } while (pt.TileBuilding != null);

                    new CityCenter(player, pt).ProcessCreation();
                }
            }

            foreach (var player in game.Players)
            {
                new TestQuest(player).Deploy();
            }
        }
    }
}
