using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
