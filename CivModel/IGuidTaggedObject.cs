using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The interface represents an object which can be created and class-level distinguished from <see cref="System.Guid"/>.
    /// </summary>
    /// <remarks>
    /// To enable object creation from Guid, <see cref="GuidTaggedObjectManager.RegisterGuid(Guid, Func{Player, IGuidTaggedObject})"/> must be called.
    /// </remarks>
    /// <seealso cref="GuidTaggedObjectManager"/>
    public interface IGuidTaggedObject
    {
        /// <summary>
        /// The unique identifier of this class.
        /// </summary>
        Guid Guid { get; }
    }

    /// <summary>
    /// Provides object creation of <see cref="IGuidTaggedObject"/> from Guid
    /// </summary>
    /// <seealso cref="IGuidTaggedObject"/>
    public class GuidTaggedObjectManager
    {
        private Dictionary<Guid, Func<Player, IGuidTaggedObject>> _dict = new Dictionary<Guid, Func<Player, IGuidTaggedObject>>();

        /// <summary>
        /// Registers a Guid with <see cref="IGuidTaggedObject"/> supplier.
        /// </summary>
        /// <param name="guid">The Guid.</param>
        /// <param name="supplier">The supplier.</param>
        /// <exception cref="ArgumentNullException"><paramref name="supplier"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The same Guid is already registered.</exception>
        public void RegisterGuid(Guid guid, Func<Player, IGuidTaggedObject> supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException("supplier");

            _dict.Add(guid, supplier);
        }

        /// <summary>
        /// Creates the <see cref="IGuidTaggedObject"/> from a specified Guid.
        /// </summary>
        /// <param name="guid">The Guid.</param>
        /// <param name="owner">The <see cref="Player"/> who will own the object.</param>
        /// <returns>the created <see cref="IGuidTaggedObject"/> object.</returns>
        /// <exception cref="KeyNotFoundException">the value of <paramref name="guid"/> is not registered.</exception>
        public IGuidTaggedObject Create(Guid guid, Player owner)
        {
            return _dict[guid](owner);
        }
    }
}
