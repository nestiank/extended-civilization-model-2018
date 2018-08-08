using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivObservable
{
    /// <summary>
    /// The interface represents an object which can receive fixed event.
    /// </summary>
    /// <typeparam name="T">The type of receiver</typeparam>
    public interface IFixedEventReceiver<T> where T : class
    {
        /// <summary>
        /// The children of this receiver, in the hierarchy of fixed event.
        /// </summary>
        IEnumerable<IFixedEventReceiver<T>> Children { get; }

        /// <summary>
        /// The receiver object.
        /// </summary>
        T Receiver { get; }
    }
}
