using System.ComponentModel;

namespace CrowdControl.Common
{
    public enum ItemKind : byte
    {
        Effect = 0,
        [Description("Effect Parameter")]
        Usable = 1,
        Folder = 2,
        BidWar = 4,
        BidWarValue = 5,
        [Description("Hidden Folder")]
        HiddenFolder = 8
    }
}
