using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivObservable;

namespace CivModel
{
    /// <summary>
    /// The interface to observe pre/post turn event.
    /// </summary>
    /// <seealso cref="Game"/>
    public interface ITurnObserver
    {
        /// <summary>
        /// Called on observable event [pre turn].
        /// </summary>
        void PreTurn();

        /// <summary>
        /// Called on observable event [after pre turn].
        /// </summary>
        void AfterPreTurn();

        /// <summary>
        /// Called on observable event [post turn].
        /// </summary>
        void PostTurn();

        /// <summary>
        /// Called on observable event [after post turn].
        /// </summary>
        void AfterPostTurn();

        /// <summary>
        /// Called on observable event [pre subturn].
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        void PreSubTurn(Player playerInTurn);

        /// <summary>
        /// Called on observable event [after pre subturn].
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        void AfterPreSubTurn(Player playerInTurn);

        /// <summary>
        /// Called on observable event [post subturn].
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        void PostSubTurn(Player playerInTurn);

        /// <summary>
        /// Called on observable event [after post subturn].
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        void AfterPostSubTurn(Player playerInTurn);
    }

    interface IFixedTurnReceiver : IFixedEventReceiver<IFixedTurnReceiver>
    {
        void FixedPreTurn();
        void FixedAfterPreTurn();

        void FixedPostTurn();
        void FixedAfterPostTurn();

        void FixedPreSubTurn(Player playerInTurn);
        void FixedAfterPreSubTurn(Player playerInTurn);

        void FixedPostSubTurn(Player playerInTurn);
        void FixedAfterPostSubTurn(Player playerInTurn);
    }
}
