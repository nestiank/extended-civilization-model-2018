using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public partial class Game
    {
        private List<ITurnObserver> _turnObserverList = new List<ITurnObserver>();
        private List<ITurnObserver> _turnObserverRemoveList = new List<ITurnObserver>();

        private List<ITileObjectObserver> _tileObjectObserverList = new List<ITileObjectObserver>();
        private List<ITileObjectObserver> _tileObjectObserverRemoveList = new List<ITileObjectObserver>();

        private List<IBattleObserver> _battleObserverList = new List<IBattleObserver>();
        private List<IBattleObserver> _battleObserverRemoveList = new List<IBattleObserver>();

        /// <summary>
        /// Registers an <see cref="ITurnObserver"/> object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <seealso cref="RemoveTurnObserver(ITurnObserver)"/>
        /// <seealso cref="ITurnObserver"/>
        public void AddTurnObserver(ITurnObserver observer)
        {
            _turnObserverList.Add(observer);
        }

        /// <summary>
        /// Removes a registered <see cref="ITurnObserver"/> object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <exception cref="ArgumentException"><paramref name="observer"/> is not registered</exception>
        /// <seealso cref="AddTurnObserver(ITurnObserver)"/>
        /// <seealso cref="ITurnObserver"/>
        public void RemoveTurnObserver(ITurnObserver observer)
        {
            if (!_turnObserverList.Contains(observer) || _turnObserverRemoveList.Contains(observer))
                throw new ArgumentException("observer is not registered", nameof(observer));

            _turnObserverRemoveList.Add(observer);
        }

        private void IterateTurnObserver(Action<ITurnObserver> action)
        {
            foreach (var obj in _turnObserverList)
            {
                if (!_turnObserverRemoveList.Contains(obj))
                    action(obj);
            }
            _turnObserverList.RemoveAll(obj => _turnObserverRemoveList.Contains(obj));
        }

        /// <summary>
        /// Registers an <see cref="ITileObjectObserver"/> object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <seealso cref="RemoveTileObjectObserver(ITileObjectObserver)"/>
        /// <seealso cref="ITileObjectObserver"/>
        public void AddTileObjectObserver(ITileObjectObserver observer)
        {
            _tileObjectObserverList.Add(observer);
        }

        /// <summary>
        /// Removes a registered <see cref="ITileObjectObserver"/> object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <exception cref="ArgumentException"><paramref name="observer"/> is not registered</exception>
        /// <seealso cref="AddTileObjectObserver(ITileObjectObserver)"/>
        /// <seealso cref="ITileObjectObserver"/>
        public void RemoveTileObjectObserver(ITileObjectObserver observer)
        {
            if (!_tileObjectObserverList.Contains(observer) || _tileObjectObserverRemoveList.Contains(observer))
                throw new ArgumentException("observer is not registered", nameof(observer));

            _tileObjectObserverRemoveList.Add(observer);
        }

        // this method is used by TileObject
        internal void IterateTileObjectObserver(Action<ITileObjectObserver> action)
        {
            foreach (var obj in _tileObjectObserverList)
            {
                if (!_tileObjectObserverRemoveList.Contains(obj))
                    action(obj);
            }
            _tileObjectObserverList.RemoveAll(obj => _tileObjectObserverRemoveList.Contains(obj));
        }

        /// <summary>
        /// Registers an <see cref="IBattleObserver"/> object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <seealso cref="RemoveBattleObserver(IBattleObserver)"/>
        /// <seealso cref="IBattleObserver"/>
        public void AddBattleObserver(IBattleObserver observer)
        {
            _battleObserverList.Add(observer);
        }

        /// <summary>
        /// Removes a registered <see cref="IBattleObserver"/> object.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <exception cref="ArgumentException"><paramref name="observer"/> is not registered</exception>
        /// <seealso cref="AddBattleObserver(IBattleObserver)"/>
        /// <seealso cref="IBattleObserver"/>
        public void RemoveBattleObserver(IBattleObserver observer)
        {
            if (!_battleObserverList.Contains(observer) || _battleObserverRemoveList.Contains(observer))
                throw new ArgumentException("observer is not registered", nameof(observer));

            _battleObserverRemoveList.Add(observer);
        }

        // this method is used by Actor
        internal void IterateBattleObserver(Action<IBattleObserver> action)
        {
            foreach (var obj in _battleObserverList)
            {
                if (!_battleObserverRemoveList.Contains(obj))
                    action(obj);
            }
            _battleObserverList.RemoveAll(obj => _battleObserverRemoveList.Contains(obj));
        }
    }
}
