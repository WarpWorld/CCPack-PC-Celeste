using System;
using System.Collections.Generic;
using CrowdControl.Common.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    [Serializable]
    public class RemoteItem : IEquatable<RemoteItem>, IItem
    {
        /// <summary>
        /// The item identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public uint? ID;
        [JsonIgnore] uint? IItem.ID => ID;
        [JsonIgnore] uint? IItem.BaseID => ID;

        /// <summary>
        /// The item GUID.
        /// </summary>
        [JsonProperty(PropertyName = "guid")]
        public Guid Guid;

        /// <summary>
        /// The parent identifier.
        /// </summary>
        [JsonProperty(PropertyName = "parentID")]
        public uint? ParentID;
        [JsonIgnore] uint? IItem.ParentID => ParentID;

        /// <summary>
        /// The name of the effect.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "name")]
        public string Name;
        [JsonIgnore] string IItem.Name => Name;

        /// <summary>
        /// The effect code for use in commands.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "safeName")]
        public string SafeName;
        [JsonIgnore] string IItem.SafeName => SafeName;

        /// <summary>
        /// A long description of the associated effect.
        /// </summary>
        [NotNull, JsonProperty(PropertyName = "description")]
        public string Description = string.Empty;
        [JsonIgnore] string IItem.Description => Description;

        /// <summary>
        /// A long description of the associated effect.
        /// </summary>
        [JsonProperty(PropertyName = "image")]
        public uint Image;
        [JsonIgnore] string IItem.Image => ""; //uhhhhhhh

        /// <summary>
        /// The game in which this item is used.
        /// </summary>
        [JsonProperty(PropertyName = "game")]
        public uint GameID;
        [JsonIgnore] uint IItem.GameID => GameID;

        /// <summary>
        /// True if the item corresponds to a timed effect, otherwise false.
        /// </summary>
        [JsonProperty(PropertyName = "durational")]
        public bool Durational;
        [JsonIgnore] bool IItem.Durational => Durational;

        [JsonProperty(PropertyName = "formula")]
        public FormulaBank.Formulas Formula = FormulaBank.Formulas.Sum;
        [JsonIgnore] FormulaBank.Formulas IItem.Formula => Formula;

        [JsonProperty(PropertyName = "formulaExpression")]
        public string FormulaExpression;

        /// <summary>
        /// The kind of item.
        /// </summary>
        [JsonProperty(PropertyName = "kind")]
        public ItemKind Kind;
        [JsonIgnore] ItemKind IItem.Kind => Kind;

        /// <summary>
        /// The type of item.
        /// </summary>
        [CanBeNull, JsonProperty(PropertyName = "type")]
        public uint? Type;
        [JsonIgnore] uint? IItem.TypeID => Type;

        /// <summary>
        /// The type of item.
        /// </summary>
        [CanBeNull, JsonProperty(PropertyName = "paramTypes")]
        public List<ItemType> ParamTypes;

        [JsonProperty(PropertyName = "available")]
        public bool Available = true;
        [JsonIgnore] bool IItem.Available
        {
            get => Available;
            set => Available = value;
        }

        [JsonProperty(PropertyName = "hidden")]
        public bool Hidden = false;
        [JsonIgnore] bool IItem.Hidden
        {
            get => Hidden;
            set => Hidden = value;
        }

        [JsonIgnore] public bool Remote => true;

        [JsonProperty(PropertyName = "formulaVariableType")]
        public FormulaVariableType FormulaVariableType => FormulaVariableType.InventoryItem;

        long IFormulaVariable.Reduce() => BasePrice;
        string IFormulaVariable.FinalCode => SafeName;

        /// <summary>
        /// The per-viewer limit for this item.
        /// </summary>
        [JsonProperty(PropertyName = "userLimit")]
        public uint? UserLimit;
        [JsonIgnore] uint? IItem.UserLimit => UserLimit;

        /// <summary>
        /// The per-game limit for this item.
        /// </summary>
        [JsonProperty(PropertyName = "gameLimit")]
        public uint? GameLimit;
        [JsonIgnore] uint? IItem.GameLimit => GameLimit;

        /// <summary>
        /// The per-viewer cooldown time for this item.
        /// </summary>
        [JsonProperty(PropertyName = "userCooldown")]
        public uint? UserCooldown;
        [JsonIgnore] uint? IItem.UserCooldown => UserCooldown;

        /// <summary>
        /// The per-game cooldown time for this item.
        /// </summary>
        [JsonProperty(PropertyName = "gameCooldown")]
        public uint? GameCooldown;
        [JsonIgnore] uint? IItem.GameCooldown => GameCooldown;

        [JsonProperty(PropertyName = "scaleMode")]
        public InventoryItem.PriceScaleType ScaleMode { get; set; }
        [JsonIgnore] InventoryItem.PriceScaleType IItem.ScaleMode => ScaleMode;

        [JsonProperty(PropertyName = "scaleFactor")]
        public float ScaleFactor { get; set; }
        [JsonIgnore] float IItem.ScaleFactor => ScaleFactor;

        [JsonProperty(PropertyName = "scaleDecayTime")]
        public TimeSpan ScaleDecayTime { get; set; }
        [JsonIgnore] TimeSpan IItem.ScaleDecayTime => ScaleDecayTime;

        [JsonProperty(PropertyName = "scaleParent")]
        public uint? ScaleParent { get; set; }
        [JsonIgnore] uint? IItem.ScaleParent => ScaleParent;

        /// <summary>
        /// The base price of the effect.
        /// </summary>
        [JsonProperty(PropertyName = "price")]
        public uint BasePrice;
        [JsonIgnore] uint IItem.BasePrice => BasePrice;

#if NET35
        [JsonIgnore] IEnumerable<IItemType> IItem.ParamTypes => ParamTypes?.ToArray();
#else
        [JsonIgnore] IEnumerable<IItemType> IItem.ParamTypes => ParamTypes;
#endif

        /*[NotNull, ItemNotNull]
        public List<Tag> Tags => Tag.LoadAll(this);*/
        public RemoteItem() { }

        public RemoteItem(Guid guid, [NotNull] string name, [NotNull] string safeName, [NotNull] uint game)
        {
            Guid = guid;
            Name = name;
            SafeName = safeName;
            GameID = game;
        }

        /// <summary>
        /// Represents the object as a string.
        /// </summary>
        /// <returns>A string representing the object.</returns>
        public override string ToString() => $"{Name} ({SafeName})";

        public bool Equals(RemoteItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ((ID != null) && (ID == other.ID)) || (string.Equals(SafeName, other.SafeName) && GameID.Equals(other.GameID));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RemoteItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = ID?.GetHashCode() ?? -1;
                hashCode = (hashCode * 397) ^ GameID.GetHashCode();
                hashCode = (hashCode * 397) ^ SafeName.GetHashCode();
                return hashCode;
            }
        }
    }
}
