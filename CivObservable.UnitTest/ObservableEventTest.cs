using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CivObservable.UnitTest
{
    [TestClass]
    public class ObservableEventTest
    {
        private enum Priority { High = 0, Low = 1 }
        private ObservableEvent<object, Priority> _event;

        [TestInitialize]
        public void Initialize()
        {
            _event = new ObservableEvent<object, Priority>();
        }

        [TestMethod]
        public void InitialTest()
        {
            _event.RaiseObservable(o => Assert.Fail());
        }

        [TestMethod]
        public void RaiseTest()
        {
            object[] data = { "a", "b", "c" };
            _event.AddObserver(data[1], Priority.Low);
            _event.AddObserver(data[2], Priority.Low);
            _event.AddObserver(data[0], Priority.High);

            int idx = 0;
            _event.RaiseObservable(o => Assert.AreSame(data[idx++], o));
            Assert.AreEqual(3, idx);

            idx = 0;
            _event.RemoveObserver(data[2]);
            _event.RaiseObservable(o => Assert.AreSame(data[idx++], o));
            Assert.AreEqual(2, idx);

            _event.RemoveObserver(data[1]);
            _event.RemoveObserver(data[0]);
            _event.RaiseObservable(o => Assert.Fail());
        }
    }
}
