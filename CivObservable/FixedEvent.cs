using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivObservable
{
    /// <summary>
    /// Represents a fixed event.
    /// </summary>
    /// <typeparam name="FixedReceiver">The type of the fixed event receiver.</typeparam>
    /// <seealso cref="IFixedEventReceiver{T}"/>
    /// <seealso cref="ObservableEvent{Observer, Priority}"/>
    /// <seealso cref="FixedObservableEvent{Observer, Priority, FixedReceiver}"/>
    public class FixedEvent<FixedReceiver> where FixedReceiver : class
    {
        private class DummyRoot : IFixedEventReceiver<FixedReceiver>
        {
            public Func<IEnumerable<IFixedEventReceiver<FixedReceiver>>> ChildrenSupplier;

            public IEnumerable<IFixedEventReceiver<FixedReceiver>> Children => ChildrenSupplier();
            public FixedReceiver Receiver => null;
        }

        private IFixedEventReceiver<FixedReceiver> _fixedRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedEvent{FixedReceiver}"/> class by children supplier.
        /// </summary>
        /// <param name="childrenSupplier">The function that returns children of root in fixed event hierarchy.</param>
        /// <exception cref="ArgumentNullException">childrenSupplier is <c>null</c>.</exception>
        public FixedEvent(Func<IEnumerable<IFixedEventReceiver<FixedReceiver>>> childrenSupplier)
        {
            if (childrenSupplier == null)
                throw new ArgumentNullException(nameof(childrenSupplier));

            _fixedRoot = new DummyRoot { ChildrenSupplier = childrenSupplier };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedEvent{FixedReceiver}"/> class by a specific root receiver.
        /// </summary>
        /// <param name="fixedRoot">The root receiver in fixed event hierarchy.</param>
        /// <exception cref="ArgumentNullException">fixedRoot is <c>null</c>.</exception>
        public FixedEvent(IFixedEventReceiver<FixedReceiver> fixedRoot)
        {
            _fixedRoot = fixedRoot ?? throw new ArgumentNullException(nameof(fixedRoot));
        }

        /// <summary>
        /// Raises the fixed event in the direction of forward DFS.
        /// </summary>
        /// <param name="action">The action to be called with receiver.</param>
        public void RaiseFixedForward(Action<FixedReceiver> action)
        {
            FixedEventReceiver.RaiseDownForward(_fixedRoot, action);
        }

        /// <summary>
        /// Raises the fixed event in the direction of backward DFS.
        /// </summary>
        /// <param name="action">The action to be called with receiver.</param>
        public void RaiseFixedBackward(Action<FixedReceiver> action)
        {
            FixedEventReceiver.RaiseDownBackward(_fixedRoot, action);
        }
    }
}
