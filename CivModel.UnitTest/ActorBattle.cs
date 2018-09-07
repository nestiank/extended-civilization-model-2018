using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CivModel.UnitTest
{
    [TestClass]
    public class ActorBattle
    {
        [TestMethod]
        public void DrawAliveTest()
        {
            var (game, unit1, unit2) = Setup();
            unit1.RemainHP = unit2.DefencePower + 1;
            unit2.RemainHP = unit1.AttackPower + 1;

            var rs = unit1.MeleeAttackTo(unit2);
            Assert.IsTrue(rs == BattleResult.DrawAlive);
        }

        [TestMethod]
        public void DrawDeadTest()
        {
            var (game, unit1, unit2) = Setup();
            unit1.RemainHP = unit2.DefencePower;
            unit2.RemainHP = unit1.AttackPower;

            var rs = unit1.MeleeAttackTo(unit2);
            Assert.IsTrue(rs == BattleResult.DrawDead);
        }

        [TestMethod]
        public void VictoryTest()
        {
            var (game, unit1, unit2) = Setup();
            unit1.RemainHP = unit2.DefencePower + 1;
            unit2.RemainHP = unit1.AttackPower;

            var rs = unit1.MeleeAttackTo(unit2);
            Assert.IsTrue(rs == BattleResult.Victory);
        }

        [TestMethod]
        public void DefeatedTest()
        {
            var (game, unit1, unit2) = Setup();
            unit1.RemainHP = unit2.DefencePower;
            unit2.RemainHP = unit1.AttackPower + 1;

            var rs = unit1.MeleeAttackTo(unit2);
            Assert.IsTrue(rs == BattleResult.Defeated);
        }

        private static (Game game, FakeModule.FakeKnight unit1, FakeModule.FakeKnight unit2) Setup()
        {
            var game = Utility.LoadGame();

            var block = Utility.FindTileBlock(game, 3, pt => pt.Unit == null && pt.TileBuilding == null);

            var unit1 = new FakeModule.FakeKnight(game.Players[0], block[0]);
            var unit2 = new FakeModule.FakeKnight(game.Players[1], block[1]);

            return (game, unit1, unit2);
        }
    }
}
