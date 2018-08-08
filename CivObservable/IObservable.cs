using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivObservable
{
    /// <summary>
    /// The interface represents an object observable by observer interface.
    /// </summary>
    /// <typeparam name="Observer">The type of observer.</typeparam>
    /// <typeparam name="Priority">The type of priority. This must be convertible to <see cref="int"/>.</typeparam>
    public interface IObservable<Observer, Priority> where Priority : IConvertible
    {
        /// <summary>
        /// Registers an observer object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <param name="priority">The priority of the observer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="observer"/> is <c>null</c></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="priority"/> is invalid</exception>
        /// <exception cref="ArgumentException"><paramref name="observer"/> is already registered</exception>
        /// <seealso cref="RemoveObserver(Observer)"/>
        void AddObserver(Observer observer, Priority priority);

        /// <summary>
        /// Removes a registered observer object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <exception cref="ArgumentException">observer is not registered</exception>
        /// <seealso cref="AddObserver(Observer, Priority)"/>
        void RemoveObserver(Observer observer);
    }
}
