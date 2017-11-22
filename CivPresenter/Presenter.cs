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
        private bool _moveProcess = false;

        public IReadOnlyList<Terrain.Point?> MoveAdjcents => _moveAdjcents;
        public int MoveSelectedIndex => _moveSelectedIndex;

        public Presenter(IView view)
        {
            _view = view;

            _game = new Game(100, 100, new Player[] { new Player() });
            Player = Game.Players[0];

            FocusedUnit = new Unit(Player);
            FocusedUnit.PlacedPoint = Game.Terrain.GetPoint(50, 50);
        }

        public void CommandArrowKey(Direction direction)
        {
            if (_moveProcess)
            {
                // 난 주석을 달아야 한다는 것을 알고 있지만
                // 해야 한다와 하고 싶다는 다르다는 것도 알고있지
                var table = new int[7, 4] {
                    { 1, 5, 0, 3 },
                    { 1, 5, -1, 1 },
                    { -1, 0, 0, 2 },
                    { -1, 3, 1, 3 },
                    { 2, 4, 4, -1 },
                    { 3, -1, 5, 3 },
                    { 0, -1, 0, 4 }
                };

                int r = _moveSelectedIndex;
                do
                {
                    r = table[r + 1, (int)direction];
                }
                while (!(r == -1 || _moveAdjcents[r] != null));

                if (r != -1)
                    _moveSelectedIndex = r;
            }
            else
            {
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
            }
        }

        public void CommandApply()
        {
            if (_moveProcess && _moveSelectedIndex != -1)
            {
                FocusedUnit.PlacedPoint = _moveAdjcents[_moveSelectedIndex].Value;

                _moveAdjcents = null;
                _moveSelectedIndex = -1;
                _moveProcess = false;
            }
        }

        public void CommandMove()
        {
            if (FocusedUnit != null)
            {
                if (!_moveProcess)
                { 
                    _moveAdjcents = FocusedUnit.PlacedPoint.Value.Adjacents();
                    _moveSelectedIndex = -1;
                    _moveProcess = true;
                }
                else
                {
                    _moveAdjcents = null;
                    _moveSelectedIndex = -1;
                    _moveProcess = false;
                }
            }
        }
    }
}
