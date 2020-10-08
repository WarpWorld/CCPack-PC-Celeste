using System;
using System.Collections.Generic;
using System.Linq;
using CrowdControl.Common.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    /// <summary>
    /// The Twitch channel object.
    /// </summary>
    [Serializable]
    public class ItemType : IEquatable<ItemType>, IItemType
    {
        /// <summary>
        /// The item identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public uint? ID;
        [JsonIgnore] uint? IItemType.ID => ID;

        /// <summary>
        /// The type name.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "name")]
        public string Name;
        [JsonIgnore] string IItemType.Name => Name;

        /// <summary>
        /// The type safe name.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "safeName")]
        public string SafeName;
        [JsonIgnore] string IItemType.SafeName => SafeName;

        /// <summary>
        /// The subtype of the parameter type.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public Subtype Type;
        [JsonIgnore] Subtype IItemType.Type => Type;

        /// <summary>
        /// The enumeration of type types.
        /// </summary>
        public enum Subtype : byte
        {
            ItemList = 0,
            Slider = 1
        }

        /// <summary>
        /// The type metadata.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "meta"), JsonConverter(typeof(NullStringConverter))]
        public string Meta;
        [JsonIgnore] string IItemType.Meta => Meta;

        /// <summary>
        /// Represents the object as a string.
        /// </summary>
        /// <returns>A string representing the object.</returns>
        public override string ToString() => Name;

        public ItemType([NotNull] string name, [NotNull] string safeName, Subtype type, [CanBeNull] string meta = null)
            : this(null, name, safeName, type, meta ?? string.Empty) { }

        [JsonConstructor]
        public ItemType([CanBeNull] uint? id, [NotNull] string name, [NotNull] string safeName, Subtype type, [NotNull] string meta)
        {
            ID = id ?? 0;
            Name = name;
            SafeName = safeName;
            Type = type;
            Meta = meta;
        }

        [NotNull, ItemNotNull]
        public static IEnumerable<IItemType> FromInventory([NotNull] IEnumerable<IItem> items)
        {
            List<IItemType> result = new List<IItemType>();
            foreach (IItem item in items)
            {
                result.AddRange(item.ParamTypes);
            }
            return result.Distinct();
        }

        public bool Equals(ItemType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(SafeName, other.SafeName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(ItemType)) return false;
            return Equals((ItemType)obj);
        }

        public override int GetHashCode() => SafeName.ToLowerInvariant().GetHashCode();
    }
}
