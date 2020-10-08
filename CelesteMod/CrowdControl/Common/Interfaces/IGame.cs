using JetBrains.Annotations;

namespace CrowdControl.Common.Interfaces
{
    public interface IGame
    {
        uint? ID { get; }
        [NotNull]
        string Name { get; }
        [NotNull]
        string SafeName { get; }
        bool Free { get; }
        bool Remote { get; }
    }
}
