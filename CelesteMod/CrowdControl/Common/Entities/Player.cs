using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    [Serializable]
    public class Player : IPlayer
    {
        [JsonProperty(PropertyName = "id")]
        public uint ID;

        [JsonProperty(PropertyName = "guid")]
        public Guid PlayerToken;

        [JsonProperty(PropertyName = "name")]
        public string Name;

        [JsonProperty(PropertyName = "twitchID")]
        public string TwitchID;

        [JsonProperty(PropertyName = "enabled")]
        public bool Enabled;

        uint IPlayer.ID => ID;
        Guid IPlayer.PlayerToken => PlayerToken;
        string IPlayer.Name => Name;
        string IPlayer.TwitchID => TwitchID;
        bool IPlayer.Enabled => Enabled;

        public Player() { }

        private Player([NotNull] string name) => Name = name;

        public Player([NotNull] string name, [NotNull] string twitchID) : this(name) => TwitchID = twitchID;

        public Player(uint id, Guid guid, [NotNull] string name, [NotNull] string twitchID, bool enabled = true)
        {
            ID = id;
            PlayerToken = guid;
            Name = name.ToLowerInvariant();
            TwitchID = twitchID;
            Enabled = enabled;
        }

        public Player([NotNull] IPlayer player) : this(player.ID, player.PlayerToken, player.Name, player.TwitchID, player.Enabled) { }

        public bool Equals(IPlayer other) => other?.ID.Equals(ID) ?? false;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is IPlayer)) return false;
            return Equals((IPlayer)obj);
        }

        public override int GetHashCode() => ID.GetHashCode();
    }
}
