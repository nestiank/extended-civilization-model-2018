using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivPresenter
{
    partial class Presenter
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
            /// In this state, <see cref="Presenter.RunningAction"/> is set to an action of move.
            /// </summary>
            Move,
            /// <summary>
            /// State [moving attack]. This state indicates user is giving a moving attack action command.
            /// <see cref="Presenter.CommandMovingAttack"/> method may introduce this state.
            /// In this state, <see cref="Presenter.RunningAction"/> is set to an action of moving attack.
            /// </summary>
            MovingAttack,
            /// <summary>
            /// State [holding attack]. This state indicates user is giving a holding attack action command.
            /// <see cref="Presenter.CommandHoldingAttack"/> method may introduce this state.
            /// In this state, <see cref="Presenter.RunningAction"/> is set to an action of holding attack.
            /// </summary>
            HoldingAttack,
            /// <summary>
            /// State [special act]. This state indicates user is giving a special action command.
            /// <see cref="Presenter.CommandNumeric(int)"/> method may introduce this state.
            /// In this state, <see cref="Presenter.RunningAction"/> is set to an action of special act,
            ///  and <see cref="Presenter.StateParam"/> indicates the index of special act.
            /// </summary>
            SpecialAct,
            /// <summary>
            /// State [product UI]. This state indicates user is viewing a production UI.
            /// <see cref="Presenter.CommandProductUI"/> method may introduce this state.
            /// In this state, <see cref="Presenter.SelectedDeploy"/> and <see cref="Presenter.SelectedProduction"/> indicate user's selection.
            /// If user is manipulating a selected production, <see cref="Presenter.IsProductManipulating"/> is set.
            /// </summary>
            ProductUI,
            /// <summary>
            /// State [product Add]. This state indicates user is viewing a production addition UI.
            /// This state may be introduced by <see cref="ProductUI"/> state.
            /// In this state, <see cref="Presenter.SelectedProduction"/> indicates user's selection,
            ///  and <see cref="Presenter.AvailableProduction"/> indicates the list of available productions to add.
            /// </summary>
            ProductAdd,
            /// <summary>
            /// State [deploy]. This state indicates user is giving a deployment command.
            /// This state may be introduced by <see cref="ProductUI"/> state.
            /// In this state, <see cref="Presenter.DeployProduction"/> indicates the production to deploy.
            /// </summary>
            Deploy,
            /// <summary>
            /// State [quest]. This state indicates user is viewing the list of quests.
            /// <see cref="Presenter.CommandQuest"/> method may introduce this state.
            /// In this state, asdf indicates asdf.
            /// </summary>
            Quest,
            /// <summary>
            /// State [victory]. This state indicates user is viewing a <strong>victory</strong> screen.
            /// </summary>
            Victory,
            /// <summary>
            /// State [defeated]. This state indicates user is viewing a <strong>defeated</strong> screen.
            /// </summary>
            Defeated,
            /// <summary>
            /// State [AI control]. This state indicates AI player is doing his job now.
            /// </summary>
            AIControl
        }
    }
}
