using System;
using System.Collections.Generic;
using CrowdControl.Common.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    /// <summary>
    /// A CrowdControl item corresponding to an in-game effect.
    /// </summary>
    [Serializable]
    public class InventoryItem : IEquatable<InventoryItem>, IItem
    {
        /// <summary>
        /// The channel-local inventory identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public uint ID;
        [JsonIgnore] uint? IItem.ID => ID;

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

        [NotNull, JsonProperty(PropertyName = "baseItem")]
        public BaseItem BaseItem;
        [JsonIgnore] uint? IItem.BaseID => BaseItem.ID;
        [JsonIgnore] string IItem.Name => BaseItem.Name;
        [JsonIgnore] string IItem.SafeName => BaseItem.Code;
        [JsonIgnore] string IItem.Description => BaseItem.Description;
        [JsonIgnore] uint IItem.GameID => BaseItem.Game.ID;
        [JsonIgnore] ItemKind IItem.Kind => BaseItem.Kind;
        [JsonIgnore] string IItem.Image => BaseItem.Image;
        [JsonIgnore] uint? IItem.ParentID => BaseItem.ParentID;
        [JsonIgnore] FormulaBank.Formulas IItem.Formula => BaseItem.Formula;
        [JsonIgnore] uint? IItem.TypeID => BaseItem.Type?.ID;
        [JsonIgnore] bool IItem.Durational => BaseItem.Durational;

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

        [NotNull, JsonIgnore]
        public InventoryMenu Menu;

        /// <summary>
        /// The base price of the effect.
        /// </summary>
        [JsonProperty(PropertyName = "price")]
        public uint BasePrice;
        [JsonIgnore] uint IItem.BasePrice => BasePrice;

        long IFormulaVariable.Reduce() => BasePrice;

        [JsonIgnore] public string FinalCode => BaseItem.Code;

        public enum PriceScaleType : byte
        {
            None = 0x00,
            MultiplyPerUseDecayByFactor = 0x01,
            MultiplyPerUseDecayByHalf = 0x02,
            Inherit = 0x10
        }

        [JsonProperty(PropertyName = "scaleMode")]
        public PriceScaleType ScaleMode { get; set; }

        [JsonProperty(PropertyName = "scaleFactor")]
        public float ScaleFactor { get; set; }

        [JsonProperty(PropertyName = "scaleDecayTime")]
        public TimeSpan ScaleDecayTime { get; set; }

        [JsonProperty(PropertyName = "scaleParent")]
        public uint? ScaleParent { get; set; }

        [JsonIgnore] bool IItem.Remote => false;
#if NET35
        [JsonIgnore] IEnumerable<IItemType> IItem.ParamTypes => BaseItem.ParamTypes?.ToArray();
#else
        [JsonIgnore] IEnumerable<IItemType> IItem.ParamTypes => BaseItem.ParamTypes;
#endif

        [JsonProperty(PropertyName = "formulaVariableType")]
        public FormulaVariableType FormulaVariableType => FormulaVariableType.InventoryItem;

        [JsonConstructor]
        public InventoryItem([NotNull] BaseItem baseItem, [NotNull] InventoryMenu menu)
        {
            BaseItem = baseItem;
            Menu = menu;
        }

        /// <summary>
        /// Represents the object as a string.
        /// </summary>
        /// <returns>A string representing the object.</returns>
        public override string ToString() => $"{BaseItem.Name} ({BaseItem.Code}) [▲{BasePrice}]";

#if NET471 || NETCOREAPP
        public (bool success, string message) Validate(Channel channel)
        {
            if (channel.ID != Menu.Channel.ID) { return (false, "Channels do not match"); }
            if (!channel.Name.Equals(Menu.Channel.Name, StringComparison.InvariantCultureIgnoreCase)) { return (false, "Channels do not match"); }

            return (true, string.Empty);
        }

        public (bool success, string message) Validate([NotNull] InventoryMenu menu)
        {
            if (menu.ID != Menu.ID) { return (false, "Channels do not match"); }
            if (!menu.Name.Equals(Menu.Name, StringComparison.InvariantCultureIgnoreCase)) { return (false, "Menus do not match"); }

            return (true, string.Empty);
        }
#endif

        public bool Equals(InventoryItem other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return  (ID == other.ID) && Menu.Equals(other.Menu);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((InventoryItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ ID.GetHashCode();
                return hashCode;
            }
        }
    }
}
