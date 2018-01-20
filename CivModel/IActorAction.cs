using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public interface IReadOnlyActorAction
    {
        /// <summary>
        /// the <see cref="Actor"/> object which has this action.
        /// </summary>
        Actor Owner { get; }

        /// <summary>
        /// Whether the action has a target parameter or not.
        /// </summary>
        bool IsParametered { get; }

        /// <summary>
        /// test if the action with given parameter is valid and return required AP to act
        /// return <c>-1</c> if the action is invalid.
        /// </summary>
        /// <param name="pt">the parameter with which action will be tested.</param>
        /// <returns>
        /// required AP to act. <c>-1</c> if the action is invalid.
        /// </returns>
        int GetRequiredAP(Terrain.Point? pt);
    }

    public interface IActorAction : IReadOnlyActorAction
    {
        void Act(Terrain.Point? pt);
    }

    public static class ActorAction
    {
        public static bool IsActable(this IReadOnlyActorAction action, Terrain.Point? pt)
        {
            int requiredAP = action.GetRequiredAP(pt);
            return requiredAP != -1 && action.Owner.CanConsumeAP(requiredAP);
        }
    }
}
