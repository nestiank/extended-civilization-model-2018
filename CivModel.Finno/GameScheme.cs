using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CivModel.Finno
{
    public static class FinnoPlayerNumber
    {
        public const int Number = 1;
        public static Player GetPlayerFinno(this Game game)
        {
            return game.Players[Number];
        }
    }

    public class GameSchemeFactory : IGameSchemeFactory
    {
        public static Guid ClassGuid { get; } = new Guid("5E43CB90-F860-427D-A43B-57C96091C58B");
        public Guid Guid => ClassGuid;

        public Type SchemeType => typeof(GameScheme);
        public IEnumerable<Guid> Dependencies => Enumerable.Empty<Guid>();
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
            AncientSorcererProductionFactory.Instance,
            AutismBeamDroneFactory.Instance,
            DecentralizedMilitaryProductionFactory.Instance,
            ElephantCavalryProductionFactory.Instance,
            EMUHorseArcherProductionFactory.Instance,
            GenghisKhanProductionFactory.Instance,
            JediKnightProductionFactory.Instance,
            SpyProductionFactory.Instance,
            AncientFinnoFineDustFactoryProductionFactory.Instance,
            AncientFinnoFIRFactoryProductionFactory.Instance,
            AncientFinnoFIRFortressProductionFactory.Instance,
            AncientFinnoGermaniumMineProductionFactory.Instance,
            AncientFinnoLabortoryProductionFactory.Instance,
            AncientFinnoOctagonProductionFactory.Instance,
            AncientFinnoVigilantProductionFactory.Instance,
            AncientFinnoXylitolProductionRegionProductionFactory.Instance,
            FinnoEmpireCityProductionFactory.Instance,
            PreternaturalityProductionFactory.Instance,
        };

        public GameSchemeFactory Factory { get; }
        IGameSchemeFactory IGameScheme.Factory => Factory;

        public GameScheme(GameSchemeFactory factory)
        {
            Factory = factory ?? throw new ArgumentNullException("factory");
        }

        public void OnAfterInitialized(Game game)
        {
            foreach (var p in _productions)
            {
                game.GetPlayerFinno().AvailableProduction.Add(p);
            }
        }
    }
}
