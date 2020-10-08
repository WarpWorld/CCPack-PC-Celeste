using System;
using CrowdControl.Common.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    [Serializable]
    public class Game : IEquatable<Game>, IGame
    {
        [JsonProperty(PropertyName = "id")]
        public uint ID;
        [JsonIgnore] uint? IGame.ID => ID;

        [NotNull, JsonProperty(PropertyName = "name")]
        public string Name;
        [JsonIgnore] string IGame.Name => Name;

        [NotNull, JsonProperty(PropertyName = "safeName")]
        public string SafeName;
        [JsonIgnore] string IGame.SafeName => SafeName;

        [NotNull, JsonProperty(PropertyName = "path")]
        public string Path;

        [JsonProperty(PropertyName = "guide")]
        public uint Guide;

        [JsonProperty(PropertyName = "connector")]
        public ConnectorType Connector;
        //[JsonIgnore] ConnectorType IGame.Connector => Connector;

        [JsonProperty(PropertyName = "free")]
        public bool Free;
        [JsonIgnore] bool IGame.Free => Free;

        [JsonIgnore] bool IGame.Remote => false;

        public Game() { }

        public Game(uint id, [NotNull] string name, [NotNull] string safeName)
            : this(id,name,safeName,string.Empty,0, 0, false) { }

        public Game(uint id, [NotNull] string name, [NotNull] string safeName, [NotNull] string path, ConnectorType connector)
            : this(id, name, safeName, path, 0, connector, false) { }

        public Game(uint id, [NotNull] string name, [NotNull] string safeName, [NotNull] string path, uint guide, ConnectorType connector)
            : this(id, name, safeName, path, guide, connector, false) { }

        public Game(uint id, [NotNull] string name, [NotNull] string safeName, [NotNull] string path, uint guide, ConnectorType connector, bool free)
        {
            ID = id;
            Name = name;
            SafeName = safeName;
            Path = path;
            Guide = guide;
            Connector = connector;
            Free = free;
        }

        public override string ToString() => $"{Name} [{ID}]";

        public bool Equals(Game other) => other?.ID.Equals(ID) ?? false;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Game)obj);
        }

        public override int GetHashCode() => ID.GetHashCode();
    }
}
