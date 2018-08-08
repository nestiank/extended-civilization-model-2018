using System.Linq;
using CivObservable;

namespace CivModel
{
    /// <summary>
    /// The enumeration represents priority of an observer to an observable event.
    /// </summary>
    public enum ObserverPriority
    {
        /// <summary>
        /// The priority of an observer of model extension module.
        /// </summary>
        Model = 0,
        /// <summary>
        /// The priority of an observer of view module.
        /// </summary>
        View = 1,
    }

    partial class Game
    {
        /// <summary>
        /// An <see cref="IObservable{ITurnObserver, ObserverPriority}"/> object which can be observed by <see cref="ITurnObserver"/>.
        /// </summary>
        public IObservable<ITurnObserver, ObserverPriority> TurnObservable => TurnEvent;
        internal FixedObservableEvent<ITurnObserver, ObserverPriority, IFixedTurnReceiver> TurnEvent { get; private set; }

        /// <summary>
        /// An <see cref="IObservable{IProductionObserver, ObserverPriority}"/> object which can be observed by <see cref="IProductionObserver"/>.
        /// </summary>
        public IObservable<IProductionObserver, ObserverPriority> ProductionObservable => ProductionEvent;
        internal ObservableEvent<IProductionObserver, ObserverPriority> ProductionEvent { get; private set; }

        /// <summary>
        /// An <see cref="IObservable{ITileObjectObserver, ObserverPriority}"/> object which can be observed by <see cref="ITileObjectObserver"/>.
        /// </summary>
        public IObservable<ITileObjectObserver, ObserverPriority> TileObjectObservable => TileObjectEvent;
        internal ObservableEvent<ITileObjectObserver, ObserverPriority> TileObjectEvent { get; private set; }

        /// <summary>
        /// An <see cref="IObservable{IBattleObserver, ObserverPriority}"/> object which can be observed by <see cref="IBattleObserver"/>.
        /// </summary>
        public IObservable<IBattleObserver, ObserverPriority> BattleObservable => BattleEvent;
        internal ObservableEvent<IBattleObserver, ObserverPriority> BattleEvent { get; private set; }

        /// <summary>
        /// An <see cref="IObservable{IQuestObserver, ObserverPriority}"/> object which can be observed by <see cref="IQuestObserver"/>.
        /// </summary>
        public IObservable<IQuestObserver, ObserverPriority> QuestObservable => QuestEvent;
        internal ObservableEvent<IQuestObserver, ObserverPriority> QuestEvent { get; private set; }

        /// <summary>
        /// An <see cref="IObservable{IVictoryObserver, ObserverPriority}"/> object which can be observed by <see cref="IVictoryObserver"/>.
        /// </summary>
        public IObservable<IVictoryObserver, ObserverPriority> VictoryObservable => VictoryEvent;
        internal ObservableEvent<IVictoryObserver, ObserverPriority> VictoryEvent { get; private set; }

        private void InitializeObservable()
        {
            // fixed & observable
            TurnEvent = new FixedObservableEvent<ITurnObserver, ObserverPriority, IFixedTurnReceiver>(
                () => Players.Cast<IFixedTurnReceiver>());

            // observable
            ProductionEvent = new ObservableEvent<IProductionObserver, ObserverPriority>();
            TileObjectEvent = new ObservableEvent<ITileObjectObserver, ObserverPriority>();
            BattleEvent = new ObservableEvent<IBattleObserver, ObserverPriority>();
            QuestEvent = new ObservableEvent<IQuestObserver, ObserverPriority>();
            VictoryEvent = new ObservableEvent<IVictoryObserver, ObserverPriority>();
        }
    }
}
