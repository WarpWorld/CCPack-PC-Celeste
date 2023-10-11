using System;
using CrowdControl.Common;
using JetBrains.Annotations;
using ConnectorType = CrowdControl.Common.ConnectorType;

namespace CrowdControl.Games.Packs.Celeste;

[UsedImplicitly]
public class Celeste : SimpleTCPPack
{
    public override string Host => "127.0.0.1";

    public override ushort Port => 58430;

    public override ISimpleTCPPack.MessageFormat MessageFormat => ISimpleTCPPack.MessageFormat.CrowdControlLegacyIntermediate;

    public Celeste(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

    public override Game Game { get; } = new("Celeste", "Celeste", "PC", ConnectorType.SimpleTCPServerConnector);
    
    public override EffectList Effects { get; } = new Effect[]
    {
        new("Oshiro", "oshiro") { Duration = 30 },
        new("Giant Oshiro", "oshiro_giant") { Duration = 30 },
        new("Seeker", "seeker") { Duration = 30 },
        new("Snowballs", "snowballs") { Duration = 30 },
        new("Badeline", "chaser") { Duration = 30 },
        new("Kill Player", "kill"),
        new("Reset Level", "reset"),
        new("Wind", "wind") { Duration = 15 },
        new("Taunting Laughter", "laughter") { Duration = 15 },
        new("Unlimited Dashes", "dashes") { Duration = 30 },
        new("Infinite Stamina", "stamina") { Duration = 30 },
        new("Invincibility", "invincible") { Duration = 30 },
        new("Invisibility", "invisible") { Duration = 30 },
        new("No Stamina", "nostamina") { Duration = 15 },
        new("Zoom Camera", "zoom") { Duration = 30 },
        new("Earthquake", "earthquake") { Duration = 30 },
        new("Speed Up Time", "speed") { Duration = 30 },
        new("Slow Down Time", "slow") { Duration = 30 },
        new("Hiccups", "hiccups") { Duration = 30 },
        new("Ice Physics", "icephysics") { Duration = 30 },
        new("Invert D-Pad", "invertdpad") { Duration = 15 },
        new("Flip Screen", "flipscreen") { Duration = 15 },
        new("Mirror World", "mirrorworld") { Duration = 30 },

        new("Player Sprite", "sprite", ItemKind.BidWar)
        {
            Parameters = new ParameterDef("Sprite", "sprite",
                new Parameter("Badeline", "badeline"),
                new Parameter("Madeline", "madeline")
            )
        }
    };
}