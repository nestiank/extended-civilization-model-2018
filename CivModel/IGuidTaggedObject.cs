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
    /// To enable object creation from Guid, <see cref="GuidTaggedObjectManager.RegisterGuid(Guid, Func{Player, Terrain.Point, IGuidTaggedObject})"/> must be called.
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
        private Dictionary<Guid, Func<Player, Terrain.Point, IGuidTaggedObject>> _dict = new Dictionary<Guid, Func<Player, Terrain.Point, IGuidTaggedObject>>();

        /// <summary>
        /// Registers a Guid with <see cref="IGuidTaggedObject"/> supplier, which requires <see cref="Player"/> and <see cref="Terrain.Point"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="supplier"/> creates an object from arguments. If arguments are invalid, supplier can return <c>null</c>.
        /// </remarks>
        /// <param name="guid">The Guid.</param>
        /// <param name="supplier">The supplier.</param>
        /// <exception cref="ArgumentNullException"><paramref name="supplier"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The same Guid is already registered.</exception>
        public void RegisterGuid(Guid guid, Func<Player, Terrain.Point, IGuidTaggedObject> supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException("supplier");

            _dict.Add(guid, supplier);
        }

        /// <summary>
        /// Registers a Guid with <see cref="IGuidTaggedObject"/> supplier, which requires <see cref="CityBase"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="supplier"/> creates an object from arguments. If arguments are invalid, supplier can return <c>null</c>.
        /// </remarks>
        /// <param name="guid">The Guid.</param>
        /// <param name="supplier">The supplier.</param>
        /// <exception cref="ArgumentNullException"><paramref name="supplier"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The same Guid is already registered.</exception>
        public void RegisterGuid(Guid guid, Func<CityBase, IGuidTaggedObject> supplier)
        {
            RegisterGuid(guid, (p, t) => {
                if (t.TileBuilding is CityBase city && city.Owner == p)
                    return supplier(city);
                else
                    return null;
            });
        }

        /// <summary>
        /// Creates the <see cref="IGuidTaggedObject"/> from a specified Guid. If arguments are invalid, returns <c>null</c>.
        /// </summary>
        /// <param name="guid">The Guid.</param>
        /// <param name="owner">The <see cref="Player"/> who will own the object.</param>
        /// <param name="point">The tile where the object will be.</param>
        /// <returns>the created <see cref="IGuidTaggedObject"/> object. If arguments are invalid, <c>null</c>.</returns>
        /// <exception cref="KeyNotFoundException">the value of <paramref name="guid"/> is not registered.</exception>
        public IGuidTaggedObject Create(Guid guid, Player owner, Terrain.Point point)
        {
            return _dict[guid](owner, point);
        }
    }
}
