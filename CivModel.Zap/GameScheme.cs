using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CivModel.Zap
{
    public class GameSchemeFactory : IGameSchemeFactory
    {
        public static Guid ClassGuid { get; } = new Guid("57FA8243-995C-460F-B88E-F30C2C7E0807");
        public Guid Guid => ClassGuid;

        public Type SchemeType => typeof(GameScheme);
        public IEnumerable<Guid> Dependencies { get; } = new Guid[] { Common.GameSchemeFactory.ClassGuid };
        public IEnumerable<IGameSchemeFactory> KnownSchemeFactories => Enumerable.Empty<IGameSchemeFactory>();

        public IGameScheme Create()
        {
            return new GameScheme(this);
        }
    }

    public class GameScheme : IGameScheme
    {
        private static readonly IProductionFactory[] _productions = {
            ArmedDivisionProductionFactory.Instance,
            CasinoProductionFactory.Instance,
            CityCenterProductionFactory.Instance,
            CityLabProductionFactory.Instance,
            DecentralizedMilitaryProductionFactory.Instance,
            ZapFactoryProductionFactory.Instance,
            FIRFactoryProductionFactory.Instance,
            FIRFortressProductionFactory.Instance,
            InfantryDivisionProductionFactory.Instance,
            LEOSpaceArmadaProductionFactory.Instance,
            PadawanProductionFactory.Instance,
            PioneerProductionFactory.Instance,
            ZapNinjaProductionFactory.Instance,
        };

        public GameSchemeFactory Factory { get; }
        IGameSchemeFactory IGameScheme.Factory => Factory;

        public GameScheme(GameSchemeFactory factory)
        {
            Factory = factory ?? throw new ArgumentNullException("factory");
        }

        public TextReader GetPackageData()
        {
            return new StringReader(Properties.Resources.package);
        }

        public void OnAfterInitialized(Game game)
        {
            foreach (var p in _productions)
            {
                game.GetPlayerEgypt().AvailableProduction.Add(p);
                game.GetPlayerAtlantis().AvailableProduction.Add(p);
                game.GetPlayerFish().AvailableProduction.Add(p);
                game.GetPlayerEmu().AvailableProduction.Add(p);
                game.GetPlayerSwede().AvailableProduction.Add(p);
                game.GetPlayerRamu().AvailableProduction.Add(p);
                game.GetPlayerEaster().AvailableProduction.Add(p);
            }
        }
    }
}
