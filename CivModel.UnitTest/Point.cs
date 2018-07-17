using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CivModel.UnitTest
{
    [TestClass]
    public class Point
    {
        // from docs/Coordinate System.pptx
        private static readonly (int x, int y, int a, int b, int c)[] _predefined = {
            (-1, -2, 2, 0, -2), (0, -2, 1, 1, -2), (1, -2, 0, 2, -2),
            (-1, -1, 2, -1, -1), (0, -1, 1, 0, -1), (1, -1, 0, 1, -1), (2, -1, -1, 2, -1),
            (-2, 0, 2, -2, 0), (-1, 0, 1, -1, 0), (0, 0, 0, 0, 0), (1, 0, -1, 1, 0), (2, 0, -2, 2, 0),
            (-1, 1, 1, -2, 1), (0, 1, 0, -1, 1), (1, 1, -1, 0, 1), (2, 1, -2, 1, 1),
            (-1, 2, 0, -2, 2), (0, 2, -1, -1, 2), (1, 2, -2, 0, 2),
        };

        [TestMethod]
        public void CoordinatePredefTest()
        {
            foreach ((int x, int y, int a, int b, int c) in _predefined)
            {
                var p1 = Position.FromPhysical(x, y);
                var p2 = Position.FromLogical(a, b, c);
                Assert.AreEqual((p1.X, p1.Y, p1.A, p1.B, p1.C), (x, y, a, b, c));
                Assert.AreEqual(p1, p2);
            }

            var list = Position.FromPhysical(0, 0).AdjacentsWithinDistance(2).ToArray();
            Assert.IsTrue(list.Select(p => (p.X, p.Y, p.A, p.B, p.C)).SequenceEqual(_predefined));
        }

        [TestMethod]
        public void Coordinate100Test()
        {
            int next(int x) => x > 0 ? -x : -x + 1;

            for (int x = 0; x <= 100; x = next(x))
            {
                for (int y = 0; y <= 100; y = next(y))
                {
                    var p1 = Position.FromPhysical(x, y);
                    Assert.AreEqual((p1.X, p1.Y), (x, y));

                    var p2 = Position.FromLogical(p1.A, p1.B, p1.C);
                    Assert.AreEqual(p1, p2);
                }
            }
        }

        [TestMethod]
        public void AdjacentsTest()
        {
            var pos = Position.FromLogical(0, 0, 0);
            var adj0 = OldAdjacents(pos);
            var adj1 = pos.Adjacents();
            var adj2 = pos.AdjacentsAtDistance(1).ToArray();

            Assert.IsTrue(adj0.SequenceEqual(adj1));
            Assert.IsTrue(adj1.SequenceEqual(adj2));
        }

        private static Position[] OldAdjacents(Position pos)
        {
            var ret = new Position[6];
            Position tmp;

            tmp = new Position { X = pos.X - 1, Y = pos.Y };
            ret[0] = tmp;

            tmp = new Position { X = pos.X, Y = pos.Y - 1 };
            ret[2] = ret[1] = tmp;

            tmp = new Position { X = pos.X + 1, Y = pos.Y };
            ret[3] = tmp;

            tmp = new Position { X = pos.X, Y = pos.Y + 1 };
            ret[5] = ret[4] = tmp;

            int correction = 1 - (pos.Y % 2) * 2;
            int cidx = 1 - (pos.Y % 2);

            ret[1 + cidx].X += correction;
            ret[5 - cidx].X += correction;

            return ret;
        }
    }
}
