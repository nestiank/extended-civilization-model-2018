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

        private Actor _selectedActor;
        public Actor SelectedActor => _selectedActor?.PlacedPoint != null ? _selectedActor : null;

        public Terrain.Point FocusedPoint { get; private set; }

        private Terrain.Point?[] _moveAdjcents = null;
        private int _moveSelectedIndex = -1;

        public IReadOnlyList<Terrain.Point?> MoveAdjcents => _moveAdjcents;
        public int MoveSelectedIndex => _moveSelectedIndex;

        public int SelectedDeploy { get; private set; } = -1;
        public int SelectedProduction { get; private set; } = -1;
        public bool IsProductCancelling { get; private set; } = false;

        public IReadOnlyList<IProductionFactory> AvailableProduction { get; private set; }

        public Production DeployProduction { get; private set; }

        public enum States
        {
            Normal, Move, SpecialAct,
            ProductUI, ProductAdd, Deploy
        }
        public States State { get; private set; }
        public int StateParam { get; private set; }

        private Action OnApply;
        private Action OnCancel;
        private Action<Direction> OnArrowKey;
        private Action<int> OnNumeric;

        public Presenter(IView view)
        {
            _view = view;

            _game = new Game(width: 100, height: 100, numOfPlayer: 1);

            _selectedActor = new Pioneer(Game.Players[0]);
            _selectedActor.PlacedPoint = Game.Terrain.GetPoint(50, 50);

            _game.StartTurn();

            FocusedPoint = Game.Terrain.GetPoint(0, 0);

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

        public void CommandNumeric(int index)
        {
            OnNumeric(index);
        }

        public void CommandRefocus()
        {
            if (SelectedActor != null)
                FocusedPoint = SelectedActor.PlacedPoint.Value;
        }

        public void CommandSelect()
        {
            if (FocusedPoint.Unit != null)
            {
                _selectedActor = FocusedPoint.Unit;
            }
        }

        public void CommandMove()
        {
            if (State != States.Move)
                StateMove();
            else
                OnCancel();
        }

        public void CommandProductUI()
        {
            if (State != States.ProductUI)
                StateProductUI();
            else
                OnCancel();
        }

        private void MoveSight(Direction direction)
        {
            var pos = FocusedPoint.Position;
            switch (direction)
            {
                case Direction.Up:
                    pos.Y = Math.Max(pos.Y - 1, 0);
                    break;
                case Direction.Down:
                    pos.Y = Math.Min(pos.Y + 1, Game.Terrain.Height - 1);
                    break;
                case Direction.Left:
                    pos.X = Math.Max(pos.X - 1, 0);
                    break;
                case Direction.Right:
                    pos.X = Math.Min(pos.X + 1, Game.Terrain.Width - 1);
                    break;
            }
            FocusedPoint = Game.Terrain.GetPoint(pos);
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
                MoveSight(direction);
            };
            OnNumeric = index => {
                if (State != States.SpecialAct || StateParam != index)
                    StateSpeicalAct(index);
                else
                    OnCancel();
            };
        }

        private void StateMove()
        {
            if (SelectedActor?.MoveAct == null)
                return;

            State = States.Move;

            _moveAdjcents = SelectedActor.PlacedPoint.Value.Adjacents();
            _moveSelectedIndex = -1;

            for (int i = 0; i < _moveAdjcents.Length; ++i)
            {
                int ap = SelectedActor.MoveAct.GetRequiredAP(_moveAdjcents[i]);
                if (ap == -1 || ap > SelectedActor.RemainAP)
                    _moveAdjcents[i] = null;
            }

            Action clear = () => {
                _moveAdjcents = null;
                _moveSelectedIndex = -1;
            };
            OnApply = () => {
                if (_moveSelectedIndex != -1 && _moveAdjcents[_moveSelectedIndex] != null)
                {
                    SelectedActor.MoveAct.Act(_moveAdjcents[_moveSelectedIndex]);
                    CommandRefocus();
                    OnCancel();
                }
            };
            OnCancel = () => {
                clear();
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
            OnNumeric = index => {
            };
        }

        private void StateSpeicalAct(int index)
        {
            if (SelectedActor?.SpecialActs == null)
                return;
            if (index < 0 || index >= SelectedActor.SpecialActs.Count)
                return;

            SelectedActor.SpecialActs[index].Act(null);
        }

        private void StateProductUI()
        {
            State = States.ProductUI;

            SelectedDeploy = -1;
            SelectedProduction = -1;
            IsProductCancelling = false;

            Action clear = () => {
                SelectedDeploy = -1;
                SelectedProduction = -1;
                IsProductCancelling = false;
            };
            OnApply = () => {
                if (IsProductCancelling)
                {
                    var node = Game.PlayerInTurn.Production.First;
                    for (int i = 0; i < SelectedProduction; ++i)
                        node = node.Next;
                    Game.PlayerInTurn.Production.Remove(node);

                    IsProductCancelling = false;
                    SelectedProduction = -1;
                }
                else if (SelectedDeploy != -1)
                {
                    var node = Game.PlayerInTurn.Deployment.First;
                    for (int i = 0; i < SelectedDeploy; ++i)
                        node = node.Next;
                    clear();
                    StateDeploy(node);
                }
                else if (SelectedProduction != -1)
                {
                    IsProductCancelling = true;
                }
                else
                {
                    clear();
                    StateProductAdd();
                }
            };
            OnCancel = () => {
                if (IsProductCancelling)
                {
                    IsProductCancelling = false;
                }
                else
                {
                    clear();
                    StateNormal();
                }
            };
            OnArrowKey = direction => {
                if (IsProductCancelling)
                    return;

                switch (direction)
                {
                    case Direction.Up:
                        if (SelectedProduction >= 0)
                        {
                            if (--SelectedProduction == -1)
                            {
                                if (Game.PlayerInTurn.Deployment.Count != 0)
                                    SelectedDeploy = 0;
                            }
                        }
                        else if (SelectedDeploy >= 0)
                        {
                            --SelectedDeploy;
                        }
                        break;
                    case Direction.Down:
                        if (SelectedProduction == -1)
                        {
                            if (++SelectedDeploy >= Game.PlayerInTurn.Deployment.Count)
                            {
                                SelectedDeploy = -1;
                                if (Game.PlayerInTurn.Production.Count != 0)
                                    SelectedProduction = 0;
                            }
                        }
                        else if (SelectedProduction + 1 < Game.PlayerInTurn.Production.Count)
                        {
                            ++SelectedProduction;
                        }
                        break;
                }
            };
            OnNumeric = index => {
                if (IsProductCancelling)
                    return;

                if (index < Game.PlayerInTurn.Deployment.Count)
                {
                    SelectedDeploy = index;
                    SelectedProduction = 0;
                }
            };
        }

        private void StateProductAdd()
        {
            State = States.ProductAdd;

            AvailableProduction = Game.PlayerInTurn.GetAvailableProduction();
            SelectedProduction = -1;

            Action clear = () => {
                AvailableProduction = null;
            };
            OnApply = () => {
                if (SelectedProduction != -1)
                {
                    var production = AvailableProduction[SelectedProduction].Create(Game.PlayerInTurn);
                    Game.PlayerInTurn.Production.AddLast(production);
                }
                OnCancel();
            };
            OnCancel = () => {
                clear();
                StateProductUI();
            };
            OnArrowKey = direction => {
                switch (direction)
                {
                    case Direction.Up:
                        if (SelectedProduction >= 0)
                            --SelectedProduction;
                        break;
                    case Direction.Down:
                        if (SelectedProduction + 1 < AvailableProduction.Count)
                            ++SelectedProduction;
                        break;
                }
            };
            OnNumeric = index => {
                if (index < AvailableProduction.Count)
                    SelectedProduction = index;
            };
        }

        private void StateDeploy(LinkedListNode<Production> node)
        {
            State = States.Deploy;

            DeployProduction = node.Value;

            Action clear = () => {
                DeployProduction = null;
            };
            OnApply = () => {
                if (DeployProduction.IsPlacable(FocusedPoint))
                {
                    Game.PlayerInTurn.Deployment.Remove(node);
                    DeployProduction.Place(FocusedPoint);
                    OnCancel();
                }
            };
            OnCancel = () => {
                clear();
                StateNormal();
            };
            OnArrowKey = direction => {
                MoveSight(direction);
            };
            OnNumeric = index => {
            };
        }
    }
}
