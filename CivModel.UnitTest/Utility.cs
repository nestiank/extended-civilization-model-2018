using System;
using System.Linq;
using System.IO;

namespace CivModel.UnitTest
{
    static class Utility
    {
        public static Game LoadGame()
        {
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(Properties.Resources.map);
                writer.Flush();

                stream.Position = 0;
                var reader = new StreamReader(stream);

                var knownFactory = new IGameSchemeFactory[] {
                    new CivModel.Common.GameSchemeFactory(),
                    new CivModel.AI.GameSchemeFactory(),
                    new CivModel.Finno.GameSchemeFactory(),
                    new CivModel.Hwan.GameSchemeFactory(),
                    new CivModel.Zap.GameSchemeFactory(),
                    new CivModel.Quests.GameSchemeFactory(),
                };
                return new Game(reader, knownFactory);
            }
        }

        public static Terrain.Point[] FindTileBlock(Game game, int size, Func<Terrain.Point, bool> predicate)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size), "size is negative");

            for (int y = size; y < game.Terrain.Height - size; ++y)
            {
                for (int x = size; x < game.Terrain.Width - size; ++x)
                {
                    var point = game.Terrain.GetPoint(x, y);
                    var list = point.AdjacentsWithinDistance(size).ToArray();

                    if (list.All(p => p.HasValue && predicate(p.Value)))
                    {
                        return list.Cast<Terrain.Point>().ToArray();
                    }
                }
            }

            throw new ArgumentException("size is too big", nameof(size));
        }
    }
}
