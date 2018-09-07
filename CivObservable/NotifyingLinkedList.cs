using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CivObservable
{
    /// <summary>
    /// Represents a linked list that notifies its modification.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    public class NotifyingLinkedList<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        private readonly LinkedList<T> _list = new LinkedList<T>();
        private readonly Action _notifyHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyingLinkedList{T}"/> class.
        /// </summary>
        /// <param name="notifyHandler">the handler to call on notification.</param>
        public NotifyingLinkedList(Action notifyHandler)
        {
            _notifyHandler = notifyHandler;
        }

        #region as-is implementation
        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1" />에 항목을 추가합니다.
        /// </summary>
        /// <param name="item"><see cref="T:System.Collections.Generic.ICollection`1" />에 추가할 개체입니다.</param>
        public bool Contains(T item) => _list.Contains(item);
        /// <summary>
        /// 특정 <see cref="T:System.Collections.Generic.ICollection`1" /> 인덱스부터 시작하여 <see cref="T:System.Array" />의 요소를 <see cref="T:System.Array" />에 복사합니다.
        /// </summary>
        /// <param name="array"><see cref="T:System.Array" />에서 복사한 요소의 대상인 일차원 <see cref="T:System.Collections.Generic.ICollection`1" />입니다.
        /// <see cref="T:System.Array" />에는 0부터 시작하는 인덱스가 있어야 합니다.</param>
        /// <param name="index"><paramref name="array" />에서 복사가 시작되는 0부터 시작하는 인덱스입니다.</param>
        /// <exception cref="ArgumentNullException">array</exception>
        /// <exception cref="ArgumentOutOfRangeException">arrayIndex - arrayIndex is less than 0</exception>
        /// <exception cref="ArgumentException">The available space from arrayIndex to the end of the destination array is not enough</exception>
        public void CopyTo(T[] array, int index) => _list.CopyTo(array, index);
        /// <summary>
        /// Finds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public LinkedListNode<T> Find(T value) => _list.Find(value);
        /// <summary>
        /// Finds the last.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public LinkedListNode<T> FindLast(T value) => _list.FindLast(value);
        /// <summary>
        /// 컬렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        /// <returns>
        /// 컬렉션을 반복하는 데 사용할 수 있는 열거자입니다.
        /// </returns>
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        /// <summary>
        /// Gets the last.
        /// </summary>
        /// <value>
        /// The last.
        /// </value>
        public LinkedListNode<T> Last => _list.Last;
        /// <summary>
        /// Gets the first.
        /// </summary>
        /// <value>
        /// The first.
        /// </value>
        public LinkedListNode<T> First => _list.First;
        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1" />에 포함된 요소 수를 가져옵니다.
        /// </summary>
        public int Count => _list.Count;
        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1" />가 읽기 전용인지 여부를 나타내는 값을 가져옵니다.
        /// </summary>
        public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

        void ICollection<T>.Add(T item) => AddLast(item);
        IEnumerator IEnumerable.GetEnumerator() => ((ICollection<T>)_list).GetEnumerator();
        #endregion

        #region notifyHandler implementation        
        /// <summary>
        /// Adds the after.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            var rs = _list.AddAfter(node, value);
            _notifyHandler();
            return rs;
        }

        /// <summary>
        /// Adds the after.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="newNode">The new node.</param>
        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            _list.AddAfter(node, newNode);
            _notifyHandler();
        }

        /// <summary>
        /// Adds the before.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            var rs = _list.AddBefore(node, value);
            _notifyHandler();
            return rs;
        }

        /// <summary>
        /// Adds the before.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="newNode">The new node.</param>
        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            _list.AddBefore(node, newNode);
            _notifyHandler();
        }

        /// <summary>
        /// Adds the first.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public LinkedListNode<T> AddFirst(T value)
        {
            var rs = _list.AddFirst(value);
            _notifyHandler();
            return rs;
        }

        /// <summary>
        /// Adds the first.
        /// </summary>
        /// <param name="node">The node.</param>
        public void AddFirst(LinkedListNode<T> node)
        {
            _list.AddFirst(node);
            _notifyHandler();
        }

        /// <summary>
        /// Adds the last.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public LinkedListNode<T> AddLast(T value)
        {
            var rs = _list.AddLast(value);
            _notifyHandler();
            return rs;
        }

        /// <summary>
        /// Adds the last.
        /// </summary>
        /// <param name="node">The node.</param>
        public void AddLast(LinkedListNode<T> node)
        {
            _list.AddLast(node);
            _notifyHandler();
        }

        /// <summary>
        /// <see cref="T:System.Collections.Generic.ICollection`1" />에서 항목을 모두 제거합니다.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
            _notifyHandler();
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool Remove(T value)
        {
            if (_list.Remove(value))
            {
                _notifyHandler();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void Remove(LinkedListNode<T> node)
        {
            _list.Remove(node);
            _notifyHandler();
        }

        /// <summary>
        /// Removes the first.
        /// </summary>
        public void RemoveFirst()
        {
            _list.RemoveFirst();
            _notifyHandler();
        }

        /// <summary>
        /// Removes the last.
        /// </summary>
        public void RemoveLast()
        {
            _list.RemoveLast();
            _notifyHandler();
        }
        #endregion
    }
}
