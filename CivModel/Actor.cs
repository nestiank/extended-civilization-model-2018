using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public abstract class Actor : TileObject, ITurnObserver
    {
        public abstract Player Owner { get; }

        public abstract int MaxAP { get; }

        public int RemainAP { get; private set; }
        public bool SkipFlag { get; set; }

        public abstract IActorAction MoveAct { get; }
        public abstract IActorAction AttackAct { get; }
        public abstract IReadOnlyList<IActorAction> SpecialActs { get; }

        public Actor(TileTag tag) : base(tag) { }

        public void ConsumeAP(int amount)
        {
            if (amount > RemainAP)
                throw new ArgumentException("Actor.ConsumeAP(): amount is bigger than RemainAP");

            RemainAP -= amount;
        }

        public virtual void PreTurn()
        {
            RemainAP = MaxAP;
            SkipFlag = false;
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
