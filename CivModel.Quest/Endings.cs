using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;

namespace CivModel.Quests
{
    public class FinnoUltimateVictory : Ending
    {
        public FinnoUltimateVictory(Game game) : base(game, typeof(FinnoUltimateVictory)) { }
    }

    public class HwanUltimateVictory : Ending
    {
        public HwanUltimateVictory(Game game) : base(game, typeof(HwanUltimateVictory)) { }
    }

    public class UltimateDraw : Ending
    {
        public UltimateDraw(Game game) : base(game, typeof(UltimateDraw)) { }
    }

    public class UltimateDefeat : Ending
    {
        public UltimateDefeat(Game game) : base(game, typeof(UltimateDefeat)) { }
    }

    public class ConquerVictory : Ending
    {
        public ConquerVictory(Game game) : base(game, typeof(ConquerVictory)) { }
    }

    public class EliminationDefeat : Ending
    {
        public EliminationDefeat(Game game) : base(game, typeof(EliminationDefeat)) { }
    }
}
