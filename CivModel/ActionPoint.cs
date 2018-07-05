using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents an action point (AP) required by <see cref="IActorAction"/>.
    /// <see cref="Actor"/> must consume AP to do the action.
    /// </summary>
    public struct ActionPoint : IEquatable<ActionPoint>
    {
        /// <summary>
        /// The <see cref="ActionPoint"/> struct object representing non-available action,
        ///  with <see cref="Value"/> is <see cref="double.NaN"/>.
        /// </summary>
        public static readonly ActionPoint NonAvailable = new ActionPoint(double.NaN);

        /// <summary>
        /// The action point (AP). If the action cannot be done, <see cref="double.NaN"/>.
        /// </summary>
        public double Value { get; private set; }

        /// <summary>
        /// Whether the action consumes all AP of <see cref="Actor"/>.
        /// </summary>
        public bool IsConsumingAll { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionPoint"/> struct.
        /// </summary>
        /// <param name="value">The action point (AP). If the action cannot be done, <see cref="double.NaN"/>.</param>
        /// <param name="consumeAll">
        /// Whether the action consumes all AP of <see cref="Actor"/>.
        /// If <paramref name="value"/> is <see cref="double.NaN"/>, this parameter is ignored.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="value"/> is inf or -inf
        /// or
        /// <paramref name="value"/> is negative
        /// </exception>
        public ActionPoint(double value, bool consumeAll = false)
        {
            if (value < 0)
                throw new ArgumentException("value is negative", nameof(value));
            if (value == double.PositiveInfinity || value == double.NegativeInfinity)
                throw new ArgumentException("value is inf or -inf", nameof(value));

            Value = value;
            IsConsumingAll = (double.IsNaN(value) ? false : consumeAll);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="ActionPoint"/> to <see cref="System.Double"/>.
        /// </summary>
        /// <param name="point">The <see cref="ActionPoint"/>.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator double(ActionPoint point)
        {
            return point.Value;
        }
        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Double"/> to <see cref="ActionPoint"/>.
        /// </summary>
        /// <param name="value">The <see cref="double"/> value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ActionPoint(double value)
        {
            return new ActionPoint(value, false);
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(ActionPoint lhs, double rhs)
        {
            return !double.IsNaN(lhs.Value) && lhs.Value < rhs;
        }
        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(double lhs, ActionPoint rhs)
        {
            return rhs > lhs;
        }
        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <=(ActionPoint lhs, double rhs)
        {
            return !(lhs > rhs);
        }
        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <=(double lhs, ActionPoint rhs)
        {
            return !(rhs < lhs);
        }
        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(ActionPoint lhs, double rhs)
        {
            return double.IsNaN(lhs.Value) || lhs.Value > rhs;
        }
        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(double lhs, ActionPoint rhs)
        {
            return rhs < lhs;
        }
        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >=(ActionPoint lhs, double rhs)
        {
            return !(lhs < rhs);
        }
        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >=(double lhs, ActionPoint rhs)
        {
            return !(rhs > lhs);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (double.IsNaN(Value))
                return "NonAvailable";
            else if (IsConsumingAll)
                return Value.ToString() + " +";
            else
                return Value.ToString();
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ActionPoint lhs, ActionPoint rhs)
        {
            return lhs.Value == rhs.Value && lhs.IsConsumingAll == rhs.IsConsumingAll;
        }
        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ActionPoint lhs, ActionPoint rhs)
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
            return obj is ActionPoint ap && Equals(ap);
        }
        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
        /// </summary>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        /// <returns>
        /// 현재 개체가 <see langword="true" /> 매개 변수와 같으면 <paramref name="other" />이고, 그렇지 않으면 <see langword="false" />입니다.
        /// </returns>
        public bool Equals(ActionPoint other)
        {
            return (this == other);
        }
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = 869259987;
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            hashCode = hashCode * -1521134295 + IsConsumingAll.GetHashCode();
            return hashCode;
        }
    }
}
