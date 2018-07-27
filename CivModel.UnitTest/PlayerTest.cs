using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CivModel.UnitTest
{
    /// <summary>
    /// Player의 요약 설명
    /// </summary>
    [TestClass]
    public class PlayerTest
    {
        private Game _game;

        [TestInitialize]
        public void TestInit()
        {
            _game = Utility.LoadGame();
        }

        [TestMethod]
        public void PlayerNumberTest()
        {
            for (int idx = 0; idx < _game.Players.Count; ++idx)
            {
                Assert.AreEqual(idx, _game.Players[idx].PlayerNumber);
            }

            // test twice (Player.PlayerNumber uses lazy evalution)
            for (int idx = 0; idx < _game.Players.Count; ++idx)
            {
                Assert.AreEqual(idx, _game.Players[idx].PlayerNumber);
            }
        }
    }
}
