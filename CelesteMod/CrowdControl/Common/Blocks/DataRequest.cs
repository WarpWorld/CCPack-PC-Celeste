using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    /// <summary>
    /// A request for information from the client.
    /// </summary>
    [Serializable]
    public class DataRequest : CrowdControlBlock
    {
        /// <summary>
        /// The key corresponding to the desired value.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "key")]
        public string Key;

        /// <summary>
        /// The basic constructor for use in user code.
        /// </summary>
        /// <param name="key">The key corresponding to the desired value.</param>
        public DataRequest([NotNull] string key) : base(BlockType.DataRequest) => Key = key;
    }
}
