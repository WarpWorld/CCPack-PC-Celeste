using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    /// <summary>
    /// A request for information from the client.
    /// </summary>
    [Serializable]
    public class GameSelection : CrowdControlBlock
    {
        [CanBeNull, JsonProperty(PropertyName = "game")]
        public Game Game;

        [NotNull, JsonProperty(PropertyName = "channel")]
        public Channel Channel;

        [JsonProperty(PropertyName = "newSession")]
        public bool NewSession;

        /// <summary>
        /// The basic constructor for use in user code.
        /// </summary>
        /// <param name="game">The key corresponding to the desired value.</param>
        /// <param name="channel"></param>
        /// <param name="newSession"></param>
        public GameSelection([CanBeNull] Game game, [NotNull] Channel channel, bool newSession = true) : base(BlockType.GameSelection)
        {
            Game = game;
            Channel = channel;
            NewSession = newSession;
        }
    }
}
