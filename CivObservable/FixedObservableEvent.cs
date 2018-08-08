using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivObservable
{
    /// <summary>
    /// Represents an event that is both a fixed event and an observable event.
    /// </summary>
    /// <typeparam name="Observer">The type of the observer.</typeparam>
    /// <typeparam name="Priority">
    /// The type of the priority. This must be enumeration type of <see cref="int"/> where all of values are non-negative.
    /// </typeparam>
    /// <typeparam name="FixedReceiver">The type of the fixed event receiver.</typeparam>
    /// <seealso cref="CivObservable.ObservableEvent{Observer, Priority}" />
    /// <seealso cref="FixedEvent{FixedReceiver}"/>
    /// <seealso cref="ObservableEvent{Observer, Priority}"/>
    public class FixedObservableEvent<Observer, Priority, FixedReceiver> : ObservableEvent<Observer, Priority>
        where Observer : class
    //  where Priority : Enum   // C# 7.3
        where Priority : struct, IConvertible
        where FixedReceiver : class
    {
        /// <summary>
        /// Gets this object as <see cref="FixedEvent{FixedReceiver}"/>.
        /// </summary>
        public FixedEvent<FixedReceiver> AsFixedEvent { get; }

        /// <summary>
        /// Gets this object as <see cref="ObservableEvent{Observer, Priority}"/>.
        /// </summary>
        public ObservableEvent<Observer, Priority> AsObservableEvent => this;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedObservableEvent{Observer, Priority, FixedReceiver}"/> class by children supplier.
        /// </summary>
        /// <param name="childrenSupplier">The function that returns children of root in fixed event hierarchy.</param>
        /// <exception cref="ArgumentNullException">childrenSupplier is <c>null</c>.</exception>
        public FixedObservableEvent(Func<IEnumerable<IFixedEventReceiver<FixedReceiver>>> childrenSupplier)
        {
            AsFixedEvent = new FixedEvent<FixedReceiver>(childrenSupplier);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedObservableEvent{Observer, Priority, FixedReceiver}"/> class by a specific root receiver.
        /// </summary>
        /// <param name="fixedRoot">The root receiver in fixed event hierarchy.</param>
        /// <exception cref="ArgumentNullException">fixedRoot is <c>null</c>.</exception>
        public FixedObservableEvent(IFixedEventReceiver<FixedReceiver> fixedRoot)
        {
            AsFixedEvent = new FixedEvent<FixedReceiver>(fixedRoot);
        }

        /// <summary>
        /// Raises the fixed event in the direction of forward DFS.
        /// </summary>
        /// <param name="action">The action to be called with receiver.</param>
        public void RaiseFixedForward(Action<FixedReceiver> action)
        {
            AsFixedEvent.RaiseFixedForward(action);
        }

        /// <summary>
        /// Raises the fixed event in the direction of backward DFS.
        /// </summary>
        /// <param name="action">The action to be called with receiver.</param>
        public void RaiseFixedBackward(Action<FixedReceiver> action)
        {
            AsFixedEvent.RaiseFixedBackward(action);
        }
    }
}
