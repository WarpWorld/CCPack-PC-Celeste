using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    [Serializable]
    public class Effect : IEquatable<Effect>, IFormulaVariable
    {
        [NotNull, JsonProperty(PropertyName = "name")]
        public string Name;

        [NotNull, JsonProperty(PropertyName = "code")]
        public string Code;

        [CanBeNull, JsonProperty(PropertyName = "parent")]
        public string Parent;

        [JsonProperty(PropertyName = "quantity")]
        public uint Quantity;

        [JsonProperty(PropertyName = "kind")]
        public ItemKind Kind = ItemKind.Effect;

        [JsonProperty(PropertyName = "type")]
        public string Type;

        [CanBeNull, ItemNotNull, JsonProperty(PropertyName = "paramTypes")]
        public List<string> ParamTypes;

        [CanBeNull, ItemNotNull, JsonProperty(PropertyName = "paramValues")]
        public List<object> ParamValues;

        [JsonProperty(PropertyName = "formulaVariableType")]
        public FormulaVariableType FormulaVariableType => FormulaVariableType.Effect;

        public override string ToString() => $"{Name}{((Quantity > 1) ? $" [{Quantity}]" : string.Empty)}";

        public Effect() { }

        public Effect([NotNull] string name, [NotNull] string code, uint quantity = 1)
        {
            Name = name;
            Code = code;
            Quantity = quantity;
        }

        public Effect([NotNull] string name, [NotNull] string code, [NotNull] string parent, uint quantity = 1)
            : this(name, code, quantity) { Parent = parent; }

        public Effect([NotNull] string name, [NotNull] string code, ItemKind kind)
            : this(name, code) { Kind = kind; }

        public Effect([NotNull] string name, [NotNull] string code, ItemKind kind, [NotNull] string auto)
            : this(name, code, kind)
        {
            switch (kind)
            {
                case ItemKind.Effect:
                case ItemKind.Folder:
                case ItemKind.BidWar:
                case ItemKind.BidWarValue:
                    Parent = auto;
                    return;
                case ItemKind.Usable:
                    Type = auto;
                    return;
            }
        }

        public Effect([NotNull] string name, [NotNull] string code, [NotNull, ItemNotNull] IEnumerable<string> paramTypes)
            : this(name, code)
        {
            ParamTypes = new List<string>(paramTypes);
        }

        public Effect([NotNull] string name, [NotNull] string code, [NotNull, ItemNotNull] IEnumerable<string> paramTypes, [NotNull] string parent)
            : this(name, code)
        {
            ParamTypes = new List<string>(paramTypes);
            Parent = parent;
        }

        public Effect([NotNull] string name, [NotNull] string code, [NotNull, ItemNotNull] IEnumerable<string> paramTypes, [NotNull, ItemNotNull] IEnumerable<object> paramValues, uint quantity = 1)
            : this(name, code, paramTypes)
        {
            ParamValues = new List<object>(paramValues);
        }

        public Effect([NotNull] string name, [NotNull] string code, [NotNull, ItemNotNull] IEnumerable<string> paramTypes, [NotNull, ItemNotNull] IEnumerable<object> paramValues, [NotNull] string parent, uint quantity = 1)
            : this(name, code, paramTypes, parent)
        {
            ParamValues = new List<object>(paramValues);
        }

        public bool Equals(Effect other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Code, other.Code, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Effect)) return false;
            return Equals((Effect)obj);
        }

        public override int GetHashCode() => Code.GetHashCode();

        public long Reduce() => 1;

        public string FinalCode => Code;
    }
}
