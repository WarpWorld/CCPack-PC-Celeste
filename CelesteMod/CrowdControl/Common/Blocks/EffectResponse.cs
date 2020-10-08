using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    /// <summary>
    /// A response to an EffectRequest block.
    /// </summary>
    [Serializable]
    public class EffectResponse : ResponseBlock
    {
        /// <summary>
        /// True if the effect was processed successfully, otherwise false.
        /// </summary>
        [JsonIgnore]
        public bool Success => (((byte)Status) & 0x10) == 0x10;

        /// <summary>
        /// True if the effect was processed successfully, otherwise false.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public EffectStatus Status { get; }

        /// <summary>
        /// The due time of the effect, if applicable.
        /// </summary>
        [JsonProperty(PropertyName = "dueTime"), JsonConverter(typeof(DateTimeOffsetLongConverter))]
        public DateTimeOffset DueTime = DateTimeOffset.UtcNow;

        /// <summary>
        /// The message from the client (if any).
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "message")]
        public string Message { get; }

#if NET35
        public override string ToString() => $"{Request} - {Status + (string.IsNullOrEmpty(Message?.Trim()) ? string.Empty : (" - " + Message)) + (((((byte)Status) & 0x30) == 0x30) ? (" - " + DueTime.ToUniversalTime().ToString("O")) : string.Empty)}";
#else
        public override string ToString() => $"{Request} - {Status + (string.IsNullOrWhiteSpace(Message) ? string.Empty : (" - " + Message)) + (((((byte)Status) & 0x30) == 0x30) ? (" - " + DueTime.ToUniversalTime().ToString("O")) : string.Empty)}";
#endif

        /// <summary>
        /// The basic constructor for use in user code.
        /// </summary>
        /// <param name="request">The original request to which this response pertains.</param>
        /// <param name="status">The current status of the effect.</param>
        /// <param name="message">The message from the client (if any).</param>
        public EffectResponse(Guid request, EffectStatus status, [NotNull] string message) : base(BlockType.EffectResponse)
        {
            Request = request;
            Status = status;
            Message = message;
        }

        /// <summary>
        /// The basic constructor for use in user code.
        /// </summary>
        /// <param name="request">The original request to which this response pertains.</param>
        /// <param name="status">The current status of the effect.</param>
        /// <param name="dueTime">The due time for the status update.</param>
        /// <param name="message">The message from the client (if any).</param>
        public EffectResponse(Guid request, EffectStatus status, DateTimeOffset? dueTime, [NotNull] string message) : this(request, status, message)
            => DueTime = dueTime ?? DateTimeOffset.UtcNow;

        /// <summary>
        /// The basic constructor for use in user code.
        /// </summary>
        /// <param name="request">The original request to which this response pertains.</param>
        /// <param name="status">The current status of the effect.</param>
        /// <param name="dueTime">The due time for the status update, as a unix timestamp.</param>
        /// <param name="message">The message from the client (if any).</param>
        [JsonConstructor]
        protected EffectResponse(Guid request, EffectStatus status, long dueTime, [NotNull] string message) : this(request, status, message)
            => DueTime = dueTime.FromUnixTimeSeconds();
    }
}
