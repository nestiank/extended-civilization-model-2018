using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class HwanUltimateVictory : IVictoryCondition
    {
        public bool CheckVictory(Player player)
        {
            var c1 = player.SpecialResource[SpecialResourceAutismBeamReflex.Instance] > 0;
            var c2 = player.SpecialResource[SpecialResourceCthulhuProjectInfo.Instance] > 0;
            var c3 = player.SpecialResource[SpecialResourceAlienCommunication.Instance] > 0;
            return c1 && c2 && c3;
        }

        public void DoVictory(Player player)
        {
        }
    }
}
