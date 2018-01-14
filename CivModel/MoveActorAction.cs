using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public class MoveActorAction : IActorAction
    {
        public bool IsParametered => true;

        private readonly Actor _owner;

        public MoveActorAction(Actor owner)
        {
            _owner = owner;
        }

        public int GetRequiredAP(Terrain.Point? pt)
        {
            if (pt is Terrain.Point p1 && _owner.PlacedPoint is Terrain.Point p2)
            {
                foreach (var p in p1.Adjacents())
                {
                    if (p == p2)
                    {
                        return 1;
                    }
                }
            }

            return -1;
        }

        public void Act(Terrain.Point? pt)
        {
            int requiredAP = GetRequiredAP(pt);

            if (requiredAP == -1)
                throw new ArgumentException("parameter is invalid");
            if (!_owner.PlacedPoint.HasValue)
                throw new InvalidOperationException("Actor is not placed yet");

            _owner.ConsumeAP(requiredAP);
            _owner.PlacedPoint = pt;
        }
    }
}
