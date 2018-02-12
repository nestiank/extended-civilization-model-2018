using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The result of a battle.
    /// </summary>
    /// <seealso cref="Actor.AttackTo(double, Actor, double, bool, bool)"/>
    public enum BattleResult
    {
        /// <summary>
        /// Indicating that battle result is draw.
        /// </summary>
        Draw,
        /// <summary>
        /// Indicating that battle result is victory.
        /// </summary>
        Victory,
        /// <summary>
        /// Indicating that battle result is defeated.
        /// </summary>
        Defeated
    }

    /// <summary>
    /// An absract class represents the <see cref="TileObject"/> which can have actions and action point (AP).
    /// </summary>
    /// <seealso cref="CivModel.TileObject" />
    /// <seealso cref="ITurnObserver"/>
    public abstract class Actor : TileObject, ITurnObserver
    {
        /// <summary>
        /// The player who owns this actor. <c>null</c> if this actor is destroyed.
        /// </summary>
        /// <remarks>
        /// Setter of this property is wrapper of <see cref="ChangeOwner(Player)"/>. See <see cref="ChangeOwner(Player)"/> for more information.
        /// </remarks>
        /// <seealso cref="ChangeOwner(Player)"/>
        public Player Owner
        {
            get => _owner;
            set => ChangeOwner(value);
        }
        private Player _owner;

        /// <summary>
        /// The name of this actor.
        /// </summary>
        public virtual string Name => "";

        /// <summary>
        /// The maximum AP.
        /// </summary>
        public abstract int MaxAP { get; }

        /// <summary>
        /// The remaining AP. It must be in [0, <see cref="MaxAP"/>].
        /// It is reset to <see cref="MaxAP"/> when <see cref="PreTurn"/> is called.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="RemainAP"/> is not in [0, <see cref="MaxAP"/>]</exception>
        public int RemainAP
        {
            get => _remainAP;
            set
            {
                if (value < 0 || value > MaxAP)
                    throw new ArgumentOutOfRangeException("RemainAP", RemainAP, "RemainAP is not in [0, MaxAP]");

                _remainAP = value;
            }
        }
        private int _remainAP = 0;

        /// <summary>
        /// Whether this <see cref="Actor"/> is controllable by <see cref="Owner"/> or not.
        /// </summary>
        public bool IsControllable { get; set; } = true;

        /// <summary>
        /// The flag indicating this actor is skipped in this turn.
        /// If this flag is <c>false</c>, ,<see cref="SleepFlag"/> is also <c>false</c>.
        /// </summary>
        /// <seealso cref="SleepFlag"/>
        public bool SkipFlag
        {
            get => _skipFlag;
            set
            {
                _skipFlag = value;
                if (_sleepFlag && !_skipFlag)
                    _sleepFlag = false;
            }
        }
        private bool _skipFlag = false;

        /// <summary>
        /// The flag indicating this actor is skipped in every turn.
        /// If this flag is <c>true</c>, <see cref="SkipFlag"/> is always <c>true</c>.
        /// </summary>
        /// <seealso cref="SkipFlag"/>
        public bool SleepFlag
        {
            get => _sleepFlag;
            set
            {
                _sleepFlag = value;
                if (_sleepFlag && !_skipFlag)
                    _skipFlag = true;
            }
        }
        private bool _sleepFlag = false;

        /// <summary>
        /// The action performing movement. <c>null</c> if this actor cannot do.
        /// </summary>
        public abstract IActorAction MoveAct { get; }

        /// <summary>
        /// The action performing movement. <c>null</c> if this actor cannot do.
        /// </summary>
        public virtual IActorAction HoldingAttackAct => null;

        /// <summary>
        /// The action performing moving attack. <c>null</c> if this actor cannot do.
        /// </summary>
        public virtual IActorAction MovingAttackAct => null;

        /// <summary>
        /// The list of special actions. <c>null</c> if not exists.
        /// </summary>
        public virtual IReadOnlyList<IActorAction> SpecialActs => null;

        /// <summary>
        /// The attack power.
        /// </summary>
        public virtual double AttackPower => 0;

        /// <summary>
        /// The defence power.
        /// </summary>
        public virtual double DefencePower => 0;

        /// <summary>
        /// The maximum HP. <c>0</c> if this actor is not a combattant.
        /// </summary>
        public virtual double MaxHP => 0;

        /// <summary>
        /// The maximum heal per turn.
        /// </summary>
        /// <seealso cref="RemainHP" />
        public virtual double MaxHealPerTurn => 5;

        /// <summary>
        /// The remaining AP. It must be in [0, <see cref="MaxHP"/>].
        /// If this value gets <c>0</c> while <see cref="MaxHP"/> is not <c>0</c>,
        ///  <see cref="Die(Player)"/> is called with <c>null</c> argument.
        /// </summary>
        /// <remarks>
        /// If this is lower than <see cref="MaxHP"/>,
        ///  this value is increased to min{<see cref="MaxHP"/>, value + <see cref="MaxHealPerTurn"/>}
        ///  when <see cref="PreTurn"/> is called.
        /// </remarks>
        /// <exception cref="InvalidOperationException">actor is already destroyed</exception>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="RemainHP"/> is not in [0, <see cref="MaxHP"/>]</exception>
        public double RemainHP
        {
            get => _remainHP;
            set
            {
                if (Owner == null)
                    throw new InvalidOperationException("actor is already destroyed");
                if (value < 0 || value > MaxHP)
                    throw new ArgumentOutOfRangeException("RemainHP", RemainHP, "RemainHP is not in [0, MaxHP]");

                _remainHP = value;
                if (_remainHP == 0 && MaxHP != 0)
                    Die(null);
            }
        }
        private double _remainHP = 0;

        /// <summary>
        /// Battle class level of this actor. This value can affect the ATK/DEF power during battle.
        /// </summary>
        public virtual int BattleClassLevel => 0;

        private Effect[] _effects;

        /// <summary>
        /// Initializes a new instance of the <see cref="Actor"/> class.
        /// </summary>
        /// <param name="owner">The player who owns this actor.</param>
        /// <param name="point">The tile where the object will be.</param>
        /// <param name="tag">The <seealso cref="TileTag"/> of this actor.</param>
        /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <c>null</c>.</exception>
        public Actor(Player owner, Terrain.Point point, TileTag tag)
            : base(owner?.Game ?? throw new ArgumentNullException(nameof(owner)), point, tag)
        {
            _owner = owner;
            RemainHP = MaxHP;

            _effects = new Effect[Enum.GetNames(typeof(EffectTag)).Length];
        }

        /// <summary>
        /// Gets the <see cref="Effect"/> object on this actor by <see cref="EffectTag"/>.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The <see cref="Effect"/> object.</returns>
        public Effect GetEffectByTag(EffectTag tag)
        {
            return _effects[(int)tag];
        }

        // this method is used by Effect class
        internal void SetEffect(Effect effect)
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");
            if (effect == null)
                throw new ArgumentNullException(nameof(effect));

            _effects[(int)effect.Tag] = effect;
        }

        // this method is used by Effect class
        internal void UnsetEffect(EffectTag tag)
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");

            _effects[(int)tag] = null;
        }

        /// <summary>
        /// Changes <see cref="Owner"/>. <see cref="OnBeforeChangeOwner(Player)"/> is called before the property is changed.
        /// </summary>
        /// <param name="newOwner">The new owner.</param>
        /// <exception cref="InvalidOperationException">
        /// actor is already destroyed
        /// or
        /// the ownership of unit on TileBuilding cannot be changed
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="newOwner"/> is <c>null</c>.</exception>
        /// <seealso cref="Owner"/>
        public void ChangeOwner(Player newOwner)
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");
            if (newOwner == null)
                throw new ArgumentNullException("newOwner");

            if (newOwner == Owner)
                return;
            if (PlacedPoint?.TileBuilding != null)
                throw new InvalidOperationException("the ownership of unit on TileBuilding cannot be changed");

            OnBeforeChangeOwner(newOwner);
            _owner = newOwner;
        }

        /// <summary>
        /// Called before [change owner], by <see cref="ChangeOwner"/>.
        /// </summary>
        /// <param name="newOwner">The new owner.</param>
        protected virtual void OnBeforeChangeOwner(Player newOwner)
        {
        }

        /// <summary>
        /// Destroys this actor. <see cref="OnBeforeDestroy"/> is called before the actor is destroyed.
        /// </summary>
        /// <exception cref="InvalidOperationException">actor is already destroyed</exception>
        /// <remarks>
        /// <strong>postcondition</strong>:
        /// <c><see cref="TileObject.PlacedPoint"/> == null &amp;&amp; <see cref="Owner"/> == null</c>
        /// </remarks>
        public void Destroy()
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");

            OnBeforeDestroy();

            for (int i = 0; i < _effects.Length; ++i)
            {
                _effects[i].CallOnTargetDestroy();
                _effects[i] = null;
            }

            PlacedPoint = null;
            _owner = null;
        }

        /// <summary>
        /// Called before [destroy], by <see cref="Destroy"/>
        /// </summary>
        protected virtual void OnBeforeDestroy()
        {
        }

        /// <summary>
        /// Determines whether this actor can consume the specified amount of AP.
        /// </summary>
        /// <param name="amount">The amount of AP</param>
        /// <returns>
        ///   <c>true</c> if this actor can consume the specified amount of AP; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="InvalidOperationException">actor is already destroyed</exception>
        /// <exception cref="ArgumentException"><paramref name="amount"/> is negative</exception>
        public bool CanConsumeAP(int amount)
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");
            if (amount < 0)
                throw new ArgumentException("amount is negative", "amount");

            return amount <= RemainAP;
        }

        /// <summary>
        /// Consumes the specified amount of AP.
        /// </summary>
        /// <param name="amount">The amount of AP</param>
        /// <exception cref="InvalidOperationException">actor is already destroyed</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="amount"/> is negative
        /// or
        /// <paramref name="amount"/> is bigger than <see cref="RemainAP"/>
        /// </exception>
        public void ConsumeAP(int amount)
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");
            if (amount < 0)
                throw new ArgumentException("amount is negative", "amount");
            if (amount > RemainAP)
                throw new ArgumentException("amount is bigger than RemainAP", "amount");

            _remainAP -= amount;
        }

        /// <summary>
        /// Consumes all of AP which this actor has.
        /// </summary>
        /// <exception cref="InvalidOperationException">actor is already destroyed</exception>
        /// <seealso cref="ConsumeAP(int)"/>
        public void ConsumeAllAP()
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");

            _remainAP = 0;
        }

        /// <summary>
        /// Heals HP of this actor with the specified amount.
        /// </summary>
        /// <exception cref="InvalidOperationException">actor is already destroyed</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="amount"/> is negative.</exception>
        /// <param name="amount">The amount to heal.</param>
        /// <returns>The real amount which this actor was healed.</returns>
        public double Heal(double amount)
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");
            if (amount < 0)
                throw new ArgumentOutOfRangeException("amount", amount, "amount is negative");

            double x = Math.Min(MaxHP, RemainHP + amount);
            double heal = x - _remainHP;
            _remainHP = x;

            return heal;
        }

        /// <summary>
        /// Melee-Attack to another <see cref="Actor"/>.
        /// </summary>
        /// <param name="opposite">The opposite.</param>
        /// <remarks>
        /// This method is wrapper of <see cref="AttackTo(double, Actor, double, bool, bool)"/>.
        /// See <see cref="AttackTo(double, Actor, double, bool, bool)"/> for more information about battle.
        /// </remarks>
        /// <seealso cref="AttackTo(double, Actor, double, bool, bool)"/>
        public BattleResult MeleeAttackTo(Actor opposite)
        {
            return AttackTo(AttackPower, opposite, opposite.DefencePower, true, false);
        }

        /// <summary>
        /// Ranged-Attack to another <see cref="Actor"/>.
        /// </summary>
        /// <param name="opposite">The opposite.</param>
        /// <remarks>
        /// This method is wrapper of <see cref="AttackTo(double, Actor, double, bool, bool)"/>.
        /// See <see cref="AttackTo(double, Actor, double, bool, bool)"/> for more information about battle.
        /// </remarks>
        /// <seealso cref="AttackTo(double, Actor, double, bool, bool)"/>
        public BattleResult RangedAttackTo(Actor opposite)
        {
            return AttackTo(AttackPower, opposite, opposite.DefencePower, false, false);
        }

        /// <summary>
        /// Attack to another <see cref="Actor"/>.
        /// </summary>
        /// <param name="thisAttack">ATK power of this actor.</param>
        /// <param name="opposite">The opposite.</param>
        /// <param name="oppositeDefence">DEF power of <paramref name="opposite"/>.</param>
        /// <param name="isMelee">Whether the battle is melee or not.</param>
        /// <param name="isSkillAttack">Whether the battle </param>
        /// <exception cref="ArgumentNullException"><paramref name="opposite"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        /// actor is already destroyed
        /// or
        /// Opposite actor is already destroyed
        /// </exception>
        /// <returns>
        ///   <see cref="BattleResult"/> indicating the result of this battle.
        ///   if <paramref name="opposite"/> has died, <see cref="BattleResult.Victory"/>.
        ///   if this object has died, <see cref="BattleResult.Defeated"/>.
        ///   if both have died or survived, <see cref="BattleResult.Draw"/>.
        /// </returns>
        /// <remarks>
        /// This method is intented to be used to customerize battle.
        /// <see cref="MeleeAttackTo(Actor)"/>, <see cref="RangedAttackTo(Actor)"/> or battle-causing skills should be used in noraml cases.
        /// </remarks>
        /// <seealso cref="MeleeAttackTo(Actor)"/>
        /// <seealso cref="RangedAttackTo(Actor)"/>
        public BattleResult AttackTo(double thisAttack, Actor opposite, double oppositeDefence, bool isMelee, bool isSkillAttack)
        {
            if (opposite == null)
                throw new ArgumentNullException("opposite");
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");
            if (opposite.Owner == null)
                throw new InvalidOperationException("Opposite actor is already destroyed");

            double atk = CalculateAttackPower(thisAttack, opposite, isMelee, isSkillAttack);
            double def = opposite.CalculateAttackPower(oppositeDefence, this, isMelee, isSkillAttack);

            int rs = 0;

            if (opposite.GetDamage(opposite.CalculateDamage(atk, this, isMelee, isSkillAttack), Owner))
                ++rs;

            if (isMelee)
            {
                if (GetDamage(CalculateDamage(def, opposite, isMelee, isSkillAttack), opposite.Owner))
                    --rs;
            }

            var ret = rs < 0 ? BattleResult.Defeated : (rs > 0 ? BattleResult.Victory : BattleResult.Draw);

            Game.BattleObservable.IterateObserver(obj => obj.OnBattle(this, opposite, ret));

            return ret;
        }

        private bool GetDamage(double damage, Player oppositeOwner)
        {
            _remainHP -= damage;
            if (_remainHP <= 0)
            {
                _remainHP = 0;
                Die(oppositeOwner);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Calculates the ATK which is used during battle.
        /// </summary>
        /// <param name="originalPower">The original ATK power.</param>
        /// <param name="opposite">The opposite of battle.</param>
        /// <param name="isMelee">whether battle is <i>melee</i> type.</param>
        /// <param name="isSkillAttack">whether attack is <i>skill</i> type.</param>
        /// <returns>the ATK power to be used during battle.</returns>
        protected virtual double CalculateAttackPower(double originalPower, Actor opposite, bool isMelee, bool isSkillAttack)
        {
            return originalPower;
        }

        /// <summary>
        /// Calculates the DEF which is used during battle.
        /// </summary>
        /// <param name="originalPower">The original DEF power.</param>
        /// <param name="opposite">The opposite of battle.</param>
        /// <param name="isMelee">whether battle is <i>melee</i> type.</param>
        /// <param name="isSkillAttack">whether attack is <i>skill</i> type.</param>
        /// <returns>the DEF power to be used during battle.</returns>
        protected virtual double CalculateDefencePower(double originalPower, Actor opposite, bool isMelee, bool isSkillAttack)
        {
            return originalPower;
        }

        /// <summary>
        /// Calculates the damage by battle.
        /// </summary>
        /// <param name="originalDamage">The original damage.</param>
        /// <param name="opposite">The opposite of battle.</param>
        /// <param name="isMelee">whether battle is <i>melee</i> type.</param>
        /// <param name="isSkillAttack">whether attack is <i>skill</i> type.</param>
        /// <returns>the damage by battle.</returns>
        protected virtual double CalculateDamage(double originalDamage, Actor opposite, bool isMelee, bool isSkillAttack)
        {
            return originalDamage;
        }

        /// <summary>
        /// Gets the required AP to move to the specified target point from the near.
        /// </summary>
        /// <param name="target">The target point</param>
        /// <returns>the required AP. if this actor cannot move to <paramref name="target"/>, <c>-1</c>.</returns>
        public virtual int GetRequiredAPToMove(Terrain.Point target)
        {
            return 1;
        }

        /// <summary>
        /// Make this actor die. This function calls <see cref="OnDie(Player)"/>.
        /// </summary>
        /// <param name="opposite">The opposite who caused the dying of this actor. If not exists, <c>null</c>.</param>
        /// <exception cref="InvalidOperationException">actor is already destroyed</exception>
        public void Die(Player opposite)
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");

            OnDie(opposite);
        }

        /// <summary>
        /// Called when [die] by <see cref="Die(Player)"/>.
        /// The default implementation calls <see cref="Destroy"/>.
        /// </summary>
        /// <param name="opposite">The opposite who caused the dying of this actor. If not exists, <c>null</c>.</param>
        protected virtual void OnDie(Player opposite)
        {
            Destroy();
        }

        /// <summary>
        /// Called before a turn.
        /// </summary>
        /// <exception cref="InvalidOperationException">actor is already destroyed</exception>
        public virtual void PreTurn()
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");

            _remainAP = MaxAP;

            if (!SleepFlag)
                SkipFlag = false;

            foreach (var effect in _effects)
                effect?.PreTurn();
        }

        /// <summary>
        /// Called after a turn.
        /// </summary>
        /// <exception cref="InvalidOperationException">actor is already destroyed</exception>
        public virtual void PostTurn()
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");

            foreach (var effect in _effects)
                effect?.PostTurn();

            Heal(MaxHealPerTurn);
        }

        /// <summary>
        /// Called before a sub turn.
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        /// <exception cref="InvalidOperationException">actor is already destroyed</exception>
        public virtual void PrePlayerSubTurn(Player playerInTurn)
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");

            foreach (var effect in _effects)
                effect?.PrePlayerSubTurn(playerInTurn);
        }

        /// <summary>
        /// Called after a sub turn.
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        /// <exception cref="InvalidOperationException">actor is already destroyed</exception>
        public virtual void PostPlayerSubTurn(Player playerInTurn)
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");

            foreach (var effect in _effects)
                effect?.PostPlayerSubTurn(playerInTurn);
        }
    }
}
