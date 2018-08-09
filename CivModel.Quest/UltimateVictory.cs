using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static CivModel.Finno.FinnoPlayerNumber;
using static CivModel.Hwan.HwanPlayerNumber;

namespace CivModel.Quests
{
    public class FinnoUltimateVictory : IVictoryCondition, ITurnObserver
    {
        private Game _game;

        internal bool Condition { get; private set; }

        public FinnoUltimateVictory(Game game)
        {
            _game = game;
            game.TurnObservable.AddObserver(this, ObserverPriority.Model);
        }

        public bool CheckVictory(Player player)
        {
            return Condition;
        }

        public void DoVictory(Player player)
        {
        }

        public void PostSubTurn(Player playerInTurn)
        {
            if (playerInTurn == _game.GetPlayerFinno() && !playerInTurn.HasEnding)
            {
                var c1 = playerInTurn.SpecialResource[AutismBeamAmplificationCrystal.Instance] > 0;
                var c2 = playerInTurn.SpecialResource[Necronomicon.Instance] > 0;
                var c3 = playerInTurn.SpecialResource[GatesOfRlyeh.Instance] > 0;
                Condition = c1 && c2 && c3;
            }
        }

        public void PreTurn() { }
        public void AfterPreTurn() { }
        public void PostTurn() { }
        public void AfterPostTurn() { }
        public void PreSubTurn(Player playerInTurn) { }
        public void AfterPreSubTurn(Player playerInTurn) { }
        public void AfterPostSubTurn(Player playerInTurn) { }
    }

    public class HwanUltimateVictory : IVictoryCondition, ITurnObserver
    {
        private Game _game;

        internal bool Condition { get; private set; }

        public HwanUltimateVictory(Game game)
        {
            _game = game;
            game.TurnObservable.AddObserver(this, ObserverPriority.Model);
        }

        public bool CheckVictory(Player player)
        {
            return Condition;
        }

        public void DoVictory(Player player)
        {
        }

        public void PostSubTurn(Player playerInTurn)
        {
            if (playerInTurn == _game.GetPlayerHwan() && !playerInTurn.HasEnding)
            {
                var c1 = playerInTurn.SpecialResource[SpecialResourceAutismBeamReflex.Instance] > 0;
                var c2 = playerInTurn.SpecialResource[SpecialResourceCthulhuProjectInfo.Instance] > 0;
                var c3 = playerInTurn.SpecialResource[SpecialResourceAlienCommunication.Instance] > 0;
                Condition = c1 && c2 && c3;
            }
        }

        public void PreTurn() { }
        public void AfterPreTurn() { }
        public void PostTurn() { }
        public void AfterPostTurn() { }
        public void PreSubTurn(Player playerInTurn) { }
        public void AfterPreSubTurn(Player playerInTurn) { }
        public void AfterPostSubTurn(Player playerInTurn) { }
    }

    public class HyperUltimateDraw : IDrawCondition
    {
        public bool CheckDraw(Player player)
        {
            var game = player.Game;
            var finno = game.GetPlayerFinno().AvailableVictories.OfType<FinnoUltimateVictory>().First();
            var hwan = game.GetPlayerHwan().AvailableVictories.OfType<HwanUltimateVictory>().First();
            return finno.Condition && hwan.Condition;
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
