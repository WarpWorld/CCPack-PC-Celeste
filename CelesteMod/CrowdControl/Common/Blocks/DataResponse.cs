using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    /// <summary>
    /// A response to a DataRequest block.
    /// </summary>
    [Serializable]
    public class DataResponse : ResponseBlock
    {
        /// <summary>
        /// The key corresponding to the requested value.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "key")]
        public string Key;

        /// <summary>
        /// The value returned by the client.
        /// </summary>
        [CanBeNull, JsonProperty(PropertyName = "value")]
        public byte[] Value;

        /// <summary>
        /// True if the information was retrieved successfully, otherwise false.
        /// </summary>
        [JsonProperty(PropertyName = "success")]
        public bool Success;

        /// <summary>
        /// The message from the client (if any).
        /// </summary>
        [CanBeNull, JsonProperty(PropertyName = "message")]
        public string Message;

        /// <summary>
        /// The basic constructor for use in user code.
        /// </summary>
        /// <param name="key">The key corresponding to the requested value.</param>
        public DataResponse([NotNull] string key) : base(BlockType.DataResponse) => Key = key;
    }
}
