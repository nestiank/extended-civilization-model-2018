using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CivObservable.UnitTest
{
    [TestClass]
    public class SafeIterationListTest
    {
        private SafeIterationList<int> _list;

        [TestInitialize]
        public void TestInit()
        {
            _list = new SafeIterationList<int>();
        }

        [TestMethod]
        public void ListInitTest()
        {
            Assert.AreEqual(0, _list.Count);
            foreach (int x in _list)
                Assert.Fail();
        }

        [TestMethod]
        public void ListAddTest()
        {
            _list.Add(1);
            _list.Add(2);
            _list.Add(3);
            Assert.AreEqual(1, _list[0]);
            Assert.AreEqual(2, _list[1]);
            Assert.AreEqual(3, _list[2]);
            Assert.AreEqual(3, _list.Count);

            int n = 1;
            foreach (int x in _list)
                Assert.AreEqual(n++, x);

            _list.Add(4);
            Assert.AreEqual(1, _list[0]);
            Assert.AreEqual(2, _list[1]);
            Assert.AreEqual(3, _list[2]);
            Assert.AreEqual(4, _list[3]);
            Assert.AreEqual(4, _list.Count);

            n = 1;
            foreach (int x in _list)
                Assert.AreEqual(n++, x);
        }

        [TestMethod]
        public void NestedLoopTest()
        {
            (int x, int y)[] data1 = {
                (1, 1), (1, 2), (1, 3), (1, 4),
                (2, 1), (2, 2), (2, 3), (2, 4),
                (3, 1), (3, 2), (3, 3), (3, 4),
                (4, 1), (4, 2), (4, 3), (4, 4),
            };
            _list.Add(1);
            _list.Add(2);
            _list.Add(3);
            _list.Add(4);
            int idx = 0;
            foreach (int x in _list)
            {
                foreach (int y in _list)
                {
                    Assert.AreEqual(data1[idx++], (x, y));
                }
            }
        }

        [TestMethod]
        public void NestedLoopModifyTest()
        {
            (int x, int y)[] data1 = {
                (1, 2), (1, 3), (1, 4),
                (2, 1), (2, 3), (2, 4),
                (3, 1), (3, 2), (3, 4),
                (4, 1), (4, 2), (4, 3)
            };
            _list.Add(1);
            _list.Add(2);
            _list.Add(3);
            _list.Add(4);
            int idx = 0;
            foreach (int x in _list)
            {
                _list.Remove(x);
                foreach (int y in _list)
                {
                    Assert.AreEqual(data1[idx++], (x, y));
                }
                _list.Add(x);
            }
        }
    }
}
