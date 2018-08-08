using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CivObservable.UnitTest
{
    [TestClass]
    public class FixedEventReceiverTest
    {
        private TestEventReceiver _root;
        private int _count;

        [TestInitialize]
        public void Initialize()
        {
            (_root, _count, _) = TestEventReceiver.CreateData();
        }

        [TestMethod]
        public void RaiseDownForwardTest()
        {
            int id = 0;
            FixedEventReceiver.RaiseDownForward(_root, node => {
                Assert.AreEqual(id++, node.Id);
            });
            Assert.AreEqual(_count, id);
        }

        [TestMethod]
        public void RaiseDownBackwardTest()
        {
            int id = _count;
            FixedEventReceiver.RaiseDownBackward(_root, node => {
                Assert.AreEqual(--id, node.Id);
            });
            Assert.AreEqual(0, id);
        }
    }
}
