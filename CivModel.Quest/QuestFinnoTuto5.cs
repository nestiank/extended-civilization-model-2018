using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;

namespace CivModel.Quests
{
    public class QuestFinnoTuto5 : Quest
    {
        public QuestFinnoTuto5(Game game)
            : base(game.GetPlayerFinno(), game.GetPlayerFinno(), typeof(QuestFinnoTuto5))
        {
        }

        public override void OnQuestDeployTime()
        {
        }

        protected override void OnAccept()
        {
            Complete();
        }

        protected override void OnComplete()
        {
        }

        protected override void OnGiveup()
        {
        }
    }
}
