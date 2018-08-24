using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivObservable;

namespace CivModel
{
    interface IEffectTarget
    {
        void AddEffect(Effect effect);
        void RemoveEffect(Effect effect);
    }

    /// <summary>
    /// An abstract classs represents an effect.
    /// This class is used internally cannot be directly inherited.
    /// </summary>
    public abstract class Effect : IFixedTurnReceiver
    {
        /// <summary>
        /// The target of this effect. <c>null</c> if target was destroyed.
        /// </summary>
        public object Target => _target;
        private IEffectTarget _target;

        /// <summary>
        /// The duration turn of this effect.
        /// </summary>
        /// <see cref="LeftTurn"/>
        public int Duration { get; }

        /// <summary>
        /// The left duration turn of this effect.
        /// </summary>
        /// <see cref="Duration"/>
        public int LeftTurn { get; private set; } = -1;

        /// <summary>
        /// Whether this effect is enabled.
        /// </summary>
        /// <remarks>
        /// The setter of this property is a wrapper of <see cref="EffectOn"/> and <see cref="EffectOff"/>.
        /// </remarks>
        /// <seealso cref="EffectOn"/>
        /// <seealso cref="EffectOff"/>
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value && !_enabled)
                    EffectOn();
                else if (!value && _enabled)
                    EffectOff();
            }
        }
        private bool _enabled = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Effect"/> class.
        /// </summary>
        /// <param name="target">The target of the effect.</param>
        /// <param name="duration">The duration of the effect. <c>-1</c> if forever.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is negative and not -1</exception>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <c>null</c>.</exception>
        internal Effect(IEffectTarget target, int duration)
        {
            if (duration < -1)
                throw new ArgumentOutOfRangeException(nameof(duration), duration, "duration is negative and not -1");

            _target = target ?? throw new ArgumentNullException(nameof(target));
            Duration = duration;
        }

        /// <summary>
        /// Turn on this effect on <see cref="Target"/> and set <see cref="Enabled"/> to <c>true</c>.
        /// </summary>
        /// <seealso cref="Enabled"/>
        public void EffectOn()
        {
            if (Enabled)
                throw new InvalidOperationException("effect is already turned on");

            _target.AddEffect(this);
            LeftTurn = Duration;
            _enabled = true;

            OnEffectOn();
        }

        /// <summary>
        /// Turn off this effect on <see cref="Target"/> and set <see cref="Enabled"/> to <c>false</c>.
        /// </summary>
        /// <seealso cref="Enabled"/>
        public void EffectOff()
        {
            if (!Enabled)
                throw new InvalidOperationException("effect is not turned on");

            _target.RemoveEffect(this);
            LeftTurn = -1;
            _enabled = false;

            OnEffectOff();
        }

        // this method is used by Actor class
        internal void CallOnTargetDestroy()
        {
            OnTargetDestroy();

            _target = null;
            LeftTurn = -1;
            _enabled = false;
        }

        /// <summary>
        /// Called when <see cref="EffectOn"/> is called. This method should turn on this effect.
        /// </summary>
        protected abstract void OnEffectOn();

        /// <summary>
        /// Called when <see cref="EffectOff"/> is called. This method should turn off this effect.
        /// </summary>
        protected abstract void OnEffectOff();

        /// <summary>
        /// Called when <see cref="Target"/> is destroyed.
        /// </summary>
        protected abstract void OnTargetDestroy();

        IEnumerable<IFixedEventReceiver<IFixedTurnReceiver>> IFixedEventReceiver<IFixedTurnReceiver>.Children => null;
        IFixedTurnReceiver IFixedEventReceiver<IFixedTurnReceiver>.Receiver => this;

        /// <summary>
        /// Called on fixed event [pre turn].
        /// </summary>
        protected virtual void FixedPreTurn()
        {
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
            if (Enabled && LeftTurn >= 0)
            {
                if (--LeftTurn <= 0)
                {
                    EffectOff();
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
    }
}
