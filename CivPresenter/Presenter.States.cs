using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivPresenter
{
    public partial class Presenter
    {
        /// <summary>
        /// Indicates the state of <see cref="Presenter"/>.
        /// </summary>
        public enum States
        {
            /// <summary>
            /// State [normal]. This is a default state.
            /// </summary>
            Normal,
            /// <summary>
            /// State [move]. This state indicates user is giving a move action command.
            /// <see cref="Presenter.CommandMove"/> method may introduce this state.
            /// </summary>
            Move,
            /// <summary>
            /// State [moving attack]. This state indicates user is giving a moving attack action command.
            /// <see cref="Presenter.CommandMovingAttack"/> method may introduce this state.
            /// </summary>
            MovingAttack,
            /// <summary>
            /// State [holding attack]. This state indicates user is giving a holding attack action command.
            /// <see cref="Presenter.CommandHoldingAttack"/> method may introduce this state.
            /// </summary>
            HoldingAttack,
            /// <summary>
            /// State [special act]. This state indicates user is giving a special action command.
            /// <see cref="Presenter.CommandNumeric(int)"/> method may introduce this state.
            /// </summary>
            SpecialAct,
            /// <summary>
            /// State [product UI]. This state indicates user is viewing a production UI.
            /// <see cref="Presenter.CommandProductUI"/> method may introduce this state.
            /// </summary>
            ProductUI,
            /// <summary>
            /// State [product Add]. This state indicates user is viewing a production addition UI.
            /// This state may be introduced by <see cref="ProductUI"/> state.
            /// </summary>
            ProductAdd,
            /// <summary>
            /// State [deploy]. This state indicates user is giving a deployment command.
            /// This state may be introduced by <see cref="ProductUI"/> state.
            /// </summary>
            Deploy,
            /// <summary>
            /// State [victory]. This state indicates user is viewing a <strong>victory</strong> screen.
            /// </summary>
            Victory,
            /// <summary>
            /// State [defeated]. This state indicates user is viewing a <strong>defeated</strong> screen.
            /// </summary>
            Defeated
        }
    }
}
