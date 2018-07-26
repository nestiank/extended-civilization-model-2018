using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class ConquerVictory : IVictoryCondition
    {
        public bool CheckVictory(Player player)
        {
            foreach (var p in player.Game.Players)
            {
                if (p != player && !p.IsEliminated)
                    return false;
            }
            return true;
        }

        public void DoVictory(Player player)
        {
        }
    }

    public class EliminationDefeat : IDefeatCondition
    {
        public bool CheckDefeat(Player player)
        {
            return player.IsEliminated;
        }

        public void DoDefeat(Player player)
        {
        }
    }

    public class GameEndDefeat : IDefeatCondition
    {
        public bool CheckDefeat(Player player)
        {
            foreach (var p in player.Game.Players)
            {
                if (p.IsVictoried)
                    return true;
            }
            return false;
        }

        public void DoDefeat(Player player)
        {
        }
    }
}
