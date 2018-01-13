using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    public interface IActorAction
    {
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

        void Act(Terrain.Point? pt);
    }
}
