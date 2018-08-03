using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CivObservable.UnitTest
{
    [TestClass]
    public class ObservableTest
    {
        private Observable<string> _observable;

        [TestInitialize]
        public void Initialize()
        {
            _observable = new Observable<string>(3);
        }

        [TestMethod]
        public void InitialTest()
        {
            Assert.AreEqual(3, _observable.CountOfPriority);
            _observable.IterateObserver(obs => Assert.Fail());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddObserverNegativePriorityTest()
        {
            _observable.AddObserver("adf", -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AddObserverTooBigPriorityTest()
        {
            _observable.AddObserver("adf", 9999);
        }

        [TestMethod]
        public void SimpleObserverTest()
        {
            string data = "simple";
            bool visited = false;
            _observable.AddObserver(data, 0);
            _observable.IterateObserver(obs => {
                Assert.AreSame(data, obs);
                Assert.IsFalse(visited);
                visited = true;
            });
            Assert.IsTrue(visited);
            _observable.RemoveObserver(data);
            Assert.ThrowsException<ArgumentException>(() => _observable.RemoveObserver("simple"));
        }

        [TestMethod]
        public void PriorityTest()
        {
            string[] data = { "primary", "secondary", "tertiary" };
            int idx = 0;

            _observable.AddObserver(data[0], 0);
            _observable.AddObserver(data[1], 1);
            _observable.AddObserver(data[2], 2);
            _observable.IterateObserver(obs => Assert.AreSame(data[idx++], obs));
            Assert.AreEqual(data.Length, idx);
        }

        [TestMethod]
        public void ObserverEqualityTest()
        {
            string a = "equality";
            string b = new string(a.ToCharArray());
            object[] data = { a, b };
            int idx = 0;

            _observable.AddObserver(a, 0);
            _observable.AddObserver(b, 0);
            Assert.ThrowsException<ArgumentException>(() => _observable.AddObserver(a, 0));

            _observable.IterateObserver(obs => Assert.AreSame(data[idx++], obs));
            Assert.AreEqual(2, idx);
        }

        [TestMethod]
        public void NestingTest()
        {
            string[] data = { "primary", "secondary", "tartiary" };
            int idx1 = 0, idx2 = 0;

            _observable.AddObserver(data[0], 0);
            _observable.AddObserver(data[1], 0);
            _observable.IterateObserver(obs1 => {
                if (idx1 == 0)
                {
                    _observable.AddObserver(data[2], 0);
                    idx2 = 0;
                }
                else if (idx1 == 1)
                {
                    _observable.RemoveObserver(data[0]);
                    idx2 = 1;
                }

                Assert.AreSame(data[idx1++], obs1);
                _observable.IterateObserver(obs2 => {
                    Assert.AreSame(data[idx2++], obs2);
                });
                Assert.AreEqual(2, idx2);
            });

            Assert.AreEqual(2, idx1);

            idx1 = 1;
            _observable.IterateObserver(obs1 => {
                Assert.AreSame(data[idx1++], obs1);
            });

            Assert.AreEqual(3, idx1);
        }
    }
}
