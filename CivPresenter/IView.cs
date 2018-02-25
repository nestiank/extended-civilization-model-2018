using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivModel;

namespace CivPresenter
{
    /// <summary>
    /// Represents a direction.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Up direction
        /// </summary>
        Up,
        /// <summary>
        /// Down direction
        /// </summary>
        Down,
        /// <summary>
        /// Left direction
        /// </summary>
        Left,
        /// <summary>
        /// Right direction
        /// </summary>
        Right
    }

    /// <summary>
    /// The interface represents a View.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Called when view should refocus on <see cref="Presenter.FocusedPoint"/>
        /// </summary>
        void Refocus();

        /// <summary>
        /// Called when the game should be shutdown.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Invokes the specified action in the UI thread. This method must be thread-safe.
        /// </summary>
        /// <param name="action">The action.</param>
        void Invoke(Action action);
    }
}
