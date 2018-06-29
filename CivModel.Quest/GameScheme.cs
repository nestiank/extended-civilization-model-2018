using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
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

    public class GameScheme : IGameAdditionScheme, ITurnObserver
    {
        public Game Game;
        public IGameSchemeFactory Factory => _factory;
        private readonly GameSchemeFactory _factory;

        public IEnumerable<IProductionFactory> AdditionalProductionFactory
            => Enumerable.Empty<IProductionFactory>();

        public GameScheme(GameSchemeFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException("factory");
        }

        public void RegisterGuid(Game game)
        {
 
        }

        public void PreTurn()
        {

        }

        public void PostTurn()
        {
        }

        public void PrePlayerSubTurn(Player playerInTurn)
        {

        }

        public void PostPlayerSubTurn(Player playerInTurn)
        {

        }

        public void OnAfterInitialized(Game game)
        {
            this.Game = game;
            game.TurnObservable.AddObserver(this);

            if (Game.Players[CivModel.Finno.FinnoPlayerConstant.FinnoPlayer].Research >= 0)
            {
                var p = Game.Players[CivModel.Finno.FinnoPlayerConstant.FinnoPlayer];
                new QuestWarAliance(p).Deploy();
            }

            if (Game.Players[CivModel.Hwan.HwanPlayerConstant.HwanPlayer].Research >= 0)
            {
                var p = Game.Players[CivModel.Hwan.HwanPlayerConstant.HwanPlayer];
                new QuestSubAirspaceDomination(p).Deploy();
            }
        }
    }
}
