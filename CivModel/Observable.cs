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
    /// <typeparam name="T"></typeparam>
    public class Observable<T>
    {
        private List<T> _observerList = new List<T>();
        private List<T> _observerRemoveList = new List<T>();

        /// <summary>
        /// Registers an observer object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <seealso cref="M:CivModel.IObservable`1.RemoveObserver(`0)" />
        public void AddObserver(T observer)
        {
            _observerList.Add(observer);
        }

        /// <summary>
        /// Removes a registered observer object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <exception cref="ArgumentException">observer is not registered</exception>
        /// <seealso cref="M:CivModel.IObservable`1.AddObserver(`0)" />
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
            foreach (var obs in _observerList)
            {
                if (!_observerRemoveList.Contains(obs))
                    action(obs);
            }
            _observerList.RemoveAll(obs => _observerRemoveList.Contains(obs));
        }
    }
}
