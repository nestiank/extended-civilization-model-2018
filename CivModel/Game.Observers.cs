using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public partial class Game
    {
        /// <summary>
        /// An <see cref="Observable{ITurnObserver}"/> which can be observed by <see cref="ITurnObserver"/>.
        /// </summary>
        public Observable<ITurnObserver> TurnObservable { get; private set; }

        /// <summary>
        /// An <see cref="Observable{ITileObjectObserver}"/> which can be observed by <see cref="ITileObjectObserver"/>.
        /// </summary>
        public Observable<ITileObjectObserver> TileObjectObservable { get; private set; }

        /// <summary>
        /// An <see cref="Observable{IBattleObserver}"/> which can be observed by <see cref="IBattleObserver"/>.
        /// </summary>
        public Observable<IBattleObserver> BattleObservable { get; private set; }

        private void InitializeObservable()
        {
            TurnObservable = new Observable<ITurnObserver>();
            TileObjectObservable = new Observable<ITileObjectObserver>();
            BattleObservable = new Observable<IBattleObserver>();
        }
    }
}
