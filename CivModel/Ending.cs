using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The type of <see cref="Ending"/>.
    /// </summary>
    public enum EndingType
    {
        /// <summary>
        /// The ending of victory.
        /// </summary>
        Victory,
        /// <summary>
        /// The ending of draw.
        /// </summary>
        Draw,
        /// <summary>
        /// The ending of defeat.
        /// </summary>
        Defeat
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IEquatable{Ending}" />
    public abstract class Ending : IEquatable<Ending>
    {
        /// <summary>
        /// The <see cref="CivModel.Game"/> object
        /// </summary>
        public Game Game { get; }

        /// <summary>
        /// The unique identifier of this object.
        /// </summary>
        public Guid Guid => _proto.Guid;

        /// <summary>
        /// The name of this ending.
        /// </summary>
        public string TextName => _proto.TextName;

        /// <summary>
        /// The type of this ending.
        /// </summary>
        public EndingType Type => _proto.Type;

        private EndingPrototype _proto;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ending"/> class.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="type">The concrete type of this object.</param>
        public Ending(Game game, Type type)
        {
            Game = game;
            _proto = game.GetPrototype<EndingPrototype>(type);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Ending lhs, Ending rhs)
        {
            return ReferenceEquals(lhs, rhs) || (lhs?.Game == rhs?.Game && lhs._proto == rhs._proto);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Ending lhs, Ending rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is Ending other ? this == other : false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = -290064877;
            hashCode = hashCode * -1521134295 + Game.GetHashCode();
            hashCode = hashCode * -1521134295 + _proto.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
        /// </summary>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        /// <returns>
        /// 현재 개체가 <see langword="true" /> 매개 변수와 같으면 <paramref name="other" />이고, 그렇지 않으면 <see langword="false" />입니다.
        /// </returns>
        public bool Equals(Ending other)
        {
            return this == other;
        }
    }
}
