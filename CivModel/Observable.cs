using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents an object observable by observer interface.
    /// </summary>
    /// <typeparam name="T">The observer interface to receive</typeparam>
    public sealed class Observable<T>
    {
        private List<T> _observerList = new List<T>();
        private List<T> _observerAddList = new List<T>();
        private List<T> _observerRemoveList = new List<T>();

        private int counter = 0;

        /// <summary>
        /// Registers an observer object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <seealso cref="RemoveObserver(T)"/>
        public void AddObserver(T observer)
        {
            if (_observerRemoveList.Contains(observer))
            {
                _observerRemoveList.Remove(observer);
            }
            else if (counter > 0)
            {
                _observerAddList.Add(observer);
            }
            else
            {
                _observerList.Add(observer);
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
            if (!_observerList.Contains(observer) || _observerRemoveList.Contains(observer))
                throw new ArgumentException("observer is not registered", nameof(observer));

            _observerRemoveList.Add(observer);
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
                if (!_observerRemoveList.Contains(obs))
                    action(obs);
            }
            if (--counter == 0)
            {
                _observerList.RemoveAll(obs => _observerRemoveList.Contains(obs));
                _observerList.AddRange(_observerAddList);
                _observerRemoveList.Clear();
                _observerAddList.Clear();
            }
        }
    }
}
