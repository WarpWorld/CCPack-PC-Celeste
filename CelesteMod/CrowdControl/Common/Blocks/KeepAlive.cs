using System;

namespace CrowdControl.Common
{
    /// <summary>
    /// A KeepAlive block that neither contains nor requests information or effects.
    /// </summary>
    [Serializable]
    public class KeepAlive : CrowdControlBlock
    {
        /// <summary>
        /// The basic constructor for use in user code.
        /// </summary>
        public KeepAlive() : base(BlockType.KeepAlive) { }
    }
}
