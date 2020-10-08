using System;
using System.Threading;
using JetBrains.Annotations;
using Newtonsoft.Json;
#if !NET35

#endif

namespace CrowdControl.Common
{
    /// <summary>
    /// A CrowdControl viewer.
    /// </summary>
    [Serializable]
    public class Viewer : IEquatable<Viewer>
    {
        /// <summary>
        /// The viewer's username.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "name")]
        public string Name;

        [CanBeNull, JsonProperty(PropertyName = "displayName")]
        private string _display_name;

        /// <summary>
        /// The viewer's username.
        /// </summary>
        [NotNull, JsonIgnore]
        public string DisplayName
        {
#if NET35
            get => string.IsNullOrEmpty(_display_name?.Trim()) ? Name : _display_name;
#else
            get => string.IsNullOrWhiteSpace(_display_name) ? Name : _display_name;
#endif
            set => _display_name = value;
        }

        /// <summary>
        /// The channel to which this entry pertains.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "channel")]
        public Channel Channel;

        /// <summary>
        /// True if this viewer is the channel owner, otherwise false.
        /// </summary>
        [JsonIgnore]
        public bool IsOwner => string.Equals(Name, Channel.TwitchID, StringComparison.InvariantCultureIgnoreCase);

        [JsonProperty(PropertyName = "balance")]
        protected long _balance;

        /// <summary>
        /// The coin balance held by the viewer.
        /// </summary>
        [JsonIgnore] public long Balance => Math.Max(Interlocked.Read(ref _balance), 0);

        public long AddBalance(long amount) => Interlocked.Add(ref _balance, amount);

        public long EffectiveBalance => Balance;

        public Viewer() { }

        public Viewer([NotNull] string name, [NotNull] Channel channel, long balance)
        {
            Name = name.ToLowerInvariant();
            Channel = channel;
            _balance = Math.Max(balance, 0);
        }

        /// <summary>
        /// Compares the object for equality.
        /// </summary>
        /// <param name="other">The object to be compared.</param>
        /// <returns>True if the objects are equivalent, otherwise false.</returns>
        public bool Equals(Viewer other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase) && Channel.Equals(other.Channel);
        }

        /// <summary>
        /// Compares the object for equality.
        /// </summary>
        /// <param name="other">The object to be compared.</param>
        /// <returns>True if the objects are equivalent, otherwise false.</returns>
        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;
            return Equals((Viewer)other);
        }

        /// <summary>
        /// Gets a hash code for this object.
        /// </summary>
        /// <returns>A hash code for this object.</returns>
        public override int GetHashCode()
            => Name.GetHashCode() ^ Channel.GetHashCode();

        /// <summary>
        /// Represents the object as a string.
        /// </summary>
        /// <returns>A string representing the object.</returns>
        public override string ToString() => $"{Name} [{EffectiveBalance}]";
    }
}
