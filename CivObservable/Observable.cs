using System;
using System.Collections.Generic;
using System.Linq;

namespace CivObservable
{
    /// <summary>
    /// Represents an object observable by observer interface.
    /// </summary>
    /// <typeparam name="T">The observer interface to receive</typeparam>
    public sealed class Observable<T>
    {
        private struct Observer
        {
            public T value;
            public int refcount;
            public void IncRef()
            {
                ++refcount;
            }
            public void DecRef()
            {
                --refcount;
            }
        }

        private List<Observer> _observerList = new List<Observer>();
        private List<Observer> _observerAddList = new List<Observer>();

        private int counter = 0;

        /// <summary>
        /// Registers an observer object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <seealso cref="RemoveObserver(T)"/>
        public void AddObserver(T observer)
        {
            if (counter > 0)
            {
                AddToList(_observerAddList, observer);
            }
            else
            {
                AddToList(_observerList, observer);
            }
        }

        /// <summary>
        /// Removes a registered observer object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <exception cref="ArgumentException">observer is not registered</exception>
        /// <seealso cref="AddObserver(T)"/>
        public void RemoveObserver(T observer)
        {
            int idx = _observerAddList.FindIndex(o => object.ReferenceEquals(o.value, observer));
            if (idx != -1 && _observerAddList[idx].refcount > 0)
            {
                _observerAddList[idx].DecRef();
            }
            else
            {
                idx = _observerList.FindIndex(o => object.ReferenceEquals(o.value, observer));
                if (idx == -1 || _observerList[idx].refcount <= 0)
                    throw new ArgumentException("observer is not registered", nameof(observer));

                _observerList[idx].DecRef();
            }
        }

        /// <summary>
        /// Iterates through the registered observers.
        /// </summary>
        /// <param name="action">The action to do in iteration.</param>
        public void IterateObserver(Action<T> action)
        {
            ++counter;
            foreach (var obs in _observerList)
            {
                if (obs.refcount > 0)
                    action(obs.value);
            }
            if (--counter == 0)
            {
                _observerList.RemoveAll(obs => obs.refcount <= 0);
                _observerList.AddRange(_observerAddList.Where(obs => obs.refcount > 0));
                _observerAddList.Clear();
            }
        }

        private static void AddToList(List<Observer> list, T val)
        {
            int idx = list.FindIndex(o => object.ReferenceEquals(o.value, val));
            if (idx != -1)
            {
                list[idx].IncRef();
            }
            else
            {
                list.Add(new Observer { value = val, refcount = 1 });
            }
        }
    }
}
