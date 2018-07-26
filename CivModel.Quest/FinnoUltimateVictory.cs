using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class FinnoUltimateVictory : IVictoryCondition
    {
        public bool CheckVictory(Player player)
        {
            var c1 = player.SpecialResource[AutismBeamAmplificationCrystal.Instance] > 0;
            var c2 = player.SpecialResource[Necronomicon.Instance] > 0;
            var c3 = player.SpecialResource[GatesOfRlyeh.Instance] > 0;
            return c1 && c2 && c3;
        }

        public void DoVictory(Player player)
        {
        }
    }
}
