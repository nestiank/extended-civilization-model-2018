using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivObservable
{
    /// <summary>
    /// Represents an observable event.
    /// </summary>
    /// <typeparam name="Observer">The type of the observer.</typeparam>
    /// <typeparam name="Priority">
    /// The type of the priority. This must be enumeration type of <see cref="int"/> where all of values are non-negative.
    /// </typeparam>
    /// <seealso cref="CivObservable.IObservable{Observer, Priority}" />
    /// <seealso cref="FixedEvent{FixedReceiver}"/>
    /// <seealso cref="FixedObservableEvent{Observer, Priority, FixedReceiver}"/>
    public class ObservableEvent<Observer, Priority> : IObservable<Observer, Priority>
        where Observer : class
    //  where Priority : Enum   // C# 7.3
        where Priority : struct, IConvertible
    {
        private readonly Observable<Observer> _observable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableEvent{Observer, Priority}"/> class.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Priority is not an enumeration type of int
        /// or
        /// one of values of Priority enumeration is negative
        /// </exception>
        public ObservableEvent()
        {
            try
            {
                var ar = (int[])Enum.GetValues(typeof(Priority));
                if (ar.Any(x => x < 0))
                    throw new InvalidOperationException("one of values of Priority enumeration is negative");

                _observable = new Observable<Observer>(ar.Max() + 1);
            }
            catch (Exception e) when (e is ArgumentException || e is InvalidCastException)
            {
                throw new InvalidOperationException("Priority is not an enumeration type of int", e);
            }
        }

        /// <summary>
        /// Registers an observer object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <param name="priority">The priority of the observer.</param>
        /// <seealso cref="M:CivObservable.IObservable`2.RemoveObserver(`0)" />
        public void AddObserver(Observer observer, Priority priority)
        {
            _observable.AddObserver(observer, Convert.ToInt32(priority));
        }

        /// <summary>
        /// Removes a registered observer object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <seealso cref="M:CivObservable.IObservable`2.AddObserver(`0,`1)" />
        public void RemoveObserver(Observer observer)
        {
            _observable.RemoveObserver(observer);
        }

        /// <summary>
        /// Raises the observable event.
        /// </summary>
        /// <param name="action">The action to be called with observer.</param>
        public void RaiseObservable(Action<Observer> action)
        {
            _observable.IterateObserver(action);
        }
    }
}
