using System;
using System.Collections.Generic;
using System.Linq;

namespace CivObservable
{
    /// <summary>
    /// Represents an object observable by observer interface.
    /// </summary>
    /// <typeparam name="T">The observer interface to receive</typeparam>
    /// <remarks>
    /// <para>
    /// The observable object from which observers can be modified during event is raised.
    /// </para>
    /// <para>
    /// If observer is removed during event, the observer list is immediately affected.<br/>
    /// If observer is added during event, the effect on the observer list is delayed until event raising is finished,
    /// that is, the added observer is not called during current event but next event.<br/>
    /// However, the added observer can be safely removed during the same event.
    /// </para>
    /// <para>
    /// The same observer cannot be registered on the same observable twice or more.
    /// Equality of observer is checked by <see cref="object.ReferenceEquals(object, object)"/>.
    /// </para>
    /// </remarks>
    public sealed class Observable<T> where T : class
    {
        private struct PriorityPair
        {
            public T observer;
            public int priority;
            public static PriorityPair Create(T obs, int prior)
            {
                return new PriorityPair { observer = obs, priority = prior };
            }
        }

        private List<T>[] _observerList;
        private List<PriorityPair> _observerAddList = new List<PriorityPair>();

        private int _countOfEnumerator = 0;

        /// <summary>
        /// The count of priorities of an observer to this observable.
        /// </summary>
        public int CountOfPriority { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Observable{T}"/> class.
        /// </summary>
        /// <param name="countOfPriority">The count of priorities of an observer to this observable.</param>
        public Observable(int countOfPriority)
        {
            CountOfPriority = countOfPriority;

            _observerList = new List<T>[countOfPriority];
            for (int i = 0; i < countOfPriority; ++i)
                _observerList[i] = new List<T>();
        }

        /// <summary>
        /// Registers an observer object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <param name="priority">The priority of the observer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="observer"/> is <c>null</c></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="priority"/> is invalid</exception>
        /// <exception cref="ArgumentException"><paramref name="observer"/> is already registered</exception>
        /// <seealso cref="RemoveObserver(T)"/>
        public void AddObserver(T observer, int priority)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));
            if (priority < 0 || priority >= CountOfPriority)
                throw new ArgumentOutOfRangeException("priority is invalid");

            if (_observerList.Any(list => list.Any(obs => object.ReferenceEquals(obs, observer)))
                || _observerAddList.Any(obs => object.ReferenceEquals(obs, observer)))
            {
                throw new ArgumentException("observer is already registered");
            }

            if (_countOfEnumerator > 0)
            {
                _observerAddList.Add(PriorityPair.Create(observer, priority));
            }
            else
            {
                _observerList[priority].Add(observer);
            }
        }

        /// <summary>
        /// Removes a registered observer object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <exception cref="ArgumentException">observer is not registered</exception>
        /// <seealso cref="AddObserver(T, int)"/>
        public void RemoveObserver(T observer)
        {
            int idx = _observerAddList.FindLastIndex(o => object.ReferenceEquals(o, observer));
            if (idx != -1)
            {
                _observerAddList.RemoveAt(idx);
            }
            else
            {
                foreach (var list in _observerList)
                {
                    idx = list.FindLastIndex(o => object.ReferenceEquals(o, observer));
                    if (idx != -1)
                    {
                        if (_countOfEnumerator == 0)
                            list.RemoveAt(idx);
                        else
                            list[idx] = null;

                        return;
                    }
                }

                throw new ArgumentException("observer is not registered", nameof(observer));
            }
        }

        /// <summary>
        /// Iterates through the registered observers.
        /// </summary>
        /// <param name="action">The action to do in iteration.</param>
        public void IterateObserver(Action<T> action)
        {
            ++_countOfEnumerator;

            foreach (var list in _observerList)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i] != null)
                        action(list[i]);
                }
            }

            if (--_countOfEnumerator == 0)
            {
                foreach (var list in _observerList)
                {
                    list.RemoveAll(obs => obs == null);
                }

                foreach (var pair in _observerAddList)
                {
                    _observerList[pair.priority].Add(pair.observer);
                }
                _observerAddList.Clear();
            }
        }
    }
}
