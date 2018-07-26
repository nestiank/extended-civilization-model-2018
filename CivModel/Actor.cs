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
        /// Indicating that battle result is draw, and no one has died.
        /// </summary>
        DrawAlive,
        /// <summary>
        /// Indicating that battle result is draw, and both one have died.
        /// </summary>
        DrawDead,
        /// <summary>
        /// Indicating that battle result is victory.
        /// </summary>
        Victory,
        /// <summary>
        /// Indicating that battle result is defeated.
        /// </summary>
        Defeated,
        /// <summary>
        /// Indicating that battle is cancelled
        /// It can be happened by an actor destroyed before damage step.
        /// </summary>
        Cancelled
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
        /// The original constants of this actor. The actual values can be different from the values of this property.
        /// </summary>
        public ActorConstants OriginalConstants => _originalConstants;
        private readonly ActorConstants _originalConstants;

        /// <summary>
        /// The maximum AP.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">MaxAP is negative</exception>
        public double MaxAP
        {
            get => _maxAP;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "MaxAP is negative");
                if (value < RemainAP)
                    RemainAP = value;
                _maxAP = value;
            }
        }
        private double _maxAP;

        /// <summary>
        /// The remaining AP. It must be in [0, <see cref="MaxAP"/>].
        /// It is reset to <see cref="MaxAP"/> when <see cref="PreTurn"/> is called.
        /// </summary>
        /// <remarks>
        /// When setting this property with the value close to <c>0</c> or <see cref="MaxAP"/> within small error,
        /// setter automatically make a correction of that error.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="RemainAP"/> is not in [0, <see cref="MaxAP"/>]</exception>
        public double RemainAP
        {
            get => _remainAP;
            set
            {
                if (value < 0 || value > MaxAP)
                    throw new ArgumentOutOfRangeException("RemainAP", RemainAP, "RemainAP is not in [0, MaxAP]");

                _remainAP = value;

                if (AboutEqual(_remainAP, 0))
                    _remainAP = 0;
                else if (AboutEqual(_remainAP, MaxAP))
                    _remainAP = MaxAP;
            }
        }
        private double _remainAP = 0;

        /// <summary>
        /// The maximum HP. <c>0</c> if this actor is not a combattant.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">MaxHP is negative</exception>
        public double MaxHP
        {
            get => _maxHP;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "MaxHP is negative");
                if (value < RemainHP)
                    RemainHP = value;
                _maxHP = value;
            }
        }
        private double _maxHP;

        /// <summary>
        /// The maximum heal per turn.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">MaxHealPerTurn is negative</exception>
        /// <seealso cref="HealByRepair(double)"/>
        public double MaxHealPerTurn
        {
            get => _maxHealPerTurn;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "MaxHealPerTurn is negative");
                _maxHealPerTurn = value;
            }
        }
        private double _maxHealPerTurn;

        /// <summary>
        /// The remaining HP. It must be in [0, <see cref="MaxHP"/>].
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
        /// The attack power.
        /// </summary>
        public double AttackPower { get; set; }

        /// <summary>
        /// The defence power.
        /// </summary>
        public double DefencePower { get; set; }

        /// <summary>
        /// The amount of gold logistics of this actor.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">GoldLogistics is negative</exception>
        public double GoldLogistics
        {
            get => _goldLogistics;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "GoldLogistics is negative");
                _goldLogistics = value;
            }
        }
        private double _goldLogistics;

        /// <summary>
        /// The amount of labor logistics of this actor.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">LaborLogistics is negative</exception>
        public double LaborLogistics
        {
            get => _laborLogistics;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "LaborLogistics is negative");
                _laborLogistics = value;
            }
        }
        private double _laborLogistics;

        /// <summary>
        /// Whether this actor is provided with appropriate amount of logistics.
        /// </summary>
        public bool IsStarved
        {
            get
            {
                if (Owner == null)
                    throw new InvalidOperationException("actor is already destroyed");
                Owner.CalculateLogistics();
                return _isStarved;
            }
            // this setter is used by Player class
            internal set =>_isStarved = value;
        }
        private bool _isStarved = false;

        /// <summary>
        /// The amount of labor for this actor to get the full heal amount of <see cref="MaxHealPerTurn"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">FullLaborForRepair is negative</exception>
        /// <seealso cref="HealByRepair(double)" />
        public double FullLaborForRepair
        {
            get => _fullLaborForRepair;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "FullLaborForRepair is negative");
                _fullLaborForRepair = value;
            }
        }
        private double _fullLaborForRepair;

        /// <summary>
        /// The amount of labor for repair of this actor to get the maximum heal mount in this turn.
        /// </summary>
        public double BasicLaborForRepair => FullLaborForRepair * Math.Min(MaxHP - RemainHP, MaxHealPerTurn) / MaxHealPerTurn;

        /// <summary>
        /// The amount of labor for repair to be inputed, estimated by <see cref="Player.EstimateResourceInputs"/>.
        /// </summary>
        /// <remarks>
        /// This property is updated by <see cref="Player.EstimateResourceInputs"/>.
        /// You must call that function before use this property.
        /// </remarks>
        /// <seealso cref="Player.EstimateResourceInputs"/>
        public double EstimatedLaborForRepair { get; internal set; }

        /// <summary>
        /// Battle class level of this actor. This value can affect the ATK/DEF power during battle.
        /// </summary>
        public int BattleClassLevel { get; set; }

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
        /// Whether this actor is cloacking.
        /// </summary>
        public bool IsCloacking { get; set; }

        private Effect[] _effects;

        /// <summary>
        /// Initializes a new instance of the <see cref="Actor"/> class.
        /// </summary>
        /// <param name="owner">The player who owns this actor.</param>
        /// <param name="constants">constants of this actor.</param>
        /// <param name="point">The tile where the object will be.</param>
        /// <param name="tag">The <seealso cref="TileTag"/> of this actor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="owner"/> is <c>null</c>.
        /// or
        /// <paramref name="constants"/> is <c>null</c>.
        /// </exception>
        public Actor(Player owner, ActorConstants constants, Terrain.Point point, TileTag tag)
            : base(owner?.Game ?? throw new ArgumentNullException(nameof(owner)), point, tag)
        {
            CopyConstants(constants);
            _originalConstants = constants.Clone();

            _owner = owner;
            RemainHP = MaxHP;

            _effects = new Effect[Enum.GetNames(typeof(EffectTag)).Length];
        }

        private void CopyConstants(ActorConstants constants)
        {
            if (constants == null)
                throw new ArgumentNullException(nameof(constants));

            MaxAP = constants.MaxAP;
            MaxHP = constants.MaxHP;
            MaxHealPerTurn = constants.MaxHealPerTurn;
            AttackPower = constants.AttackPower;
            DefencePower = constants.DefencePower;
            GoldLogistics = constants.GoldLogistics;
            LaborLogistics = constants.LaborLogistics;
            FullLaborForRepair = constants.FullLaborForRepair;
            BattleClassLevel = constants.BattleClassLevel;
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
        /// the ownership of actor on TileBuilding cannot be changed
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

            var tileBuilding = PlacedPoint?.TileBuilding;
            if (tileBuilding != null && tileBuilding != this)
                throw new InvalidOperationException("the ownership of actor on TileBuilding cannot be changed");

            OnBeforeChangeOwner(newOwner);
            var prevOwner = _owner;
            _owner = newOwner;
            OnAfterChangeOwner(prevOwner);
        }

        /// <summary>
        /// Called before [change owner], by <see cref="ChangeOwner"/>.
        /// </summary>
        /// <param name="newOwner">The new owner.</param>
        protected virtual void OnBeforeChangeOwner(Player newOwner)
        {
        }

        /// <summary>
        /// Called after [change owner], by <see cref="ChangeOwner"/>.
        /// </summary>
        /// <param name="prevOwner">The previous owner.</param>
        protected virtual void OnAfterChangeOwner(Player prevOwner)
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
                _effects[i]?.CallOnTargetDestroy();
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
        public bool CanConsumeAP(ActionPoint amount)
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");

            return (amount <= RemainAP);
        }

        /// <summary>
        /// Consumes the specified amount of AP.
        /// </summary>
        /// <param name="amount">The amount of AP</param>
        /// <exception cref="InvalidOperationException">actor is already destroyed</exception>
        /// <exception cref="ArgumentException"><paramref name="amount"/> is too big to consume</exception>
        public void ConsumeAP(ActionPoint amount)
        {
            if (!CanConsumeAP(amount))
                throw new ArgumentException("amount is too big to consume");

            if (amount.IsConsumingAll)
                _remainAP = 0;
            else
                RemainAP -= amount.Value; // call setter of RemainAP to fix floating-point error
        }

        /// <summary>
        /// Consumes all of AP which this actor has.
        /// </summary>
        /// <exception cref="InvalidOperationException">actor is already destroyed</exception>
        /// <seealso cref="ConsumeAP(ActionPoint)"/>
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
        /// Heals this actor by inputing labor for repair.
        /// </summary>
        /// <param name="labor">labor amount to input</param>
        /// <returns>The amount which is really inputed. It can be different from the argument.</returns>
        /// <exception cref="InvalidOperationException">actor is already destroyed</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="labor"/> is negative</exception>
        public double HealByRepair(double labor)
        {
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");
            if (labor < 0)
                throw new ArgumentOutOfRangeException(nameof(labor), labor, "labor is negative");

            labor = Math.Min(labor, BasicLaborForRepair);
            if (AboutEqual(labor, FullLaborForRepair))
                Heal(MaxHealPerTurn);
            else
                Heal(MaxHealPerTurn * labor / FullLaborForRepair);

            return labor;
        }

        /// <summary>
        /// Melee-Attack to another <see cref="Actor"/>.
        /// </summary>
        /// <param name="opposite">The opposite.</param>
        /// <remarks>
        /// This method is wrapper of <see cref="AttackTo(double, Actor, double, bool, bool)"/>.
        /// See <see cref="AttackTo(double, Actor, double, bool, bool)"/> for more information about battle.
        /// </remarks>
        /// <returns><see cref="BattleResult"/> indicating the result of this battle.</returns>
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
        /// <returns><see cref="BattleResult"/> indicating the result of this battle.</returns>
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
        /// <exception cref="ArgumentException">cannot attack myself</exception>
        /// <exception cref="InvalidOperationException">
        /// actor is already destroyed
        /// or
        /// Opposite actor is already destroyed
        /// </exception>
        /// <returns>
        ///   <see cref="BattleResult"/> indicating the result of this battle.
        ///   <ul>
        ///     <li>if <paramref name="opposite"/> has died, <see cref="BattleResult.Victory"/>.</li>
        ///     <li>if this object has died, <see cref="BattleResult.Defeated"/>.</li>
        ///     <li>if both have survived, <see cref="BattleResult.DrawAlive"/>.</li>
        ///     <li>if both have died, <see cref="BattleResult.DrawDead"/>.</li>
        ///     <li>if the battle is cancelled, <see cref="BattleResult.Cancelled"/>.</li>
        ///   </ul>
        /// </returns>
        /// <remarks>
        /// This method is intented to be used to customerize battle.
        /// <see cref="MeleeAttackTo(Actor)"/>, <see cref="RangedAttackTo(Actor)"/> or battle-causing skills should be used in noraml cases.
        /// <br/>
        /// See "전투 매커니즘" documentation of wiki for detailed information about battle.
        /// </remarks>
        /// <seealso cref="MeleeAttackTo(Actor)"/>
        /// <seealso cref="RangedAttackTo(Actor)"/>
        public BattleResult AttackTo(double thisAttack, Actor opposite, double oppositeDefence, bool isMelee, bool isSkillAttack)
        {
            if (opposite == null)
                throw new ArgumentNullException(nameof(opposite));
            if (this == opposite)
                throw new ArgumentException("cannot attack myself", nameof(opposite));
            if (Owner == null)
                throw new InvalidOperationException("actor is already destroyed");
            if (opposite.Owner == null)
                throw new InvalidOperationException("Opposite actor is already destroyed");

            Game.BattleObservable.IterateObserver(obj => obj.OnBeforeBattle(this, opposite));

            double atk = CalculateAttackPower(thisAttack, opposite, isMelee, isSkillAttack);
            double def = opposite.CalculateDefencePower(oppositeDefence, this, isMelee, isSkillAttack);

            double yourDamage = opposite.CalculateDamage(atk, this, opposite, isMelee, isSkillAttack);
            double myDamage = CalculateDamage(def, this, opposite, isMelee, isSkillAttack);

            OnBeforeDamage(atk, def, myDamage, yourDamage,
                this, opposite, isMelee, isSkillAttack);
            opposite.OnBeforeDamage(atk, def, myDamage, yourDamage,
                this, opposite, isMelee, isSkillAttack);

            Player myOwner = Owner;
            Player yourOwner = opposite.Owner;
            if (Owner == null || opposite.Owner == null)
            {
                return BattleResult.Cancelled;
            }

            var ret = BattleResult.DrawAlive;

            if (opposite.GetDamage(yourDamage, Owner))
            {
                ret = BattleResult.Victory;
            }

            if (Owner != null && isMelee)
            {
                if (GetDamage(myDamage, opposite.Owner))
                {
                    ret = (ret == BattleResult.DrawAlive) ? BattleResult.Defeated : BattleResult.DrawDead;
                }
            }

            OnAfterDamage(atk, def, myDamage, yourDamage,
                this, opposite, myOwner, yourOwner, isMelee, isSkillAttack);
            opposite.OnAfterDamage(atk, def, myDamage, yourDamage,
                this, opposite, myOwner, yourOwner, isMelee, isSkillAttack);

            Game.BattleObservable.IterateObserver(obj => obj.OnAfterBattle(this, opposite, myOwner, yourOwner, ret));

            return ret;
        }

        /// <summary>
        /// Gets the damage directly.
        /// </summary>
        /// <param name="damage">The damage.</param>
        /// <param name="oppositeOwner">The owner who gives this damage. This can be <c>null</c>.</param>
        /// <returns>If this actor is died by damage, <c>true</c>. Otherwise, <c>false</c>.</returns>
        public bool GetDamage(double damage, Player oppositeOwner)
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
        /// Calculates the effective ATK which is used during battle.
        /// </summary>
        /// <param name="originalPower">The original ATK power.</param>
        /// <param name="opposite">The opposite of battle.</param>
        /// <param name="isMelee">whether battle is <i>melee</i> type.</param>
        /// <param name="isSkillAttack">whether attack is <i>skill</i> type.</param>
        /// <returns>the ATK power to be used during battle.</returns>
        /// <remarks>
        /// <b>Warning:</b> DO NOT attack <paramref name="opposite"/> in this method. Infinite recursion can occur.
        /// </remarks>
        protected virtual double CalculateAttackPower(double originalPower, Actor opposite, bool isMelee, bool isSkillAttack)
        {
            return originalPower;
        }

        /// <summary>
        /// Calculates the effective DEF which is used during battle.
        /// </summary>
        /// <param name="originalPower">The original DEF power.</param>
        /// <param name="opposite">The opposite of battle.</param>
        /// <param name="isMelee">whether battle is <i>melee</i> type.</param>
        /// <param name="isSkillAttack">whether attack is <i>skill</i> type.</param>
        /// <returns>the DEF power to be used during battle.</returns>
        /// <remarks>
        /// <b>Warning:</b> DO NOT attack <paramref name="opposite"/> in this method. Infinite recursion can occur.
        /// </remarks>
        protected virtual double CalculateDefencePower(double originalPower, Actor opposite, bool isMelee, bool isSkillAttack)
        {
            return originalPower;
        }

        /// <summary>
        /// Calculates the effective damage by battle.
        /// </summary>
        /// <param name="originalDamage">The original damage.</param>
        /// <param name="attacker">The attacker.</param>
        /// <param name="defender">The defender.</param>
        /// <param name="isMelee">whether battle is <i>melee</i> type.</param>
        /// <param name="isSkillAttack">whether attack is <i>skill</i> type.</param>
        /// <returns>the damage by battle.</returns>
        /// <remarks>
        /// When this method called, this actor is either <paramref name="attacker"/> or <paramref name="defender"/>.
        /// <b>Warning:</b> DO NOT attack opposite in this method. Infinite recursion can occur.
        /// </remarks>
        protected virtual double CalculateDamage(double originalDamage, Actor attacker, Actor defender, bool isMelee, bool isSkillAttack)
        {
            return originalDamage;
        }

        /// <summary>
        /// Called before getting damage by battle.
        /// </summary>
        /// <param name="atk">The effecitve attack power calculated by <see cref="CalculateAttackPower(double, Actor, bool, bool)"/> in this battle.</param>
        /// <param name="def">The effecitve defence power calculated by <see cref="CalculateDefencePower(double, Actor, bool, bool)"/> in this battle.</param>
        /// <param name="attackerDamage">The damage attacker will gain, calculated by <see cref="CalculateDamage(double, Actor, Actor, bool, bool)"/>.</param>
        /// <param name="defenderDamage">The damage defender will gain, calculated by <see cref="CalculateDamage(double, Actor, Actor, bool, bool)"/>.</param>
        /// <param name="attacker">The attacker.</param>
        /// <param name="defender">The defender.</param>
        /// <param name="isMelee">whether battle is <i>melee</i> type.</param>
        /// <param name="isSkillAttack">whether attack is <i>skill</i> type.</param>
        /// <remarks>
        /// When this method called, this actor is either <paramref name="attacker"/> or <paramref name="defender"/>.
        /// <b>Warning:</b> DO NOT attack opposite in this method. Infinite recursion can occur.
        /// </remarks>
        protected virtual void OnBeforeDamage(double atk, double def, double attackerDamage, double defenderDamage,
            Actor attacker, Actor defender, bool isMelee, bool isSkillAttack)
        {
        }

        /// <summary>
        /// Called before getting damage by battle. This method can be called even if this actor is died or destroyed during battle.
        /// </summary>
        /// <param name="atk">The effecitve attack power calculated by <see cref="CalculateAttackPower(double, Actor, bool, bool)"/> in this battle.</param>
        /// <param name="def">The effecitve defence power calculated by <see cref="CalculateDefencePower(double, Actor, bool, bool)"/> in this battle.</param>
        /// <param name="attackerDamage">The damage attacker will gain, calculated by <see cref="CalculateDamage(double, Actor, Actor, bool, bool)"/>.</param>
        /// <param name="defenderDamage">The damage defender will gain, calculated by <see cref="CalculateDamage(double, Actor, Actor, bool, bool)"/>.</param>
        /// <param name="attacker">The attacker.</param>
        /// <param name="defender">The defender.</param>
        /// <param name="atkOwner">The owner of attacker.</param>
        /// <param name="defOwner">The owner of defender.</param>
        /// <param name="isMelee">whether battle is <i>melee</i> type.</param>
        /// <param name="isSkillAttack">whether attack is <i>skill</i> type.</param>
        /// <remarks>
        /// When this method called, this actor is either <paramref name="attacker"/> or <paramref name="defender"/>.<br/>
        /// <b>Warning:</b> This method can be called even if this actor is died or destroyed during battle.
        /// You must check it by compare <see cref="Owner"/> with <c>null</c>.
        /// </remarks>
        protected virtual void OnAfterDamage(double atk, double def, double attackerDamage, double defenderDamage,
            Actor attacker, Actor defender, Player atkOwner, Player defOwner, bool isMelee, bool isSkillAttack)
        {
        }

        /// <summary>
        /// Gets the required AP to move to point, assuming the point is near the position of this actor.
        /// </summary>
        /// <param name="point">The <see cref="Terrain.Point"/> struct object</param>
        /// <returns>the required AP.</returns>
        /// <remarks>
        /// This method assumes <paramref name="point"/> is near <see cref="TileObject.PlacedPoint"/> of this actor.<br/>
        /// Thus, <see cref="Terrain.Point.Position"/> of <paramref name="point"/> can be ignored.
        /// </remarks>
        public virtual ActionPoint GetRequiredAPToMoveNearBy(Terrain.Point point)
        {
            // POMFSTIH
            switch (point.Type)
            {
                case TerrainType.Plain: return 1;
                case TerrainType.Ocean:
                    if (PlacedPoint?.Type != TerrainType.Ocean)
                        return new ActionPoint(0.5, consumeAll: true);
                    else
                        return 0.5;
                case TerrainType.Mount: return 3;
                case TerrainType.Forest: return 2;
                case TerrainType.Swamp: return 2;
                case TerrainType.Tundra: return 1;
                case TerrainType.Ice: return 2;
                case TerrainType.Hill: return 2;
                default: throw new NotImplementedException("unqualified TerrainType");
            }
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

        // compare floating point with relative error.
        // https://stackoverflow.com/questions/2411392/double-epsilon-for-equality-greater-than-less-than-less-than-or-equal-to-gre
        private static bool AboutEqual(double x, double y)
        {
            double epsilon = Math.Max(Math.Abs(x), Math.Abs(y)) * 1E-15;
            return Math.Abs(x - y) <= epsilon;
        }
    }
}
