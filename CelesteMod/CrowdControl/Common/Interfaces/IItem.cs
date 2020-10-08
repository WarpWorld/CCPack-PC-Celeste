using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CrowdControl.Common.Interfaces
{
    public interface IItem : IFormulaVariable
    {
        uint? ID { get; }
        uint? BaseID { get; }
        uint GameID { get; }
        [NotNull] string Name { get; }
        [NotNull] string SafeName { get; }
        [NotNull] string Description { get; }
        [CanBeNull] string Image { get; }
        uint? ParentID { get; }
        uint BasePrice { get; }
        bool Durational { get; }
        ItemKind Kind { get; }
        FormulaBank.Formulas Formula { get; }
        uint? TypeID { get; }
        uint? UserLimit { get; }
        uint? GameLimit { get; }
        uint? UserCooldown { get; }
        uint? GameCooldown { get; }
        InventoryItem.PriceScaleType ScaleMode { get; }
        float ScaleFactor { get; }
        uint? ScaleParent { get; }
        TimeSpan ScaleDecayTime { get; }
        IEnumerable<IItemType> ParamTypes { get; }
        bool Available { get; set; }
        bool Hidden { get; set; }
        bool Remote { get; }
    }
}
