using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Common
{
    class ControlHijackEffect : Effect
    {
        private readonly Player _hijacker;
        private Player _hijackee;

        private const int _stealTurn = 2;
        private const int _stunTurn = 1;

        private Action DoOff;

        public ControlHijackEffect(Actor target, Player hijacker)
            : base(target, _stealTurn + _stunTurn, EffectTag.Ownership)
        {
            _hijacker = hijacker;
        }

        protected override void OnEffectOn()
        {
            Steal();
        }

        protected override void OnEffectOff()
        {
            DoOff();
        }

        protected override void OnTargetDestroy()
        {
        }

        public override void PostTurn()
        {
            base.PostTurn();
            if (Enabled && LeftTurn == _stunTurn)
            {
                DoOff();
                if (Target != null)
                    Stun();
            }
        }

        private void Steal()
        {
            _hijackee = Target.Owner;
            Target.Owner = _hijacker;
            DoOff = () => {
                if (Target.PlacedPoint?.TileBuilding == null)
                {
                    Target.Owner = _hijackee;
                    Target.SkipFlag = false;
                }
                else
                {
                    // if Target is on TileBuilding of hijacker, Ownership cannot be changed
                    // Target must be moved to another position or destroyed.
                    Target.Destroy();
                }
            };
        }

        private void Stun()
        {
            Target.IsControllable = false;
            DoOff = () => Target.IsControllable = true;
        }
    }
}
