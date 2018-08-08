using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace CivObservable
{
    /// <summary>
    /// Represents a collection can be modified safely during <see langword="foreach"/> iteration.
    /// </summary>
    /// <remarks>
    /// This class is not thread safe.
    /// </remarks>
    /// <typeparam name="T">The type of elements in the collection</typeparam>
    /// <seealso cref="System.Collections.Generic.ICollection{T}" />
    /// <seealso cref="System.Collections.Generic.IReadOnlyCollection{T}" />
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebuggerView<>))]
    public class SafeIterationList<T> : ICollection<T>, IReadOnlyList<T>
    {
        private readonly List<T> _list = new List<T>();
        private readonly List<T> _removeList = new List<T>();

        private int _countOfEnumerator = 0;

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1" />에 포함된 요소 수를 가져옵니다.
        /// </summary>
        public int Count => _list.Count - _removeList.Count;

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1" />가 읽기 전용인지 여부를 나타내는 값을 가져옵니다.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// 이 <see cref="SafeIterationList{T}"/> 개체에 대한 작업이 진행중인지 여부를 나타내는 값을 가져옵니다.
        /// </summary>
        /// <remarks>
        /// 이 값이 <c>true</c>이면 이 객체에 대한 작업이 순회 작업과 동시에 일어날 수 있도록 처리됩니다.
        /// </remarks>
        /// <seealso cref="GetEnumerator"/>
        public bool IsWorking => _countOfEnumerator > 0;

        /// <summary>
        /// 읽기 전용 목록에서 지정된 인덱스의 요소를 가져옵니다.
        /// </summary>
        /// <param name="index">가져올 요소의 0부터 시작하는 인덱스입니다.</param>
        /// <returns>읽기 전용 목록에서 지정된 인덱스의 요소입니다.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0
        /// or
        /// <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
        /// </exception>
        public T this[int index]
        {
            get
            {
                if (!IsWorking)
                {
                    return _list[index];
                }
                else
                {
                    if (index < 0)
                        throw new ArgumentOutOfRangeException(nameof(index), index, "index is less than 0");

                    foreach (var item in this)
                    {
                        if (index-- == 0)
                            return item;
                    }

                    throw new ArgumentOutOfRangeException(nameof(index), index, "index is equal to or greater than Count.");
                }
            }
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1" />에 항목을 추가합니다.
        /// </summary>
        /// <param name="item"><see cref="T:System.Collections.Generic.ICollection`1" />에 추가할 개체입니다.</param>
        public void Add(T item)
        {
            if (_removeList.Contains(item))
            {
                _removeList.Remove(item);
            }
            else
            {
                _list.Add(item);
            }
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1" />에서 맨 처음 발견되는 특정 개체를 제거합니다.
        /// </summary>
        /// <param name="item"><see cref="T:System.Collections.Generic.ICollection`1" />에서 제거할 개체입니다.</param>
        /// <returns>
        /// <see langword="true" />이 <paramref name="item" />에서 제거되면 <see cref="T:System.Collections.Generic.ICollection`1" />이고, 그렇지 않으면 <see langword="false" />입니다.
        /// 이 메서드는 <see langword="false" />이 원래 <paramref name="item" />에 없는 경우에도 <see cref="T:System.Collections.Generic.ICollection`1" />를 반환합니다.
        /// </returns>
        public bool Remove(T item)
        {
            if (!IsWorking)
            {
                return _list.Remove(item);
            }
            else if (Contains(item))
            {
                _removeList.Add(item);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1" />에서 항목을 모두 제거합니다.
        /// </summary>
        public void Clear()
        {
            if (!IsWorking)
            {
                _list.Clear();
            }
            else
            {
                int prevCount = _removeList.Count;
                foreach (var item in _list)
                {
                    if (_removeList.IndexOf(item, 0, prevCount) == -1)
                        _removeList.Add(item);
                }
            }
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1" />에 특정 값이 들어 있는지 여부를 확인합니다.
        /// </summary>
        /// <param name="item"><see cref="T:System.Collections.Generic.ICollection`1" />에서 찾을 개체입니다.</param>
        /// <returns>
        /// <see langword="true" />가 <paramref name="item" />에 있으면 <see cref="T:System.Collections.Generic.ICollection`1" />이고, 그렇지 않으면 <see langword="false" />입니다.
        /// </returns>
        public bool Contains(T item)
        {
            return _list.Contains(item) && !_removeList.Contains(item);
        }

        /// <summary>
        /// 컬렉션을 반복하는 열거자를 반환합니다. 열거하는 동안 <see cref="IsWorking"/>은 <c>true</c>가 됩니다.
        /// </summary>
        /// <returns>
        /// 컬렉션을 반복하는 데 사용할 수 있는 열거자입니다.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            ++_countOfEnumerator;
            try
            {
                for (int i = 0; i < _list.Count; ++i)
                {
                    var item = _list[i];
                    if (!_removeList.Contains(item))
                        yield return item;
                }
            }
            finally
            {
                --_countOfEnumerator;
                if (!IsWorking)
                {
                    _list.RemoveAll(item => _removeList.Contains(item));
                    _removeList.Clear();
                }
            }
        }

        /// <summary>
        /// 시퀀스의 요소 순서를 반전합니다.
        /// </summary>
        /// <returns>시퀀스의 요소 순서를 뒤집은 시퀀스입니다.</returns>
        public IEnumerable<T> Reverse()
        {
            ++_countOfEnumerator;
            try
            {
                for (int i = _list.Count - 1; i >= 0; --i)
                {
                    var item = _list[i];
                    if (!_removeList.Contains(item))
                        yield return item;
                }
            }
            finally
            {
                --_countOfEnumerator;
                if (!IsWorking)
                {
                    _list.RemoveAll(item => _removeList.Contains(item));
                    _removeList.Clear();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 특정 <see cref="T:System.Collections.Generic.ICollection`1" /> 인덱스부터 시작하여 <see cref="T:System.Array" />의 요소를 <see cref="T:System.Array" />에 복사합니다.
        /// </summary>
        /// <param name="array"><see cref="T:System.Array" />에서 복사한 요소의 대상인 일차원 <see cref="T:System.Collections.Generic.ICollection`1" />입니다.
        /// <see cref="T:System.Array" />에는 0부터 시작하는 인덱스가 있어야 합니다.</param>
        /// <param name="arrayIndex"><paramref name="array" />에서 복사가 시작되는 0부터 시작하는 인덱스입니다.</param>
        /// <exception cref="ArgumentNullException">array</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex - arrayIndex is less than 0</exception>
        /// <exception cref="ArgumentException">The available space from arrayIndex to the end of the destination array is not enough</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (!IsWorking)
            {
                _list.CopyTo(array, arrayIndex);
            }
            else
            {
                if (array == null)
                    throw new ArgumentNullException(nameof(array));
                if (arrayIndex < 0)
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "arrayIndex is less than 0");
                if (arrayIndex + Count >= array.Length)
                    throw new ArgumentException("The available space from arrayIndex to the end of the destination array is not enough");

                foreach (var item in this)
                    array[arrayIndex++] = item;
            }
        }
    }

    class CollectionDebuggerView<T>
    {
        private readonly ICollection<T> _collection;

        public CollectionDebuggerView(ICollection<T> collection)
        {
            _collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => System.Linq.Enumerable.ToArray(_collection);
    }
}
