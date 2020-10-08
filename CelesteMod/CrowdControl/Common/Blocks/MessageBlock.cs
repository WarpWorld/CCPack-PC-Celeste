using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    /// <summary>
    /// A simple message block.
    /// </summary>
    [Serializable]
    public class MessageBlock : CrowdControlBlock
    {
        /// <summary>
        /// Who the chat message is from.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "from")]

        public string From;

        /// <summary>
        /// The message contents.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "message")]

        public string Message;

        /// <summary>
        /// Whether or not the message should cause an alert to occur.
        /// </summary>
        [JsonProperty(PropertyName = "alert")] public bool Alert;

        /// <summary>
        /// The basic constructor for use in user code.
        /// </summary>
        /// <param name="from">The key corresponding to the requested value.</param>
        public MessageBlock([NotNull] string from, [NotNull] string message, bool alert = false) : base(BlockType.Message)
        {
            From = from;
            Message = message;
            Alert = alert;
        }
    }
}
