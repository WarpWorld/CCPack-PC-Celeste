using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    /// <summary>
    /// The Twitch channel object.
    /// </summary>
    [Serializable]
    public class Channel : IEquatable<Channel>
    {
        /// <summary>
        /// The channel identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public uint ID;

        /// <summary>
        /// The channel GUID identifier.
        /// </summary>
        [JsonProperty(PropertyName = "guid")]
        public Guid ChannelToken;

        /// <summary>
        /// The channel (streamer) name.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "name")]
        public string Name;

        /// <summary>
        /// The channel (streamer) twitch ID.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "twitchID")]
        public string TwitchID = string.Empty;

        /// <summary>
        /// The channel (streamer) twitch security token.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "twitchToken")]
        public volatile string TwitchToken = string.Empty;

        [JsonProperty(PropertyName = "twitchTokenExpiration")]
        public DateTimeOffset TwitchTokenExpiration = DateTimeOffset.MinValue;

        /// <summary>
        /// True if the channel is pro-subscribed, otherwise false.
        /// </summary>
        [JsonIgnore]
        public bool Pro = false;

        /// <summary>
        /// True if the channel is pro-subscribed, otherwise false.
        /// </summary>
        [JsonIgnore]
        public ChannelType BroadcasterType = ChannelType.Community;

        [Flags]
        public enum ChannelType : byte
        {
            Community = 0,
            Affiliate = 1,
            Partner = 2
        }

        /// <summary>
        /// True if the channel is enabled, otherwise false.
        /// </summary>
        [JsonProperty(PropertyName = "enabled")]
        public bool Enabled = false;

        /// <summary>
        /// Represents the object as a string.
        /// </summary>
        /// <returns>A string representing the object.</returns>
        public override string ToString() => Name;

        [JsonProperty(PropertyName = "promotionalCoins")]
        public PromotionalCoinsValues PromotionalCoins;

        [Serializable]
        public struct PromotionalCoinsValues
        {
            [JsonProperty(PropertyName = "free")]
            public uint Free;

            [JsonProperty(PropertyName = "tier1")]
            public uint Tier1;

            [JsonProperty(PropertyName = "tier2")]
            public uint Tier2;

            [JsonProperty(PropertyName = "tier3")]
            public uint Tier3;

#if NET471 || NETCOREAPP
            public static implicit operator (uint, uint, uint, uint)(PromotionalCoinsValues coins)
                => (coins.Free, coins.Tier1, coins.Tier2, coins.Tier3);

            public static implicit operator PromotionalCoinsValues((uint, uint, uint, uint) values)
                => new PromotionalCoinsValues
                {
                    Free = values.Item1,
                    Tier1 = values.Item2,
                    Tier2 = values.Item3,
                    Tier3 = values.Item4
                };
#endif
        }


        public bool Equals(Channel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Channel)obj);
        }

        public override int GetHashCode() => Name.ToLowerInvariant().GetHashCode();
    }
}
