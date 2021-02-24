using System;
using System.Collections.Generic;
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

        public Celeste([NotNull] IPlayer player, [NotNull] Func<CrowdControlBlock, bool> responseHandler, [NotNull] Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

        public override Game Game { get; } = new Game(5, "Celeste", "Celeste", "PC", ConnectorType.SimpleTCPConnector);

        public override List<Effect> Effects
        {
            get
            {
                List<Effect> effects = new List<Effect>
                {
                    new Effect("Oshiro (30 seconds)", "oshiro"),
                    new Effect("Giant Oshiro (30 seconds)", "oshiro_giant"),
                    new Effect("Seeker (30 seconds)", "seeker"),
                    new Effect("Snowballs (30 seconds)", "snowballs"),
                    new Effect("Badeline (30 seconds)", "chaser"),
                    new Effect("Kill Player", "kill"),
                    new Effect("Reset Level", "reset"),
                    new Effect("Wind (30 seconds)", "wind"),
                    new Effect("Taunting Laughter (15 seconds)", "laughter"),
                    new Effect("Unlimited Dashes (30 seconds)", "dashes"),
                    new Effect("Infinite Stamina (30 seconds)", "stamina"),
                    new Effect("Invincibility (30 seconds)", "invincible"),
                    new Effect("Invisibility (30 seconds)", "invisible"),
                    new Effect("No Stamina (15 seconds)", "nostamina"),
                    new Effect("Zoom Camera (30 seconds)", "zoom"),
                    new Effect("Earthquake (30 seconds)", "earthquake"),
                    new Effect("Speed Up Time (30 seconds)", "speed"),
                    new Effect("Hiccups (30 seconds)", "hiccups"),
                    new Effect("Ice Physics (30 seconds)", "icephysics"),
                    new Effect("Invert D-Pad (15 seconds)", "invertdpad"),
                    new Effect("Flip Screen (15 seconds)", "flipscreen"),
                    new Effect("Mirror World (30 seconds)", "mirrorworld"),
                    //new Effect("No Gravity (30 seconds)", "nogravity"),

                    new Effect("Player Sprite", "sprite", ItemKind.BidWar),
                    new Effect("Badeline", "sprite_badeline", ItemKind.BidWarValue, "sprite"),
                    new Effect("Madeline", "sprite_madeline", ItemKind.BidWarValue, "sprite")
                };
                return effects;
            }
        }
    }
}
