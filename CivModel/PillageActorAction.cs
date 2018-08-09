using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents an pillage action.
    /// </summary>
    /// <seealso cref="CivModel.IActorAction" />
    public class PillageActorAction : IActorAction
    {
        /// <summary>
        /// The <see cref="Actor" /> object which has this action.
        /// </summary>
        public Actor Owner => _owner;
        private readonly Actor _owner;

        /// <summary>
        /// Whether the action has a target parameter or not.
        /// </summary>
        public bool IsParametered => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="PillageActorAction"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="Actor"/> who will own the action.</param>
        /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <c>null</c>.</exception>
        public PillageActorAction(Actor owner)
        {
            _owner = owner ?? throw new ArgumentNullException("owner");
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
            if (!_owner.PlacedPoint.HasValue)
                throw new InvalidOperationException("Owner of this action is not placed yet");
            if (target != null)
                throw new ArgumentException("target is invalid", nameof(target));

            if (Owner.PlacedPoint.Value.TileBuilding is TileBuilding tb && tb.Owner != Owner.Owner)
            {
                return 1;
            }

            return ActionPoint.NonAvailable;
        }

        /// <summary>
        /// Acts with the specified parameter.
        /// </summary>
        /// <param name="pt">The parameter.</param>
        /// <exception cref="ArgumentException">AP is not enough</exception>
        /// <exception cref="InvalidOperationException">Owner of this action is not placed yet</exception>
        public void Act(Terrain.Point? pt)
        {
            if (!_owner.PlacedPoint.HasValue)
                throw new InvalidOperationException("Owner of this action is not placed yet");
            if (pt != null)
                throw new ArgumentException("target is invalid", nameof(pt));

            ActionPoint requiredAP = GetRequiredAP(_owner.PlacedPoint.Value, pt);

            if (!_owner.CanConsumeAP(requiredAP))
                throw new ArgumentException("AP is not enough");

            _owner.ConsumeAP(requiredAP);

            var tb = Owner.PlacedPoint.Value.TileBuilding;
            tb.BePillaged(Owner);
        }
    }
}
