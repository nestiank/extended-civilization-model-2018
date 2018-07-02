using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public static class EgyptPlayerConstant
    {
        public const int EgyptPlayer = 2;
        public static Player GetPlayerEgypt(this Game game)
        {
            return game.Players[CivModel.Zap.EgyptPlayerConstant.EgyptPlayer];
        }
    }
    public static class AtlantisPlayerConstant
    {
        public  const int AtlantisPlayer = 3;
        public static Player GetPlayerAtlantis(this Game game)
        {
            return game.Players[CivModel.Zap.AtlantisPlayerConstant.AtlantisPlayer];
        }
    }
    public static class FishPlayerConstant
    {
        public const int FishPlayer = 4;
        public static Player GetPlayerFish(this Game game)
        {
            return game.Players[CivModel.Zap.FishPlayerConstant.FishPlayer];
        }
    }
    public static class EmuPlayerConstant
    {
        public const int EmuPlayer = 5;
        public static Player GetPlayerEmu(this Game game)
        {
            return game.Players[CivModel.Zap.EmuPlayerConstant.EmuPlayer];
        }
    }
    public static class SwedePlayerConstant
    {
        public const int SwedePlayer = 6;
        public static Player GetPlayerSwede(this Game game)
        {
            return game.Players[CivModel.Zap.SwedePlayerConstant.SwedePlayer];
        }
    }
    public static class RamuPlayerConstant
    {
        public const int RamuPlayer = 7;
        public static Player GetPlayerRamu(this Game game)
        {
            return game.Players[CivModel.Zap.RamuPlayerConstant.RamuPlayer];
        }
    }
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

    public class GameScheme : IGameAdditionScheme
    {
        public IGameSchemeFactory Factory => _factory;
        private readonly GameSchemeFactory _factory;

        public IEnumerable<IProductionFactory> AdditionalProductionFactory
        {
            get
            {
                yield return ArmedDivisionProductionFactory.Instance;
                yield return CasinoProductionFactory.Instance;
                yield return CityCenterProductionFactory.Instance;
                yield return CityLabProductionFactory.Instance;
                yield return DecentralizedMilitaryProductionFactory.Instance;
                yield return ZapFactoryProductionFactory.Instance;
                yield return FIRFactoryProductionFactory.Instance;
                yield return FIRFortressProductionFactory.Instance;
                yield return InfantryDivisionProductionFactory.Instance;
                yield return LEOSpaceArmadaProductionFactory.Instance;
                yield return PadawanProductionFactory.Instance;
                yield return PioneerProductionFactory.Instance;
                yield return ZapNinjaProductionFactory.Instance;
            }
        }

        public GameScheme(GameSchemeFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException("factory");
        }

        public void RegisterGuid(Game game)
        {
            game.GuidManager.RegisterGuid(ArmedDivision.ClassGuid, (p, t) => new ArmedDivision(p, t));
            game.GuidManager.RegisterGuid(Casino.ClassGuid, (p, t) => new Casino(p, t));
            game.GuidManager.RegisterGuid(CityCenter.ClassGuid, (p, t) => new CityCenter(p, t, true));
            game.GuidManager.RegisterGuid(CityLab.ClassGuid, (c) => new CityLab(c));
            game.GuidManager.RegisterGuid(DecentralizedMilitary.ClassGuid, (p, t) => new DecentralizedMilitary(p, t));
            game.GuidManager.RegisterGuid(ZapFactory.ClassGuid, (p, t) => new ZapFactory(p, t));
            game.GuidManager.RegisterGuid(FIRFactory.ClassGuid, (c) => new FIRFactory(c));
            game.GuidManager.RegisterGuid(FIRFortress.ClassGuid, (p, t) => new FIRFortress(p, t));
            game.GuidManager.RegisterGuid(InfantryDivision.ClassGuid, (p, t) => new InfantryDivision(p, t));
            game.GuidManager.RegisterGuid(LEOSpaceArmada.ClassGuid, (p, t) => new LEOSpaceArmada(p, t));
            game.GuidManager.RegisterGuid(Padawan.ClassGuid, (p, t) => new Padawan(p, t));
            game.GuidManager.RegisterGuid(Pioneer.ClassGuid, (p, t) => new Pioneer(p, t));
            game.GuidManager.RegisterGuid(ZapNinja.ClassGuid, (p, t) => new ZapNinja(p, t));
        }

        public void OnAfterInitialized(Game game)
        {
            foreach (var p in AdditionalProductionFactory)
            {
                game.Players[EgyptPlayerConstant.EgyptPlayer].AvailableProduction.Add(p);
            }

            foreach (var p in AdditionalProductionFactory)
            {
                game.Players[AtlantisPlayerConstant.AtlantisPlayer].AvailableProduction.Add(p);
            }

            foreach (var p in AdditionalProductionFactory)
            {
                game.Players[FishPlayerConstant.FishPlayer].AvailableProduction.Add(p);
            }

            foreach (var p in AdditionalProductionFactory)
            {
                game.Players[EmuPlayerConstant.EmuPlayer].AvailableProduction.Add(p);
            }

            foreach (var p in AdditionalProductionFactory)
            {
                game.Players[SwedePlayerConstant.SwedePlayer].AvailableProduction.Add(p);
            }

            foreach (var p in AdditionalProductionFactory)
            {
                game.Players[RamuPlayerConstant.RamuPlayer].AvailableProduction.Add(p);
            }
        }
    }
}
