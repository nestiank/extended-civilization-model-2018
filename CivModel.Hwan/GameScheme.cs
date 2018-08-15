using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CivModel.Hwan
{
    public static class HwanPlayerNumber
    {
        public const int Number = 0;
        public static Player GetPlayerHwan(this Game game)
        {
            return game.Players[Number];
        }
    }

    public class GameSchemeFactory : IGameSchemeFactory
    {
        public static Guid ClassGuid { get; } = new Guid("E5FA297B-3BFB-472D-AD8B-01C8A78B1251");
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
            PioneerProductionFactory.Instance,
            BrainwashedEMUKnightProductionFactory.Instance,
            DecentralizedMilitaryProductionFactory.Instance,
            JackieChanProductionFactory.Instance,
            JediKnightProductionFactory.Instance,
            LEOSpaceArmadaProductionFactory.Instance,
            ProtoNinjaProductionFactory.Instance,
            UnicornOrderProductionFactory.Instance,
            SpyProductionFactory.Instance,
            HwanEmpireCityProductionFactory.Instance,
            HwanEmpireCityCentralLabProductionFactory.Instance,
            HwanEmpireFIRFactoryProductionFactory.Instance,
            HwanEmpireFIRFortressProductionFactory.Instance,
            HwanEmpireIbizaProductionFactory.Instance,
            HwanEmpireKimchiFactoryProductionFactory.Instance,
            HwanEmpireLatifundiumProductionFactory.Instance,
            HwanEmpireSungsimdangProductionFactory.Instance,
            HwanEmpireVigilantProductionFactory.Instance,
            PreternaturalityProductionFactory.Instance,
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
                game.GetPlayerHwan().AvailableProduction.Add(p);
            }
        }
    }
}
