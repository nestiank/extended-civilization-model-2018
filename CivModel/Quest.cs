using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivObservable;

namespace CivModel
{
    /// <summary>
    /// The status of <see cref="Quest"/>.
    /// </summary>
    /// <seealso cref="Quest.Status"/>
    public enum QuestStatus
    {
        /// <summary>
        /// <see cref="Quest"/> is disabled.
        /// </summary>
        Disabled,
        /// <summary>
        /// <see cref="Quest"/> is deployed.
        /// </summary>
        Deployed,
        /// <summary>
        /// <see cref="Quest"/> is accepted.
        /// </summary>
        Accepted,
        /// <summary>
        /// <see cref="Quest"/> is completed.
        /// </summary>
        Completed,
    }

    /// <summary>
    /// Represents a quest.
    /// </summary>
    public abstract class Quest : IFixedTurnReceiver
    {
        /// <summary>
        /// The requester of this quest. <c>null</c> if not exists.
        /// </summary>
        public Player Requester { get; }

        /// <summary>
        /// The requestee of this quest.
        /// </summary>
        public Player Requestee { get; }

        /// <summary>
        /// The <see cref="CivModel.Game"/> object.
        /// </summary>
        public Game Game => Requestee.Game;

        /// <summary>
        /// The unique identifier of this class.
        /// </summary>
        public Guid Guid { get; private set; }

        /// <summary>
        /// [퀘스트 이름].
        /// </summary>
        public string TextName { get; private set; }

        /// <summary>
        /// Whether the quest is visible to user or not.
        /// </summary>
        public bool IsVisible { get; private set; }

        /// <summary>
        /// [퀘스트 게시 기간]. <c>-1</c> if forever.
        /// </summary>
        public int PostingTurn { get; private set; }

        /// <summary>
        /// [퀘스트 제한 기간]. <c>-1</c> if forever.
        /// </summary>
        public int LimitTurn { get; private set; }

        /// <summary>
        /// [퀘스트 설명].
        /// </summary>
        public string QuestDescription { get; private set; }

        /// <summary>
        /// [퀘스트 조건].
        /// </summary>
        public string GoalNotice { get; private set; }

        /// <summary>
        /// [퀘스트 보상].
        /// </summary>
        public string RewardNotice { get; private set; }

        /// <summary>
        /// [교육용 알림].
        /// </summary>
        public string CompleteNotice { get; private set; }

        /// <summary>
        /// The list of progress of this quest.
        /// </summary>
        public QuestProgressList Progresses;

        /// <summary>
        /// The current total progress of this quest.
        /// This value is equal to sum of <see cref="QuestProgress.Value"/>.
        /// </summary>
        public int TotalProgress => Progresses.Sum(p => p.Value);

        /// <summary>
        /// The maximum total progress of this quest.
        /// This value is equal to sum of <see cref="QuestProgress.MaxValue"/>.
        /// </summary>
        public int MaxTotalProgress => Progresses.Sum(p => p.MaxValue);

        /// <summary>
        /// The left turn. <c>-1</c> if this value is invalid.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this value become 0 and <c><see cref="Status"/> == <see cref="QuestStatus.Accepted"/></c>,
        ///  <see cref="Status"/> becomes <see cref="QuestStatus.Deployed"/>.<br/>
        /// If this value become 0 and <c><see cref="Status"/> == <see cref="QuestStatus.Deployed"/></c>,
        ///  <see cref="Status"/> becomes <see cref="QuestStatus.Disabled"/>.
        /// </para>
        /// <para>
        /// This value is invalid iff
        ///  <c><see cref="Status"/> == <see cref="QuestStatus.Accepted"/> || <see cref="Status"/> == <see cref="QuestStatus.Deployed"/></c>.
        /// </para>
        /// </remarks>
        public int LeftTurn { get; private set; } = -1;

        /// <summary>
        /// <see cref="QuestStatus"/> of this quest.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// cannot mark <see cref="QuestStatus.Completed"/> quest as <see cref="QuestStatus.Disabled"/>
        /// or
        /// cannot mark <see cref="QuestStatus.Completed"/> quest as <see cref="QuestStatus.Deployed"/>
        /// or
        /// cannot mark <see cref="QuestStatus.Disabled"/> quest as <see cref="QuestStatus.Accepted"/>
        /// or
        /// cannot mark <see cref="QuestStatus.Completed"/> quest as <see cref="QuestStatus.Accepted"/>
        /// or
        /// cannot mark <see cref="QuestStatus.Disabled"/> quest as <see cref="QuestStatus.Completed"/>
        /// or
        /// cannot mark <see cref="QuestStatus.Deployed"/> quest as <see cref="QuestStatus.Completed"/>
        /// </exception>
        public QuestStatus Status
        {
            get => _status;
            set
            {
                switch (value)
                {
                    case QuestStatus.Disabled:
                        Disable();
                        break;

                    case QuestStatus.Deployed:
                        Deploy();
                        break;

                    case QuestStatus.Accepted:
                        Accept();
                        break;

                    case QuestStatus.Completed:
                        Complete();
                        break;
                }
            }
        }
        private QuestStatus _status = QuestStatus.Disabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="Quest"/> class.
        /// </summary>
        /// <param name="requester">The requester of this quest. <c>null</c> if not exists.</param>
        /// <param name="requestee">The requestee of this quest.</param>
        /// <param name="type">The concrete type of this object.</param>
        /// <exception cref="ArgumentNullException"><paramref name="requestee"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="requester"/> and <paramref name="requestee"/> do not involve in the same game</exception>
        public Quest(Player requester, Player requestee, Type type)
        {
            Requester = requester;
            Requestee = requestee ?? throw new ArgumentNullException(nameof(requestee));

            if (requester != null && requester.Game != requestee.Game)
                throw new ArgumentException("requester and requestee do not involve in the same game");

            ApplyPrototype(Game.GetPrototype<QuestPrototype>(type));

            requestee.AddQuestToList(this);
        }

        private void ApplyPrototype(QuestPrototype proto)
        {
            Guid = proto.Guid;
            TextName = proto.TextName;
            IsVisible = proto.IsVisible;
            PostingTurn = proto.PostingTurn;
            LimitTurn = proto.LimitTurn;
            QuestDescription = proto.QuestDescription;
            GoalNotice = proto.GoalNotice;
            RewardNotice = proto.RewardNotice;
            CompleteNotice = proto.CompleteNotice;
            Progresses = new QuestProgressList(this, proto.Progresses);
        }

        /// <summary>
        /// set <see cref="Status"/> to <see cref="QuestStatus.Disabled"/>
        /// </summary>
        /// <exception cref="System.InvalidOperationException">cannot mark <see cref="QuestStatus.Completed"/> quest as <see cref="QuestStatus.Disabled"/></exception>
        /// <seealso cref="Status"/>
        public void Disable()
        {
            var prev = _status;

            if (prev == QuestStatus.Disabled)
                return;
            if (prev == QuestStatus.Completed)
                throw new InvalidOperationException("cannot mark completed quest as disabled");

            _status = QuestStatus.Disabled;
            LeftTurn = -1;

            if (prev == QuestStatus.Accepted)
            {
                CallOnGiveup();
                ClearProgress();
            }
        }

        /// <summary>
        /// set <see cref="Status"/> to <see cref="QuestStatus.Deployed"/>
        /// </summary>
        /// <exception cref="System.InvalidOperationException">cannot mark <see cref="QuestStatus.Completed"/> quest as <see cref="QuestStatus.Deployed"/></exception>
        /// <seealso cref="Status"/>
        public void Deploy()
        {
            var prev = _status;

            if (prev == QuestStatus.Deployed)
                return;
            if (prev == QuestStatus.Completed)
                throw new InvalidOperationException("cannot mark completed quest as deployed");

            _status = QuestStatus.Deployed;
            LeftTurn = PostingTurn;

            if (prev == QuestStatus.Accepted)
            {
                CallOnGiveup();
                ClearProgress();
            }
        }

        /// <summary>
        /// set <see cref="Status"/> to <see cref="QuestStatus.Accepted"/>
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// cannot mark <see cref="QuestStatus.Disabled"/> quest as <see cref="QuestStatus.Accepted"/>
        /// or
        /// cannot mark <see cref="QuestStatus.Completed"/> quest as <see cref="QuestStatus.Accepted"/>
        /// </exception>
        /// <seealso cref="Status"/>
        public void Accept()
        {
            var prev = _status;

            if (prev == QuestStatus.Accepted)
                return;
            if (prev == QuestStatus.Disabled)
                throw new InvalidOperationException("cannot mark disabled quest as accepted");
            if (prev == QuestStatus.Completed)
                throw new InvalidOperationException("cannot mark completed quest as accepted");

            _status = QuestStatus.Accepted;
            LeftTurn = LimitTurn;

            CallOnAccept();
        }

        /// <summary>
        /// set <see cref="Status"/> to <see cref="QuestStatus.Completed"/>
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// cannot mark <see cref="QuestStatus.Disabled"/> quest as <see cref="QuestStatus.Completed"/>
        /// or
        /// cannot mark <see cref="QuestStatus.Deployed"/> quest as <see cref="QuestStatus.Completed"/>
        /// </exception>
        /// <seealso cref="Status"/>
        public void Complete()
        {
            var prev = _status;

            if (prev == QuestStatus.Completed)
                return;
            if (prev == QuestStatus.Disabled)
                throw new InvalidOperationException("cannot mark disabled quest as completed");
            if (prev == QuestStatus.Deployed)
                throw new InvalidOperationException("cannot mark deployed quest as copmleted");

            _status = QuestStatus.Completed;
            LeftTurn = -1;

            CallOnComplete();
        }

        private void ClearProgress()
        {
            foreach (var progress in Progresses)
            {
                progress.Value = 0;
            }
        }

        /// <summary>
        /// Called when Quest Deploy Time of the game.
        /// Override this method to implement when to deploy the quest.
        /// This method is called only if <see cref="Status"/> is <see cref="QuestStatus.Disabled"/>.
        /// </summary>
        public abstract void OnQuestDeployTime();

        IEnumerable<IFixedEventReceiver<IFixedTurnReceiver>> IFixedEventReceiver<IFixedTurnReceiver>.Children => null;
        IFixedTurnReceiver IFixedEventReceiver<IFixedTurnReceiver>.Receiver => this;

        /// <summary>
        /// Called on fixed event [pre turn].
        /// </summary>
        protected virtual void FixedPreTurn()
        {
            if (Status == QuestStatus.Disabled)
            {
                OnQuestDeployTime();
            }
        }
        void IFixedTurnReceiver.FixedPreTurn() => FixedPreTurn();

        /// <summary>
        /// Called on fixed event [after pre turn].
        /// </summary>
        protected virtual void FixedAfterPreTurn()
        {
        }
        void IFixedTurnReceiver.FixedAfterPreTurn() => FixedAfterPreTurn();

        /// <summary>
        /// Called on fixed event [post turn].
        /// </summary>
        protected virtual void FixedPostTurn()
        {
            if (LeftTurn >= 0)
            {
                if (--LeftTurn <= 0)
                {
                    if (Status == QuestStatus.Accepted || Status == QuestStatus.Deployed)
                        Disable();
                }
            }
        }
        void IFixedTurnReceiver.FixedPostTurn() => FixedPostTurn();

        /// <summary>
        /// Called on fixed event [after post turn].
        /// </summary>
        protected virtual void FixedAfterPostTurn()
        {
        }
        void IFixedTurnReceiver.FixedAfterPostTurn() => FixedAfterPostTurn();

        /// <summary>
        /// Called on fixed event [pre subturn].
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        protected virtual void FixedPreSubTurn(Player playerInTurn)
        {
        }
        void IFixedTurnReceiver.FixedPreSubTurn(Player playerInTurn) => FixedPreSubTurn(playerInTurn);

        /// <summary>
        /// Called on fixed event [after pre subturn].
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        protected virtual void FixedAfterPreSubTurn(Player playerInTurn)
        {
        }
        void IFixedTurnReceiver.FixedAfterPreSubTurn(Player playerInTurn) => FixedAfterPreSubTurn(playerInTurn);

        /// <summary>
        /// Called on fixed event [post subturn].
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        protected virtual void FixedPostSubTurn(Player playerInTurn)
        {
        }
        void IFixedTurnReceiver.FixedPostSubTurn(Player playerInTurn) => FixedPostSubTurn(playerInTurn);

        /// <summary>
        /// Called on fixed event [after post subturn]
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        protected virtual void FixedAfterPostSubTurn(Player playerInTurn)
        {
        }
        void IFixedTurnReceiver.FixedAfterPostSubTurn(Player playerInTurn) => FixedAfterPostSubTurn(playerInTurn);

        private void CallOnAccept()
        {
            OnAccept();
            Game.QuestEvent.RaiseObservable(obs => obs.QuestAccepted(this));
        }

        private void CallOnGiveup()
        {
            OnGiveup();
            Game.QuestEvent.RaiseObservable(obs => obs.QuestGivenup(this));
        }

        private void CallOnComplete()
        {
            OnComplete();
            Game.QuestEvent.RaiseObservable(obs => obs.QuestCompleted(this));
        }

        /// <summary>
        /// Called when [accept].
        /// </summary>
        protected abstract void OnAccept();

        /// <summary>
        /// Called when [give up].
        /// </summary>
        protected abstract void OnGiveup();

        /// <summary>
        /// Called when [complete].
        /// </summary>
        protected abstract void OnComplete();
    }
}
