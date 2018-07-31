using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The interface represents the path of <see cref="Actor"/> to move along.
    /// </summary>
    public interface IMovePath
    {
        /// <summary>
        /// The <see cref="Actor"/> to move along path, while acting <see cref="Action"/>.
        /// </summary>
        Actor Actor { get; }

        /// <summary>
        /// The start point of path.
        /// </summary>
        Terrain.Point StartPoint { get; }

        /// <summary>
        /// The end point of path.
        /// </summary>
        Terrain.Point EndPoint { get; }

        /// <summary>
        /// The list of points of path. If this path is invalid, this value is an empty collection.
        /// </summary>
        IEnumerable<Terrain.Point> Path { get; }

        /// <summary>
        /// The <see cref="IActorAction"/> of <see cref="Actor"/> to act at <see cref="EndPoint"/> of path.
        /// </summary>
        IActorAction FinalAction { get; }

        /// <summary>
        /// Whether path is invalid.
        /// Path is invalid if <see cref="Actor"/> is destroyed or no available path exists.
        /// </summary>
        bool IsInvalid { get; }

        /// <summary>
        /// Whether the first walk of path is invalid now.
        /// If this value is <c>true</c>, <see cref="ActFirstWalk"/> calls <see cref="RecalculateFirstWalk"/> before walk.
        /// </summary>
        bool IsFirstMoveInvalid { get; }

        /// <summary>
        /// Recalculates the first walk of path.
        /// </summary>
        void RecalculateFirstWalk();

        /// <summary>
        /// Make <see cref="Actor"/> move from <see cref="StartPoint"/> to the next point,
        ///  then set <see cref="StartPoint"/> to where <see cref="Actor"/> is.
        /// If necessary, calls <see cref="RecalculateFirstWalk"/> before walk.
        /// If the next point is <see cref="EndPoint"/>, make <see cref="Actor"/> act <see cref="FinalAction"/> instead of moving.
        /// </summary>
        /// <returns>Whether <see cref="RecalculateFirstWalk"/> is called before moving.</returns>
        /// <exception cref="InvalidOperationException">
        /// the path or the first walk of path is invalid
        /// </exception>
        /// <returns>Whether the walk is done or not. The walk is not done if AP is not enough.</returns>
        bool ActFirstWalk();
    }
}
