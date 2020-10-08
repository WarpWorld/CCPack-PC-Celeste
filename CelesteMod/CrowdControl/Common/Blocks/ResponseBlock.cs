using System;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    public abstract class ResponseBlock : CrowdControlBlock
    {
        /// <summary>
        /// The original request to which this response pertains.
        /// </summary>
        [JsonProperty(PropertyName = "request")]
        public Guid Request;

        protected ResponseBlock(BlockType type) : base(type) { }
    }
}
