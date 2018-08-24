using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel.Quests
{
    public class SpecialResourceAutismBeamReflex : ISpecialResource
    {
        public static SpecialResourceAutismBeamReflex Instance => _instance.Value;
        private static Lazy<SpecialResourceAutismBeamReflex> _instance
            = new Lazy<SpecialResourceAutismBeamReflex>(() => new SpecialResourceAutismBeamReflex());

        private SpecialResourceAutismBeamReflex() { }

        public int MaxCount => 1;

        public object EnablePlayer(Player player)
        {
            return new DataObject(player);
        }

        private class DataObject : ITurnObserver, IBattleObserver
        {
            private Player _player;

            public DataObject(Player player)
            {
                _player = player;

                player.Game.TurnObservable.AddObserver(this, ObserverPriority.Model);
                player.Game.BattleObservable.AddObserver(this, ObserverPriority.Model);
            }

            public void PostTurn()
            {
                // TODO
            }

            public void PreTurn() { }
            public void AfterPreTurn() { }
            public void AfterPostTurn() { }
            public void PreSubTurn(Player playerInTurn) { }
            public void AfterPreSubTurn(Player playerInTurn) { }
            public void PostSubTurn(Player playerInTurn) { }
            public void AfterPostSubTurn(Player playerInTurn) { }

            public void OnBeforeBattle(Actor attacker, Actor defender)
            {
                if (_player.SpecialResource[SpecialResourceAutismBeamReflex.Instance] < 1)
                    return;

                if (attacker.Owner != _player && defender.Owner == _player)
                {
                    attacker.AttackPower = attacker.AttackPower / 2;
                }
                else if (attacker.Owner == _player && defender.Owner != _player)
                {
                    defender.DefencePower = defender.DefencePower / 2;
                }

            }

            public void OnAfterBattle(Actor attacker, Actor defender, Player atkOwner, Player defOwner, BattleResult result)
            {
                if (_player.SpecialResource[SpecialResourceAutismBeamReflex.Instance] < 1)
                    return;

                if (attacker.Owner != _player && defender.Owner == _player)
                {
                    attacker.AttackPower = attacker.AttackPower * 2;
                }
                else if (attacker.Owner == _player && defender.Owner != _player)
                {
                    defender.DefencePower = defender.DefencePower * 2;
                }
            }
        }
    }
}
