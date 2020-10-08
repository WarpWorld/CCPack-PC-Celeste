using System;
using JetBrains.Annotations;

namespace CrowdControl.Common
{
    /// <summary>
    /// Provides an interface for the CrowdControl game server to implement for a player.
    /// </summary>
    public interface IPlayer : IEquatable<IPlayer>
    {
        uint ID { get; }
        Guid PlayerToken { get; }
        [NotNull] string Name { get; }
        [NotNull] string TwitchID { get; }
        bool Enabled { get; }
    }
}
