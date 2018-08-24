using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    class SpecialResourceDictionary : IDictionary<ISpecialResource, int>
    {
        private Player _player;

        private Dictionary<ISpecialResource, (object data, int count)> _specialResources
            = new Dictionary<ISpecialResource, (object, int)>();

        internal SpecialResourceDictionary(Player player)
        {
            _player = player;
        }

        public int this[ISpecialResource resource]
        {
            get
            {
                if (_specialResources.TryGetValue(resource, out var x))
                    return x.count;
                else
                    return 0;
            }
            set
            {
                if (value < 0 || value > resource.MaxCount)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "the amount of resource is out of range");

                if (_specialResources.TryGetValue(resource, out var x))
                {
                    _specialResources[resource] = (x.data, value);
                }
                else
                {
                    _specialResources[resource] = (null, value);
                    var data = resource.EnablePlayer(_player);
                    _specialResources[resource] = (data, value);
                }
            }
        }

        public int Count => _specialResources.Count;

        public bool IsReadOnly => false;

        public void Add(ISpecialResource key, int value)
        {
            this[key] = value;
        }

        public void Add(KeyValuePair<ISpecialResource, int> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            foreach (var key in _specialResources.Keys)
                this[key] = 0;
        }

        public bool Contains(KeyValuePair<ISpecialResource, int> item)
        {
            return ContainsKey(item.Key) && this[item.Key] == item.Value;
        }

        public bool ContainsKey(ISpecialResource key)
        {
            return _specialResources.ContainsKey(key);
        }

        public bool Remove(ISpecialResource key)
        {
            if (ContainsKey(key))
            {
                this[key] = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<ISpecialResource, int> item)
        {
            if (Contains(item))
            {
                this[item.Key] = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGetValue(ISpecialResource key, out int value)
        {
            value = this[key];
            return true;
        }

        public void CopyTo(KeyValuePair<ISpecialResource, int>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex is less than 0.", nameof(arrayIndex));
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("The number of elements in the source ICollection<T> is greater than the available space from arrayIndex to the end of the destination array.", nameof(array));

            foreach (var pr in _specialResources)
                array[arrayIndex++] = new KeyValuePair<ISpecialResource, int>(pr.Key, pr.Value.count);
        }

        public IEnumerator<KeyValuePair<ISpecialResource, int>> GetEnumerator()
        {
            foreach (var pr in _specialResources)
                yield return new KeyValuePair<ISpecialResource, int>(pr.Key, pr.Value.count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ICollection<ISpecialResource> Keys => _specialResources.Keys;

        public ICollection<int> Values => new ValuesCollection { _dict = _specialResources };
        private class ValuesCollection : ICollection<int>
        {
            public Dictionary<ISpecialResource, (object data, int count)> _dict;

            public int Count => _dict.Values.Count;
            public bool IsReadOnly => true;

            public void Add(int item)
                => throw new NotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");
            public void Clear()
                => throw new NotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");
            public bool Remove(int item)
                => throw new NotSupportedException("Mutating a key collection derived from a dictionary is not allowed.");

            public bool Contains(int item)
                => _dict.Values.Any(x => x.count == item);

            public void CopyTo(int[] array, int arrayIndex)
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (arrayIndex < 0)
                    throw new ArgumentOutOfRangeException("arrayIndex is less than 0.", nameof(arrayIndex));
                if (array.Length - arrayIndex < Count)
                    throw new ArgumentException("The number of elements in the source ICollection<T> is greater than the available space from arrayIndex to the end of the destination array.", nameof(array));

                foreach (var (data, count) in _dict.Values)
                    array[arrayIndex++] = count;
            }

            public IEnumerator<int> GetEnumerator()
            {
                foreach (var (data, count) in _dict.Values)
                    yield return count;
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
