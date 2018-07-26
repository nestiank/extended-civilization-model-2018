using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;

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

    public class GameScheme : IGameAdditionScheme
    {
        public Game Game { get; private set; }

        public IGameSchemeFactory Factory { get; }

        public IEnumerable<IProductionFactory> AdditionalProductionFactory
            => Enumerable.Empty<IProductionFactory>();

        public GameScheme(GameSchemeFactory factory)
        {
            Factory = factory ?? throw new ArgumentNullException("factory");
        }

        public void RegisterGuid(Game game)
        {
        }

        public void OnAfterInitialized(Game game)
        {
            this.Game = game;

            // Victory & Defeat Condition
            game.GetPlayerHwan().AddVictoryCondition(new HwanUltimateVictory());
            game.GetPlayerFinno().AddVictoryCondition(new FinnoUltimateVictory());

            foreach (var p in game.Players)
            {
                p.AddVictoryCondition(new ConquerVictory());
                p.AddDefeatCondition(new EliminationDefeat());
                p.AddDefeatCondition(new GameEndDefeat());
            }

            // Hwan Main
            new QuestAutismBeamReflex(Game);
            new QuestPorjectCthulhu(Game);
            new QuestEgyptKingdom(Game);

            // Hwan Sub
            new QuestSubAirspaceDomination(Game);
            new QuestSubMoaiForceField(Game);

            // Finno Main
            new QuestWarAliance(Game);
            new QuestAtlantis(Game);
            new QuestRlyeh(Game);

            // Finno Sub
            new QuestSubInterstellarEnergy(Game);
            new QuestSubGeneticEngineering(Game);
        }
    }
}
