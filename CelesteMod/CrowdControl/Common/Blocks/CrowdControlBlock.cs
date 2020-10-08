using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace CrowdControl.Common
{
    public enum BlockType : byte
    {
        EffectRequest = 0x10,
        EffectResponse = 0x11,
        EffectReport = 0x12,

        DataRequest = 0x20,
        DataResponse = 0x21,

        RemotePackLoad = 0xD0,

        Initialization = 0xE0,
        GameSelection = 0xE1,

        KeepAlive = 0xF0,
        Command = 0xF1,
        Message = 0xF2,
        Version = 0xF3,
        Shutdown = 0xFE,
        Log = 0xFF
    }

    /// <summary>
    /// The base class for CrowdControl communication blocks.
    /// </summary>
    [Serializable]
    public abstract class CrowdControlBlock : IEffectPackProcessable
    {
        /// <summary>
        /// The block identifier. This only needs to be unique over a short period.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid ID = Guid.NewGuid();

        /// <summary>
        /// The block creation timestamp.
        /// </summary>
        [JsonProperty(PropertyName = "stamp"), JsonConverter(typeof(DateTimeOffsetLongConverter))]
        public DateTimeOffset Stamp = DateTimeOffset.UtcNow;

        /// <summary>
        /// The block type.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public BlockType BlockType;

        protected CrowdControlBlock(BlockType type) => BlockType = type;

        static CrowdControlBlock()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public static CrowdControlBlock Deserialize([NotNull] string s) => Deserialize(JToken.Parse(s));

        public static CrowdControlBlock Deserialize([NotNull] JToken tok) =>
            (BlockType)tok["type"].Value<byte>() switch
            {
                BlockType.EffectRequest => tok.ToObject<EffectRequest>(),
                BlockType.EffectResponse => tok.ToObject<EffectResponse>(),
                BlockType.EffectReport => tok.ToObject<EffectReport>(),
                BlockType.Initialization => tok.ToObject<InitializationBlock>(),
                BlockType.DataRequest => tok.ToObject<DataRequest>(),
                BlockType.DataResponse => tok.ToObject<DataResponse>(),
                BlockType.GameSelection => tok.ToObject<GameSelection>(),
                BlockType.KeepAlive => tok.ToObject<KeepAlive>(),
                BlockType.Message => tok.ToObject<MessageBlock>(),
                BlockType.Shutdown => tok.ToObject<ShutdownBlock>(),
#if NETCOREAPP
                BlockType.RemotePackLoad => tok.ToObject<LoadRemotePack>(),
#endif
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
