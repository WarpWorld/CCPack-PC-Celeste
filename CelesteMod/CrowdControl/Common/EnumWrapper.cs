using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace CrowdControl.Common
{
    /// <summary>
    /// This class wraps any enum for the purposes providing additional useful functionality.
    /// </summary>
    /// <typeparam name="T">Any enum type.</typeparam>
    public class EnumWrapper<T> : IEquatable<T>, IEquatable<EnumWrapper<T>> where T : struct
    {
        /// <summary>
        /// The raw value of the enumeration member.
        /// </summary>
        public T Value { get; }
        private readonly Type _type;

        /// <summary>
        /// Splits a flag enum into its constituent members.
        /// </summary>
        [NotNull]
        public IEnumerable<T> Flags => Enum.GetValues(typeof(T)).Cast<Enum>().Where(((Enum)((object)Value)).HasFlag).Cast<T>();

        /// <summary>
        /// Constructs a member of the EnumWrapper&lt;T&gt; class using the specified enum value.
        /// </summary>
        /// <param name="value">The enum value which this instance should describe.</param>
        private EnumWrapper(T value)
        {
            _type = value.GetType();
            if (!_type.IsEnum) { throw new ArgumentException("Value must be of Enum type.", nameof(value)); }
            Value = value;
        }

        /// <summary>
        /// A factory method for EnumWrapper&lt;T&gt;.
        /// </summary>
        /// <param name="value">The enum value which the returned instance should describe.</param>
        /// <returns>This function simply calls the EnumWrapper(T value) constructor.</returns>
        [NotNull]
        public static EnumWrapper<T> FromValue(T value) => new EnumWrapper<T>(value);

        /// <summary>
        /// Enumerates all members of an enum type and returns them all wrapped in EnumWrapper&lt;T&gt; objects.
        /// </summary>
        [NotNull]
        public static IEnumerable<EnumWrapper<T>> FromType()
        {
            if (!typeof(T).IsEnum) { throw new ArgumentException("Type must be of Enum type.", nameof(T)); }
            return Enum.GetValues(typeof(T)).Cast<T>().Select(next => (EnumWrapper<T>)next);
        }

        /// <summary>
        /// The total number of named elements in the enumeration.
        /// </summary>
        public static long Count => Enum.GetNames(typeof(T)).LongLength;

        /// <summary>
        /// A default equality comparer for members of T.
        /// </summary>
        /// <param name="other">The enum member to which this instance of EnumWrapper&lt;T&gt; should be compared.</param>
        /// <returns>True if other is equal to the Value property of this instance.</returns>
        public bool Equals(T other) => Value.Equals(other);

        /// <summary>
        /// A default equality comparer for members of Nullable&lt;T&gt;.
        /// </summary>
        /// <param name="other">The Nullable&lt;T&gt;-wrapped enum member to which this instance of EnumWrapper&lt;T&gt; should be compared.</param>
        /// <returns>True if other is equal to the Value property of this instance.</returns>
        private bool Equals(T? other) => Value.Equals(other);

        /// <summary>
        /// A default equality comparer for members of EnumWrapper&lt;T&gt;.
        /// </summary>
        /// <param name="other">The EnumWrapper&lt;T&gt;-wrapped enum member to which this instance of EnumWrapper&lt;T&gt; should be compared.</param>
        /// <returns>True if other is equal to the Value property of this instance.</returns>
        public bool Equals(EnumWrapper<T> other) => Equals(other?.Value);

        /// <summary>
        /// A default equality comparer.
        /// </summary>
        /// <param name="other">The object to which this instance of EnumWrapper&lt;T&gt; should be compared.</param>
        /// <returns>True if other is equal to the Value property of this instance.</returns>
        public override bool Equals(object other) => (Equals(other as T?) || Equals(other as EnumWrapper<T>));

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>An integer representing this instance's hash.</returns>
        /// <remarks>This value is equal to Value.GetHashCode().</remarks>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="o1">An EnumWrapper&lt;T&gt; object to compare to o2.</param>
        /// <param name="o2">Another object to compare to o1.</param>
        /// <returns>True if the values are equal, false otherwise.</returns>
        public static bool operator ==([CanBeNull] EnumWrapper<T> o1, [CanBeNull] object o2)
        {
            if (ReferenceEquals(o1, null) && ReferenceEquals(o2, null)) { return true; }
            if (!ReferenceEquals(o1, null) && !ReferenceEquals(o2, null)) { return o1.Equals(o2); }
            return false;
        }

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="o1">An EnumWrapper&lt;T&gt; object to compare to o2.</param>
        /// <param name="o2">Another object to compare to o1.</param>
        /// <returns>False if the values are equal, true otherwise.</returns>
        public static bool operator !=([CanBeNull] EnumWrapper<T> o1, [CanBeNull] object o2) => !(o1 == o2);

        /// <summary>
        /// An implicit conversion operator for conversion from EnumWrapper&lt;T&gt; to T.
        /// </summary>
        /// <param name="item">The EnumWrapper&lt;T&gt; to convert.</param>
        public static implicit operator T([NotNull] EnumWrapper<T> item) => item.Value;

        /// <summary>
        /// An implicit conversion operator for conversion from EnumWrapper&lt;T&gt; to T?.
        /// </summary>
        /// <param name="item">The EnumWrapper&lt;T&gt; to convert.</param>
        public static implicit operator T? ([CanBeNull] EnumWrapper<T> item) => item?.Value;

        /// <summary>
        /// An implicit conversion operator for conversion from T to EnumWrapper&lt;T&gt;.
        /// </summary>
        /// <param name="item">The member of T to convert.</param>
        [NotNull]
        public static implicit operator EnumWrapper<T>(T item) => new EnumWrapper<T>(item);

        /// <summary>
        /// Pretty-prints the enum member according to the member's Description attribute.
        /// </summary>
        /// <returns>A string representing the enum member.</returns>
        /// <remarks>If the enum member does not have a Description attribute, the member's name within the enum is used instead.</remarks>
        public override string ToString()
        {
            MemberInfo[] memberInfo = _type.GetMember(Value.ToString());
            if (memberInfo.Length <= 0) { return Value.ToString(); }

            object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attrs.Length <= 0) { return Value.ToString(); }

            return ((DescriptionAttribute)attrs[0]).Description;
        }
    }
}
