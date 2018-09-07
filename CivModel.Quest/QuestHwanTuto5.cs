using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Hwan.HwanPlayerNumber;

namespace CivModel.Quests
{
    public class QuestHwanTuto5 : Quest
    {
        public QuestHwanTuto5(Game game)
            : base(game.GetPlayerHwan(), game.GetPlayerHwan(), typeof(QuestHwanTuto5))
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
            for (int i = 0; i < 10; ++i)
            {
                Requestee.Deployment.AddLast(Hwan.HwanEmpireCityCentralLabProductionFactory.Instance.Create(Requestee));
            }
        }

        protected override void OnGiveup()
        {
        }
    }
}
