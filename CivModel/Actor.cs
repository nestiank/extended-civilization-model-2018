using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public enum BattleResult
    {
        Draw, Victory, Defeated
    }

    public abstract class Actor : TileObject, ITurnObserver
    {
        public Player Owner { get; private set; }

        public abstract int MaxAP { get; }

        public int RemainAP { get; private set; }
        public bool SkipFlag { get; set; }

        public abstract IActorAction MoveAct { get; }
        public virtual IActorAction HoldingAttackAct => null;
        public virtual IActorAction MovingAttackAct => null;
        public virtual IReadOnlyList<IActorAction> SpecialActs => null;

        public virtual double AttackPower => 0;
        public virtual double DefencePower => 0;
        public virtual double MaxHP => 0;

        public virtual double MaxHealPerTurn => 5;

        private double _remainHP = 0;
        public double RemainHP
        {
            get => _remainHP;
            set
            {
                if (value < 0 || value > MaxHP)
                    throw new ArgumentException("RemainHP must be in [0, MaxHP]");

                _remainHP = value;
                if (_remainHP == 0 && MaxHP != 0)
                    Die(null);
            }
        }

        public Actor(Player owner, TileTag tag) : base(tag)
        {
            Owner = owner ?? throw new ArgumentNullException("owner");
            RemainHP = MaxHP;
        }

        public void ChangeOwner(Player newOwner)
        {
            if (newOwner == null)
                throw new ArgumentNullException("value");
            if (newOwner == Owner)
                return;

            OnChangeOwner(newOwner);
            Owner = newOwner;
        }

        protected virtual void OnChangeOwner(Player newOwner)
        {
        }

        public void Destroy()
        {
            OnDestroy();
            PlacedPoint = null;
            Owner = null;
        }

        protected virtual void OnDestroy()
        {
        }

        public bool CanConsumeAP(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("amount must be not negative", "amount");

            return amount <= RemainAP;
        }

        public void ConsumeAP(int amount)
        {
            if (amount < 0)
                throw new ArgumentException("amount must be not negative", "amount");
            if (amount > RemainAP)
                throw new ArgumentException("amount is bigger than RemainAP", "amount");

            RemainAP -= amount;
        }

        public void ConsumeAllAP()
        {
            RemainAP = 0;
        }

        public BattleResult AttackTo(Actor opposite)
        {
            int rs = 0;

            _remainHP -= opposite.DefencePower;
            opposite._remainHP -= AttackPower;

            if (_remainHP <= 0)
            {
                _remainHP = 0;
                Die(opposite.Owner);
                --rs;
            }
            else if (_remainHP > MaxHP)
            {
                _remainHP = MaxHP;
            }

            if (opposite._remainHP <= 0)
            {
                opposite._remainHP = 0;
                opposite.Die(Owner);
                ++rs;
            }
            else if (opposite._remainHP > opposite.MaxHP)
            {
                opposite._remainHP = opposite.MaxHP;
            }

            return rs < 0 ? BattleResult.Defeated : (rs > 0 ? BattleResult.Victory : BattleResult.Draw);
        }

        public virtual int GetRequiredAPToMove(Terrain.Point target)
        {
            return 1;
        }

        public void Die(Player opposite)
        {
            OnDie(opposite);
        }

        protected virtual void OnDie(Player opposite)
        {
            Destroy();
        }

        public virtual void PreTurn()
        {
            RemainAP = MaxAP;
            SkipFlag = false;

            RemainHP = Math.Min(MaxHP, RemainHP + MaxHealPerTurn);
        }

        public virtual void PostTurn()
        {
        }

        public virtual void PrePlayerSubTurn(Player playerInTurn)
        {
        }

        public virtual void PostPlayerSubTurn(Player playerInTurn)
        {
        }
    }
}
