using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;
using static CivModel.Zap.EgyptPlayerNumber;
using static CivModel.Zap.AtlantisPlayerNumber;
using static CivModel.Zap.FishPlayerNumber;
using static CivModel.Zap.EmuPlayerNumber;
using static CivModel.Zap.SwedePlayerNumber;
using static CivModel.Zap.RamuPlayerNumber;
using static CivModel.Zap.EasterPlayerNumber;

namespace CivModel.Quests
{
    public class GameSchemeFactory : IGameSchemeFactory
    {
        public static Guid ClassGuid { get; } = new Guid("63715AEC-64F9-4CCD-B8B4-E8932B4FCD21");
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
        public GameSchemeFactory Factory { get; }
        IGameSchemeFactory IGameScheme.Factory => Factory;

        public Game Game { get; private set; }

        public GameScheme(GameSchemeFactory factory)
        {
            Factory = factory ?? throw new ArgumentNullException("factory");
        }

        public void OnAfterInitialized(Game game)
        {
            this.Game = game;

            // Player Name
            game.GetPlayerHwan().PlayerName = "환 제국";
            game.GetPlayerFinno().PlayerName = "수오미 제국";
            game.GetPlayerEgypt().PlayerName = "이집트 캉덤";
            game.GetPlayerAtlantis().PlayerName = "아틀란티스";
            game.GetPlayerFish().PlayerName = "어류 공화국";
            game.GetPlayerEmu().PlayerName = "에뮤 연방";
            game.GetPlayerSwede().PlayerName = "쉬드 왕국";
            game.GetPlayerRamu().PlayerName = "레무리아";
            game.GetPlayerEaster().PlayerName = "이스터 왕국";

            // Hwan Ally
            game.GetPlayerHwan().Team = 0;
            game.GetPlayerEgypt().Team = 0;
            game.GetPlayerFish().Team = 0;
            game.GetPlayerSwede().Team = 0;
            game.GetPlayerEaster().Team = 0;

            // Finno Ally
            game.GetPlayerFinno().Team = 1;
            game.GetPlayerAtlantis().Team = 1;
            game.GetPlayerEmu().Team = 1;
            game.GetPlayerRamu().Team = 1;

            // Ending
            game.GetPlayerHwan().AddAvailableEnding(new HwanUltimateVictory(game));
            game.GetPlayerFinno().AddAvailableEnding(new FinnoUltimateVictory(game));

            foreach (var p in game.Players)
            {
                p.AddAvailableEnding(new UltimateDraw(game));
                p.AddAvailableEnding(new UltimateDefeat(game));
                p.AddAvailableEnding(new ConquerVictory(game));
                p.AddAvailableEnding(new EliminationDefeat(game));
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

            // Ending Quest
            new QuestHwanVictory(Game);
            new QuestFinnoVictory(Game);
            foreach (var p in game.Players)
            {
                new QuestConquerVictory(p);
            }
        }
    }
}
