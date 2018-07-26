using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Zap
{
    public static class EgyptPlayerNumber
    {
        public const int Number = 2;
        public static Player GetPlayerEgypt(this Game game)
        {
            return game.Players[Number];
        }
    }

    public static class AtlantisPlayerNumber
    {
        public const int Number = 3;
        public static Player GetPlayerAtlantis(this Game game)
        {
            return game.Players[Number];
        }
    }

    public static class FishPlayerNumber
    {
        public const int Number = 4;
        public static Player GetPlayerFish(this Game game)
        {
            return game.Players[Number];
        }
    }

    public static class EmuPlayerNumber
    {
        public const int Number = 5;
        public static Player GetPlayerEmu(this Game game)
        {
            return game.Players[Number];
        }
    }

    public static class SwedePlayerNumber
    {
        public const int Number = 6;
        public static Player GetPlayerSwede(this Game game)
        {
            return game.Players[Number];
        }
    }

    public static class RamuPlayerNumber
    {
        public const int Number = 7;
        public static Player GetPlayerRamu(this Game game)
        {
            return game.Players[Number];
        }
    }

    public static class EasterPlayerNumber
    {
        public const int Number = 8;
        public static Player GetPlayerEaster(this Game game)
        {
            return game.Players[Number];
        }
    }
}
