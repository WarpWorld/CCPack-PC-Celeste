using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    /// <summary>
    /// An update on the general state of an effect.
    /// </summary>
    [Serializable]
    public class EffectReport : CrowdControlBlock
    {
        /// <summary>
        /// The name of the effect to which this report pertains.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "effect")]
        public string Effect;

        /// <summary>
        /// True if the effect was processed successfully, otherwise false.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public EffectStatus Status { get; }

        /// <summary>
        /// The message from the client (if any).
        /// </summary>
        [CanBeNull, JsonProperty(PropertyName = "message")]
        public string Message;

#if NET35
        public override string ToString() => $"{Effect} - {Status + (string.IsNullOrEmpty(Message?.Trim()) ? string.Empty : (" - " + Message))}";
#else
        public override string ToString() => $"{Effect} - {Status + (string.IsNullOrWhiteSpace(Message) ? string.Empty : (" - " + Message))}";
#endif

        /// <summary>
        /// The basic constructor for use in user code.
        /// </summary>
        /// <param name="effect">The effect to which this information pertains.</param>
        /// <param name="status">The current status of the effect.</param>
        /// <param name="message">The message from the client (if any).</param>
        public EffectReport([NotNull] string effect, EffectStatus status, [CanBeNull] string message = null) : base(BlockType.EffectReport)
        {
            Effect = effect;
            Status = status;
            Message = message;
        }
    }
}
