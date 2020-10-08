using System;

namespace CrowdControl.Common
{
    /// <summary>
    /// A KeepAlive block that neither contains nor requests information or effects.
    /// </summary>
    [Serializable]
    public class ShutdownBlock : CrowdControlBlock
    {
        /// <summary>
        /// The basic constructor for use in user code.
        /// </summary>
        public ShutdownBlock() : base(BlockType.Shutdown) { }
    }
}
