using System;
using CrowdControl.Common;
using JetBrains.Annotations;
using ConnectorType = CrowdControl.Common.ConnectorType;

namespace CrowdControl.Games.Packs
{
    [UsedImplicitly]
    public class Celeste : SimpleTCPPack
    {
        public override string Host => "127.0.0.1";

        public override ushort Port => 58430;

        public override ISimpleTCPPack.MessageFormat MessageFormat => ISimpleTCPPack.MessageFormat.CrowdControlLegacy;

        public Celeste(UserRecord player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

        public override Game Game { get; } = new(5, "Celeste", "Celeste", "PC", ConnectorType.SimpleTCPConnector);

        public override EffectList Effects { get; } = new Effect[]
        {
            new Effect("Oshiro", "oshiro"){Duration = 30},
            new Effect("Giant Oshiro", "oshiro_giant"){Duration = 30},
            new Effect("Seeker", "seeker"){Duration = 30},
            new Effect("Snowballs", "snowballs"){Duration = 30},
            new Effect("Badeline", "chaser"){Duration = 30},
            new Effect("Kill Player", "kill"),
            new Effect("Reset Level", "reset"),
            new Effect("Wind", "wind"),
            new Effect("Taunting Laughter", "laughter"){Duration = 15},
            new Effect("Unlimited Dashes", "dashes"){Duration = 30},
            new Effect("Infinite Stamina", "stamina"){Duration = 30},
            new Effect("Invincibility", "invincible"){Duration = 30},
            new Effect("Invisibility", "invisible"){Duration = 30},
            new Effect("No Stamina", "nostamina"){Duration = 15},
            new Effect("Zoom Camera", "zoom"){Duration = 30},
            new Effect("Earthquake", "earthquake"){Duration = 30},
            new Effect("Speed Up Time", "speed"){Duration = 30},
            new Effect("Slow Down Time", "slow"){Duration = 30},
            new Effect("Hiccups", "hiccups"){Duration = 30},
            new Effect("Ice Physics", "icephysics"){Duration = 30},
            new Effect("Invert D-Pad", "invertdpad"){Duration = 15},
            new Effect("Flip Screen", "flipscreen"){Duration = 15},
            new Effect("Mirror World", "mirrorworld"){Duration = 30},
            //new Effect("No Gravity", "nogravity"){Duration = 30},

            new Effect("Player Sprite", "sprite", ItemKind.BidWar)
            {
                Parameters = new ParameterDef("Sprite", "sprite",
                    new Parameter("Badeline", "sprite_badeline"),
                    new Parameter("Madeline", "sprite_madeline")
                )
            },
        };
    }
}
