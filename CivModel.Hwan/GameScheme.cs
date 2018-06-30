using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Hwan
{
    public class HwanPlayerConstant
    {
        public const int HwanPlayer = 0;
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

    public class GameScheme : IGameAdditionScheme
    {
        public IGameSchemeFactory Factory => _factory;
        private readonly GameSchemeFactory _factory;

        public IEnumerable<IProductionFactory> AdditionalProductionFactory
        {
            get
            {
                yield return BrainwashedEMUKnightProductionFactory.Instance;
                yield return DecentralizedMilitaryProductionFactory.Instance;
                yield return JackieChanProductionFactory.Instance;
                yield return JediKnightProductionFactory.Instance;
                yield return LEOSpaceArmadaProductionFactory.Instance;
                yield return ProtoNinjaProductionFactory.Instance;
                yield return UnicornOrderProductionFactory.Instance;
                yield return SpyProductionFactory.Instance;
                yield return HwanEmpireCityProductionFactory.Instance;
                yield return HwanEmpireCityCentralLabProductionFactory.Instance;
                yield return HwanEmpireFIRFactoryProductionFactory.Instance;
                yield return HwanEmpireFIRFortressProductionFactory.Instance;
                yield return HwanEmpireIbizaProductionFactory.Instance;
                yield return HwanEmpireKimchiFactoryProductionFactory.Instance;
                yield return HwanEmpireLatifundiumProductionFactory.Instance;
                yield return HwanEmpireSungsimdangProductionFactory.Instance;
                yield return HwanEmpireVigilantProductionFactory.Instance;
            }
        }

        public GameScheme(GameSchemeFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException("factory");
        }

        public void RegisterGuid(Game game)
        {
            game.GuidManager.RegisterGuid(BrainwashedEMUKnight.ClassGuid, (p, t) => new BrainwashedEMUKnight(p, t));
            game.GuidManager.RegisterGuid(DecentralizedMilitary.ClassGuid, (p, t) => new DecentralizedMilitary(p, t));
            game.GuidManager.RegisterGuid(JackieChan.ClassGuid, (p, t) => new JackieChan(p, t));
            game.GuidManager.RegisterGuid(JediKnight.ClassGuid, (p, t) => new JediKnight(p, t));
            game.GuidManager.RegisterGuid(LEOSpaceArmada.ClassGuid, (p, t) => new LEOSpaceArmada(p, t));
            game.GuidManager.RegisterGuid(ProtoNinja.ClassGuid, (p, t) => new ProtoNinja(p, t));
            game.GuidManager.RegisterGuid(UnicornOrder.ClassGuid, (p, t) => new UnicornOrder(p, t));
            game.GuidManager.RegisterGuid(Spy.ClassGuid, (p, t) => new Spy(p, t));
            game.GuidManager.RegisterGuid(HwanEmpireCity.ClassGuid, (p, t) => new HwanEmpireCity(p, t));
            game.GuidManager.RegisterGuid(HwanEmpireCityCentralLab.ClassGuid, (c) => new HwanEmpireCityCentralLab(c));
            game.GuidManager.RegisterGuid(HwanEmpireFIRFactory.ClassGuid, (c) => new HwanEmpireFIRFactory(c));
            game.GuidManager.RegisterGuid(HwanEmpireFIRFortress.ClassGuid, (p, t) => new HwanEmpireFIRFortress(p, t));
            game.GuidManager.RegisterGuid(HwanEmpireIbiza.ClassGuid, (p, t) => new HwanEmpireIbiza(p, t));
            game.GuidManager.RegisterGuid(HwanEmpireKimchiFactory.ClassGuid, (p, t) => new HwanEmpireKimchiFactory(p, t));
            game.GuidManager.RegisterGuid(HwanEmpireLatifundium.ClassGuid, (p, t) => new HwanEmpireLatifundium(p, t));
            game.GuidManager.RegisterGuid(HwanEmpireSungsimdang.ClassGuid, (c) => new HwanEmpireSungsimdang(c));
            game.GuidManager.RegisterGuid(HwanEmpireVigilant.ClassGuid, (c) => new HwanEmpireVigilant(c));
        }

        public void OnAfterInitialized(Game game)
        {
            foreach (var p in AdditionalProductionFactory)
            {
                game.Players[HwanPlayerConstant.HwanPlayer].AvailableProduction.Add(p);
            }
        }
    }
}
