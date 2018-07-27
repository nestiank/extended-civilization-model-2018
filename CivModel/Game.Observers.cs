using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    partial class Game
    {
        /// <summary>
        /// An <see cref="Observable{ITurnObserver}"/> object which can be observed by <see cref="ITurnObserver"/>.
        /// </summary>
        public Observable<ITurnObserver> TurnObservable { get; private set; }

        /// <summary>
        /// An <see cref="Observable{IProductionObserver}"/> object which can be observed by <see cref="IProductionObserver"/>.
        /// </summary>
        public Observable<IProductionObserver> ProductionObservable { get; private set; }

        /// <summary>
        /// An <see cref="Observable{ITileObjectObserver}"/> object which can be observed by <see cref="ITileObjectObserver"/>.
        /// </summary>
        public Observable<ITileObjectObserver> TileObjectObservable { get; private set; }

        /// <summary>
        /// An <see cref="Observable{IBattleObserver}"/> object which can be observed by <see cref="IBattleObserver"/>.
        /// </summary>
        public Observable<IBattleObserver> BattleObservable { get; private set; }

        /// <summary>
        /// An <see cref="Observable{IQuestObserver}"/> object which can be observed by <see cref="IQuestObserver"/>.
        /// </summary>
        public Observable<IQuestObserver> QuestObservable { get; private set; }

        /// <summary>
        /// An <see cref="Observable{IVictoryObserver}"/> object which can be observed by <see cref="IVictoryObserver"/>.
        /// </summary>
        public Observable<IVictoryObserver> VictoryObservable { get; private set; }

        private void InitializeObservable()
        {
            TurnObservable = new Observable<ITurnObserver>();
            ProductionObservable = new Observable<IProductionObserver>();
            TileObjectObservable = new Observable<ITileObjectObserver>();
            BattleObservable = new Observable<IBattleObserver>();
            QuestObservable = new Observable<IQuestObserver>();
            VictoryObservable = new Observable<IVictoryObserver>();
        }
    }
}
