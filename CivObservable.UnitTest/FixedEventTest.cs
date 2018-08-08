using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CivObservable.UnitTest
{
    [TestClass]
    public class FixedEventTest
    {
        private FixedEvent<TestEventReceiver> _event;
        private SafeIterationList<IFixedEventReceiver<TestEventReceiver>> _children;

        private TestEventReceiver _root;
        private int _count;
        private List<TestEventReceiver> _data;

        [TestInitialize]
        public void Initialize()
        {
            (_root, _count, _data) = TestEventReceiver.CreateData();

            _event = new FixedEvent<TestEventReceiver>(() => _children);
            _children = new SafeIterationList<IFixedEventReceiver<TestEventReceiver>>();
            foreach (var child in _root.Children)
                _children.Add(child);
        }

        [TestMethod]
        public void RaiseFixedForwardTest()
        {
            int id = 0;
            FixedEventReceiver.RaiseDownForward(_root, node => {
                Assert.AreEqual(id++, node.Id);
            });
            Assert.AreEqual(_count, id);
        }

        [TestMethod]
        public void RaiseFixedBackwardTest()
        {
            int id = _count;
            FixedEventReceiver.RaiseDownBackward(_root, node => {
                Assert.AreEqual(--id, node.Id);
            });
            Assert.AreEqual(0, id);
        }

        [TestMethod]
        public void RaiseAndModifyTest()
        {
            var (other, _, _) = TestEventReceiver.CreateData(_count);

            int id = 0;
            FixedEventReceiver.RaiseDownForward(_root, node => {
                Assert.AreEqual(id++, node.Id);
                if (id == _count - 1)
                {
                    foreach (var datum in _data)
                    {
                        if (datum.Children.Remove(_data[id]))
                            break;
                    }
                    _data[id - 1].Children.Add(other);
                    ++id;
                }
            });
            Assert.AreEqual(_count * 2, id);
        }
    }
}
