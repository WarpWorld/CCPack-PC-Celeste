using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    [Serializable]
    public class BaseItem : IEquatable<BaseItem>
    {
        /// <summary>
        /// The item identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public uint? ID;

        /// <summary>
        /// The parent identifier.
        /// </summary>
        [JsonProperty(PropertyName = "parentID")]
        public uint? ParentID;

        /// <summary>
        /// The name of the effect.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "name")]
        public string Name;

        /// <summary>
        /// The effect code for use in commands.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "code")]
        public string Code;

        /// <summary>
        /// A long description of the associated effect.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "description")]
        public string Description = string.Empty;

        /// <summary>
        /// A long description of the associated effect.
        /// </summary>
        [CanBeNull, JsonProperty(PropertyName = "image")]
        public string Image;

        /// <summary>
        /// The game in which this item is used.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "game")]
        public Game Game;

        /// <summary>
        /// True if the item corresponds to a timed effect, otherwise false.
        /// </summary>
        [JsonProperty(PropertyName = "durational")]
        public bool Durational;

        /// <summary>
        /// True if this item is a microtransaction item, otherwise false.
        /// </summary>
        [JsonIgnore]
        public bool Micro => MaxQuantity > 1;

        /// <summary>
        /// The maximum sensible quantity for this item.
        /// </summary>
        [JsonProperty(PropertyName = "maxQuantity")]
        public uint MaxQuantity;

        [JsonProperty(PropertyName = "formula")]
        public FormulaBank.Formulas Formula = FormulaBank.Formulas.Sum;

        /// <summary>
        /// The kind of item.
        /// </summary>
        [JsonProperty(PropertyName = "kind")]
        public ItemKind Kind;

        /// <summary>
        /// The type of item.
        /// </summary>
        [CanBeNull, JsonProperty(PropertyName = "type")]
        public ItemType Type;

        /// <summary>
        /// The type of item.
        /// </summary>
        [CanBeNull, JsonProperty(PropertyName = "paramTypes")]
        public List<ItemType> ParamTypes;

        /// <summary>
        /// True if the item is currently marked as hidden, otherwise false.
        /// </summary>
        [JsonProperty(PropertyName = "hidden")]
        public bool Hidden;

        /*[NotNull, ItemNotNull]
        public List<Tag> Tags => Tag.LoadAll(this);*/
        public BaseItem() { }

        public BaseItem([NotNull] string name, [NotNull] string code, [NotNull] Game game)
        {
            Name = name;
            Code = code;
            Game = game;
        }

        /// <summary>
        /// Represents the object as a string.
        /// </summary>
        /// <returns>A string representing the object.</returns>
        public override string ToString() => $"{Name} ({Code})";

        public bool Equals(BaseItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ((ID != null) && (ID == other.ID)) || (string.Equals(Code, other.Code) && Game.Equals(other.Game));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BaseItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = ID?.GetHashCode() ?? -1;
                hashCode = (hashCode * 397) ^ Code.GetHashCode();
                hashCode = (hashCode * 397) ^ Game.GetHashCode();
                return hashCode;
            }
        }
    }
}
