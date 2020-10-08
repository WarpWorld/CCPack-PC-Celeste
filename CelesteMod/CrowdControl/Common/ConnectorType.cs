// ReSharper disable UnusedMember.Global
namespace CrowdControl.Common
{
    /// <remarks>
    /// MAKE SURE THE EQUIVALENT ENUMS IN CrowdControl.Community AND ELSEWHERE
    /// MATCH THIS CANONICAL ONE. THIS IS THE OFFICIAL ONE. - kat
    /// </remarks>
    public enum ConnectorType : byte
    {
        SNESConnector = 0x00,
        NESConnector = 0x01,
        GenesisConnector = 0x02,
        N64Connector = 0x03,
        NESProConnector = 0x04,
        GBConnector = 0x05,
        RESTConnector = 0x06,
        PCConnector = 0x07,
        PS1Connector = 0x08,
        GCNConnector = 0x09,
        WiiConnector = 0x0A,
        WiiUConnector = 0x0B,
        NullConnector = 0x0C,
        NamedPipeConnector = 0x0D,
        IRCClientConnector = 0x0E,
        IRCServerConnector = 0x0F,
        SimpleTCPConnector = 0x10,
        GBAConnector = 0x11,
        GameGearConnector = 0x12,
        ExternalConnector = 0xFF
    }
}
