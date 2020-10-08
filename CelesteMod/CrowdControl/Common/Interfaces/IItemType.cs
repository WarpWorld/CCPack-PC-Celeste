using JetBrains.Annotations;

namespace CrowdControl.Common.Interfaces
{
    public interface IItemType
    {
        uint? ID { get; }
        [NotNull] string Name { get; }
        [NotNull] string SafeName { get; }
        ItemType.Subtype Type { get; }
        [NotNull] string Meta { get; }

    }
}
