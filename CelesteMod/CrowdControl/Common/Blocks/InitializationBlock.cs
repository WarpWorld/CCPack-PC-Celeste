using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    /// <summary>
    /// A player initialization block. This class is not intended for normal game effects or data requests.
    /// </summary>
    [Serializable]
    public class InitializationBlock : CrowdControlBlock
    {
        /// <summary>
        /// The player's name.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "player")]
        public Player Player { get; }

        [CanBeNull, JsonProperty(PropertyName = "channel")]
        public Channel Channel { get; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; }

        [JsonProperty(PropertyName = "games")]
        public List<Game> Games { get; }

        /// <summary>
        /// The basic constructor for use with user code.
        /// </summary>
        /// <param name="player">The player identity.</param>
        /// <param name="channel">The session channel.</param>
        /// <param name="version"></param>
        /// <param name="games"></param>
        [JsonConstructor]
        public InitializationBlock([NotNull] Player player, [CanBeNull] Channel channel, string version, [NotNull, ItemNotNull] IEnumerable<Game> games) : base(BlockType.Initialization)
        {
            Player = new Player(player);
            Channel = channel;
            Version = version;
            Games = games.ToList();
        }
    }
}
