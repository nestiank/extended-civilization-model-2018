using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivModel;

namespace CivPresenter
{
    public class Presenter
    {
        private readonly IView _view;
        public IView View => _view;

        private readonly Game _game;
        public Game Game => _game;

        public Player Player { get; private set; }

        public Unit FocusedUnit { get; private set; }

        private Terrain.Point?[] _moveAdjcents = null;
        private int _moveSelectedIndex = -1;

        public IReadOnlyList<Terrain.Point?> MoveAdjcents => _moveAdjcents;
        public int MoveSelectedIndex => _moveSelectedIndex;

        private enum State
        {
            Normal, Move
        }
        private State _state;
        private Action OnApply;
        private Action OnCancel;
        private Action<Direction> OnArrowKey;

        public Presenter(IView view)
        {
            _view = view;

            _game = new Game(100, 100, new Player[] { new Player() });
            Player = Game.Players[0];

            FocusedUnit = new Unit(Player);
            FocusedUnit.PlacedPoint = Game.Terrain.GetPoint(50, 50);

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

        public void CommandMove()
        {
            if (_state != State.Move)
                StateMove();
            else
                OnCancel();
        }

        private void StateNormal()
        {
            _state = State.Normal;

            OnApply = () => { };
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
            _state = State.Move;
            _moveAdjcents = FocusedUnit.PlacedPoint.Value.Adjacents();
            _moveSelectedIndex = -1;

            OnApply = () => {
                if (_moveSelectedIndex != -1)
                {
                    FocusedUnit.PlacedPoint = _moveAdjcents[_moveSelectedIndex].Value;

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
    }
}
