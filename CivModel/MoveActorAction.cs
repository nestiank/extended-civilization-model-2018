using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents an movement action.
    /// </summary>
    /// <seealso cref="CivModel.IActorAction" />
    public class MoveActorAction : IActorAction
    {
        public Actor Owner => _owner;
        private readonly Actor _owner;

        public bool IsParametered => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveActorAction"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="Actor"/> who will own the action.</param>
        /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <c>null</c>.</exception>
        public MoveActorAction(Actor owner)
        {
            _owner = owner ?? throw new ArgumentNullException("owner");
        }

        public int GetRequiredAP(Terrain.Point? pt)
        {
            if (pt is Terrain.Point target && _owner.PlacedPoint is Terrain.Point origin)
            {
                if (target.Unit == null)
                {
                    if (Position.Distance(origin.Position, target.Position) == 1)
                        return _owner.GetRequiredAPToMove(target);
                }
            }

            return -1;
        }

        public void Act(Terrain.Point? pt)
        {
            int requiredAP = GetRequiredAP(pt);

            if (requiredAP == -1 || !_owner.CanConsumeAP(requiredAP))
                throw new ArgumentException("parameter is invalid");
            if (!_owner.PlacedPoint.HasValue)
                throw new InvalidOperationException("Owner of this action is not placed yet");

            _owner.ConsumeAP(requiredAP);
            _owner.PlacedPoint = pt;
        }
    }
}
