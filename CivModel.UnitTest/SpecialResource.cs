using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CivModel.UnitTest
{
    [TestClass]
    public class SpecialResource
    {
        [TestMethod]
        public void SpecialResTest()
        {
            var game = Utility.LoadGame();
            var player = game.Players[0];
            int x = 0;

            foreach (var pr in player.SpecialResource)
                Assert.Fail();

            x = player.SpecialResource[TestSRes.Instance];
            Assert.AreEqual(0, x);

            foreach (var pr in player.SpecialResource)
                Assert.Fail();

            player.SpecialResource[TestSRes.Instance] = 1;
            x = player.SpecialResource[TestSRes.Instance];
            Assert.AreEqual(1, x);

            int count = 0;
            foreach (var pr in player.SpecialResource)
            {
                Assert.AreEqual(0, count++);
                Assert.AreSame(TestSRes.Instance, pr.Key);
                Assert.AreEqual(1, pr.Value);
            }

            player.SpecialResource[TestSRes.Instance] = 0;
            x = player.SpecialResource[TestSRes.Instance];
            Assert.AreEqual(0, x);

            count = 0;
            foreach (var pr in player.SpecialResource)
            {
                Assert.AreEqual(0, count++);
                Assert.AreSame(TestSRes.Instance, pr.Key);
                Assert.AreEqual(0, pr.Value);
            }
        }

        private class TestSRes : ISpecialResource
        {
            private static readonly Lazy<TestSRes> _instance = new Lazy<TestSRes>(() => new TestSRes());
            public static TestSRes Instance => _instance.Value;
            private TestSRes() { }

            public int MaxCount => 1;

            public object EnablePlayer(Player player)
            {
                return null;
            }
        }
    }
}
