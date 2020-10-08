using System.ComponentModel;

namespace CrowdControl.Common
{
    public enum EffectStatus : byte
    {
        Unknown = 0x00,
        Created = 0x01,
        [Description("Pending Request")]
        PendingRequest = 0x02,
        [Description("Request Sent")]
        RequestSent = 0x03,
        Success = 0x10,
        [Description("Delayed Success")]
        SuccessDelayed = 0x11,
        [Description("Final Success")]
        SuccessFinal = 0x12,
        [Description("Bid War Success")]
        SuccessBidWar = 0x14,
        [Description("Temporary Failure")]
        FailTemporary = 0x20,
        [Description("Permanent Failure")]
        FailPermanent = 0x21,
        [Description("Bid War Failure")]
        FailBidWar = 0x24,
        [Description("Unknown Delay")]
        DelayUnknown = 0x30,
        [Description("Exact Delay")]
        DelayExact = 0x31,
        [Description("Estimated Delay")]
        DelayEstimated = 0x32,
        [Description("Extended Delay")]
        DelayExtended = 0x33,
        [Description("Bid War Delay")]
        DelayBidWar = 0x34,
        [Description("Timed Effect Begin")]
        TimedBegin = 0x40,
        [Description("Timed Effect End")]
        TimedEnd = 0x41,
        [Description("Connector Able to Perform")]
        ConnectorAble = 0x50,
        [Description("Connetor Unable to Perform")]
        ConnectorUnable = 0x51,
        [Description("Connetor Able to Perform Using Unsafe Methodology")]
        ConnectorRisky = 0x52,
        [Description("Available for Order")]
        MenuAvailable = 0x60,
        [Description("Unavailable for Order")]
        MenuUnavailable = 0x61,
        [Description("Visible on Menu")]
        MenuVisible = 0x62,
        [Description("Hidden on Menu")]
        MenuHidden = 0x63
    }
}
