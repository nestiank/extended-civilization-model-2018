using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivModel;
using CivModel.Units;
using CivModel.TileBuildings;

namespace CivPresenter
{
    public class Presenter
    {
        private readonly IView _view;
        public IView View => _view;

        private readonly Game _game;
        public Game Game => _game;

        public Actor FocusedActor { get; private set; }

        private Terrain.Point?[] _moveAdjcents = null;
        private int _moveSelectedIndex = -1;

        public IReadOnlyList<Terrain.Point?> MoveAdjcents => _moveAdjcents;
        public int MoveSelectedIndex => _moveSelectedIndex;

        public enum States
        {
            Normal, Move, SpecialAct
        }
        public States State { get; private set; }
        public int StateParam { get; private set; }

        private Action OnApply;
        private Action OnCancel;
        private Action<Direction> OnArrowKey;

        public Presenter(IView view)
        {
            _view = view;

            _game = new Game(width: 100, height: 100, numOfPlayer: 1);

            FocusedActor = new Pioneer(Game.Players[0]);
            FocusedActor.PlacedPoint = Game.Terrain.GetPoint(50, 50);

            _game.StartTurn();

            StateNormal();
        }

        public void CommandApply()
        {
            OnApply();
        }

        public void CommandCancel()
        {
            OnCancel();
        }

        public void CommandArrowKey(Direction direction)
        {
            OnArrowKey(direction);
        }

        public void CommandRefocus()
        {
            View.Refocus();
        }

        public void CommandMove()
        {
            if (State != States.Move)
                StateMove();
            else
                OnCancel();
        }

        public void CommandSpecialAct(int index)
        {
            if (State != States.SpecialAct || StateParam != index)
                StateSpeicalAct(index);
            else
                OnCancel();
        }

        private void StateNormal()
        {
            State = States.Normal;

            OnApply = () => {
                Game.EndTurn();
                Game.StartTurn();
            };
            OnCancel = () => {
                View.Shutdown();
            };
            OnArrowKey = direction => {
                int dx = 0, dy = 0;
                switch (direction)
                {
                    case Direction.Up:
                        dy = -1;
                        break;
                    case Direction.Down:
                        dy = 1;
                        break;
                    case Direction.Left:
                        dx = -1;
                        break;
                    case Direction.Right:
                        dx = 1;
                        break;
                }
                View.MoveSight(dx, dy);
            };
        }

        private void StateMove()
        {
            if (FocusedActor.MoveAct == null)
                return;

            State = States.Move;
            _moveAdjcents = FocusedActor.PlacedPoint.Value.Adjacents();
            _moveSelectedIndex = -1;

            for (int i = 0; i < _moveAdjcents.Length; ++i)
            {
                int ap = FocusedActor.MoveAct.GetRequiredAP(_moveAdjcents[i]);
                if (ap == -1 || ap > FocusedActor.RemainAP)
                    _moveAdjcents[i] = null;
            }

            OnApply = () => {
                if (_moveSelectedIndex != -1 && _moveAdjcents[_moveSelectedIndex] != null)
                {
                    FocusedActor.MoveAct.Act(_moveAdjcents[_moveSelectedIndex]);
                    OnCancel();
                }
            };
            OnCancel = () => {
                _moveAdjcents = null;
                _moveSelectedIndex = -1;
                StateNormal();
            };
            OnArrowKey = direction => {
                // table[index + 1, (int)direction]
                //  == index after move
                // table[0,*] is for an initial state, or index == -1
                // @ref CivModel.Terrain.Point.Adjacents
                //   1   2
                // 0  -1  3
                //   5   4
                var table = new int[7, 4] {
                    { 1, 5, 0, 3 },
                    { 1, 5, -1, 3 },
                    { -1, 5, 0, 2 },
                    { -1, 4, 1, 3 },
                    { 2, 4, 0, -1 },
                    { 2, -1, 5, 3 },
                    { 1, -1, 0, 4 }
                };

                int r = _moveSelectedIndex;
                do
                {
                    r = table[r + 1, (int)direction];
                }
                while (!(r == -1 || _moveAdjcents[r] != null));

                if (r != -1)
                    _moveSelectedIndex = r;
            };
        }

        private void StateSpeicalAct(int index)
        {
            if (FocusedActor.SpecialActs == null)
                return;
            if (index < 0 || index >= FocusedActor.SpecialActs.Count)
                return;

            FocusedActor.SpecialActs[index].Act(null);
        }
    }
}
