using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The interface to observe <see cref="Quest"/> related events.
    /// </summary>
    /// <seealso cref="Game"/>
    /// <seealso cref="Quest"/>
    public interface IQuestObserver
    {
        /// <summary>
        /// Called when quest is accepted.
        /// </summary>
        /// <param name="quest"></param>
        void QuestAccepted(Quest quest);
        /// <summary>
        /// Called when quest is given up.
        /// </summary>
        /// <param name="quest"></param>
        void QuestGivenup(Quest quest);
        /// <summary>
        /// Called when quest is completed.
        /// </summary>
        /// <param name="quest"></param>
        void QuestCompleted(Quest quest);
    }
}
