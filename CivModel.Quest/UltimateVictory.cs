using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;

namespace CivModel.Quests
{
    public class FinnoUltimateVictory : IVictoryCondition
    {
        internal static bool Check(Player player)
        {
            var c1 = player.SpecialResource[AutismBeamAmplificationCrystal.Instance] > 0;
            var c2 = player.SpecialResource[Necronomicon.Instance] > 0;
            var c3 = player.SpecialResource[GatesOfRlyeh.Instance] > 0;
            return c1 && c2 && c3;
        }

        public bool CheckVictory(Player player)
        {
            var game = player.Game;
            return game.GetPlayerFinno() == player && Check(player);
        }

        public void DoVictory(Player player)
        {
        }
    }

    public class HwanUltimateVictory : IVictoryCondition
    {
        internal static bool Check(Player player)
        {
            var c1 = player.SpecialResource[SpecialResourceAutismBeamReflex.Instance] > 0;
            var c2 = player.SpecialResource[SpecialResourceCthulhuProjectInfo.Instance] > 0;
            var c3 = player.SpecialResource[SpecialResourceAlienCommunication.Instance] > 0;
            return c1 && c2 && c3;
        }

        public bool CheckVictory(Player player)
        {
            var game = player.Game;
            return game.GetPlayerHwan() == player && Check(player);
        }

        public void DoVictory(Player player)
        {
        }
    }

    public class HyperUltimateDraw : IDrawCondition
    {
        public bool CheckDraw(Player player)
        {
            var game = player.Game;
            var c1 = FinnoUltimateVictory.Check(game.GetPlayerFinno());
            var c2 = HwanUltimateVictory.Check(game.GetPlayerHwan());
            return c1 && c2;
        }

        public bool OnBothVictoriedAndDefeated(Player player, IVictoryCondition victory, IDefeatCondition defeat)
        {
            return false;
        }

        public void DoDraw(Player player)
        {
        }
    }
}
