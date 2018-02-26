using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Finno
{
    public class GameSchemeFactory : IGameSchemeFactory
    {
        public static Guid ClassGuid { get; } = new Guid("5E43CB90-F860-427D-A43B-57C96091C58B");
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
                yield return AncientSorcererProductionFactory.Instance;
                yield return AutismBeamDroneFactory.Instance;
                yield return DecentralizedMilitaryProductionFactory.Instance;
                yield return ElephantCavalryProductionFactory.Instance;
                yield return EMUHorseArcherProductionFactory.Instance;
                yield return GenghisKhanProductionFactory.Instance;
                yield return JediKnightProductionFactory.Instance;
            }
        }

        public GameScheme(GameSchemeFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException("factory");
        }

        public void RegisterGuid(Game game)
        {
            game.GuidManager.RegisterGuid(AncientSorcerer.ClassGuid, (p, t) => new AncientSorcerer(p, t));
            game.GuidManager.RegisterGuid(AutismBeamDrone.ClassGuid, (p, t) => new AutismBeamDrone(p, t));
            game.GuidManager.RegisterGuid(DecentralizedMilitary.ClassGuid, (p, t) => new DecentralizedMilitary(p, t));
            game.GuidManager.RegisterGuid(ElephantCavalry.ClassGuid, (p, t) => new ElephantCavalry(p, t));
            game.GuidManager.RegisterGuid(EMUHorseArcher.ClassGuid, (p, t) => new EMUHorseArcher(p, t));
            game.GuidManager.RegisterGuid(GenghisKhan.ClassGuid, (p, t) => new GenghisKhan(p, t));
            game.GuidManager.RegisterGuid(FinnoJediKnight.ClassGuid, (p, t) => new FinnoJediKnight(p, t));
        }
    }
}
