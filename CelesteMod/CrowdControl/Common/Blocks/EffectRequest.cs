using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CrowdControl.Common.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;
using NotNullAttribute = JetBrains.Annotations.NotNullAttribute;

namespace CrowdControl.Common
{
    /// <summary>
    /// A request for the client to apply an effect.
    /// </summary>
    [Serializable]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class EffectRequest : CrowdControlBlock, IFormulaVariable
    {
        /// <summary>
        /// The CrowdControl game item corresponding to the desired effect.
        /// </summary>
        [NotNull, JsonIgnore]
        public IItem InventoryItem => (IItem)AllItems.First();

        /// <summary>
        /// The CrowdControl game items corresponding to any possible effect parameters.
        /// </summary>
        [NotNull, ItemNotNull, JsonIgnore]
        public IEnumerable<IFormulaVariable> ParameterItems => AllItems.Skip(1);

        /// <summary>
        /// All associated CrowdControl game items.
        /// </summary>
        [NotNull, ItemNotNull, JsonProperty(PropertyName = "items"), JsonConverter(typeof(FormulaVariableConverter))]
        public List<IFormulaVariable> AllItems;

        /// <summary>
        /// The viewers who requested the effect.
        /// </summary>
        [NotNull, ItemNotNull, JsonProperty(PropertyName = "viewers")]
        public List<Viewer> Viewers;

        /// <summary>
        /// The number of attempts it took to enqueue the item.
        /// </summary>
        [JsonProperty(PropertyName = "attemptCount")]
        public uint AttemptCount;

        /// <summary>
        /// Indicates that this request is a locally-generated test.
        /// </summary>
        [JsonIgnore]
        public bool Test;

        [JsonIgnore]
        public bool Queued;

        [JsonProperty(PropertyName = "elite")]
        public bool Elite;

        [JsonProperty(PropertyName = "anonymous")]
        public bool Anonymous;

        [JsonProperty(PropertyName = "cost")]
        public long Cost;

        [JsonProperty(PropertyName = "formulaVariableType")]
        public FormulaVariableType FormulaVariableType => FormulaVariableType.EffectRequest;

        public long Reduce() => FormulaBank.ApplyFormula(AllItems, InventoryItem.Formula);

        [JsonIgnore]
#if NET35
        public string FinalCode => string.Join("_", AllItems.Select(i => i.FinalCode).ToArray());
#else
        public string FinalCode => string.Join("_", AllItems.Select(i => i.FinalCode));
#endif

        [JsonIgnore]
        public string BaseCode => InventoryItem.SafeName;

        /// <summary>
        /// The string to display as the originator of the request.
        /// </summary>
        [NotNull, JsonIgnore]
        public string DisplayViewer
        {
            get
            {
                if (Viewers.Count != 1) { return "the crowd"; }
                return Viewers[0]?.DisplayName ?? "the crowd";
            }
        }

        /// <summary>
        /// The basic constructor for use in user code.
        /// </summary>
        /// <param name="item">The CrowdControl game item corresponding to the desired effect.</param>
        /// <param name="viewers">The viewers who requested the effect.</param>
        /// <param name="test">True if the request is a locally-generated test, otherwise false.</param>
        /// <param name="elite">True if the request is from an elite viewer, otherwise false.</param>
        /// <param name="anonymous">True if the request is from an anonymous viewer, otherwise false.</param>
        // ReSharper disable once SuggestBaseTypeForParameter
        public EffectRequest([NotNull] IItem item, [CanBeNull, ItemNotNull] IEnumerable<Viewer> viewers, bool test = false, bool elite = false, bool anonymous = false) : base(BlockType.EffectRequest)
        {
            AllItems = new List<IFormulaVariable> { item }; // make sure this item is first
            Viewers = viewers?.ToList() ?? new List<Viewer>();
            Test = test;
            Elite = elite;
            Anonymous = anonymous;
        }

        /// <summary>
        /// The basic constructor for use in user code.
        /// </summary>
        /// <param name="allItems">The CrowdControl game items corresponding to the desired effect.</param>
        /// <param name="viewers">The viewers who requested the effect.</param>
        /// <param name="test"></param>
        /// <param name="elite"></param>
        /// <param name="anonymous"></param>
        [JsonConstructor]
        public EffectRequest([NotNull] IEnumerable<IFormulaVariable> allItems, [CanBeNull, ItemNotNull] IEnumerable<Viewer> viewers, bool test = false, bool elite = false, bool anonymous = false) : base(BlockType.EffectRequest)
        {
            AllItems = new List<IFormulaVariable>(allItems);
            Viewers = viewers?.ToList() ?? new List<Viewer>();
            Test = test;
            Elite = elite;
            Anonymous = anonymous;
        }
    }
}
