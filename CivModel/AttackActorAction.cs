using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents an attack action.
    /// </summary>
    /// <seealso cref="CivModel.IActorAction" />
    public class AttackActorAction : IActorAction
    {
        /// <summary>
        /// The <see cref="Actor" /> object which has this action.
        /// </summary>
        public Actor Owner => _owner;
        private readonly Actor _owner;

        /// <summary>
        /// Whether the action has a target parameter or not.
        /// </summary>
        public bool IsParametered => true;

        private readonly bool _isMoving;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackActorAction"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="Actor"/> who will own the action.</param>
        /// <param name="isMoving">
        ///   <c>true</c> if this action is <strong>moving attack</strong>.
        ///   <c>false</c> if this action is <strong>holding attack</strong>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <c>null</c>.</exception>
        public AttackActorAction(Actor owner, bool isMoving)
        {
            _owner = owner ?? throw new ArgumentNullException("owner");
            _isMoving = isMoving;
        }

        /// <summary>
        /// Test if the action with given parameter is valid and return required AP to act.
        /// Returns <see cref="ActionPoint.NonAvailable"/> if the action is invalid.
        /// </summary>
        /// <param name="origin">the point where <see cref="Owner"/> would be while testing.</param>
        /// <param name="target">the parameter with which action will be tested.</param>
        /// <returns>
        /// the required AP to act. If the action is invalid, <see cref="ActionPoint.NonAvailable"/>.
        /// </returns>
        public ActionPoint GetRequiredAP(Terrain.Point origin, Terrain.Point? target)
        {
            if (target is Terrain.Point pt)
            {
                Actor targetObject = GetTargetObject(pt);

                if (targetObject != null && !_owner.Owner.IsAlliedWith(targetObject.Owner))
                {
                    if (!_isMoving || !(pt.TileBuilding is CityBase city && city != targetObject))
                    {
                        if (Terrain.Point.Distance(origin, pt) == 1)
                            return _owner.GetRequiredAPToMoveNearBy(origin, pt);
                    }
                }
            }

            return ActionPoint.NonAvailable;
        }

        /// <summary>
        /// Acts the specified pt.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <exception cref="ArgumentException">parameter is invalid</exception>
        /// <exception cref="InvalidOperationException">Actor is not placed yet</exception>
        public void Act(Terrain.Point? pt)
        {
            if (!_owner.PlacedPoint.HasValue)
                throw new InvalidOperationException("Actor is not placed yet");

            ActionPoint requiredAP = GetRequiredAP(_owner.PlacedPoint.Value, pt);

            if (!_owner.CanConsumeAP(requiredAP))
                throw new ArgumentException("parameter is invalid");

            Actor targetObject = GetTargetObject(pt.Value);

            _owner.ConsumeAP(requiredAP);
            var result = _owner.MeleeAttackTo(targetObject);

            if (_isMoving && result == BattleResult.Victory)
            {
                if (pt.Value.Unit == null)
                {
                    _owner.PlacedPoint = pt;
                }
            }
        }

        private Actor GetTargetObject(Terrain.Point target)
        {
            /*if (target.TileBuilding != null && target.TileBuilding.RemainHP > 0)
               return target.TileBuilding;
            else
                return target.Unit;*/
            if (target.Unit != null)
                return target.Unit;
            else if (target.TileBuilding != null && target.TileBuilding.RemainHP > 0)
                return target.TileBuilding;
            else
                return null;
        }
    }
}
