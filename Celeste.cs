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
                    new Effect("Oshiro", "oshiro"),
                    new Effect("Giant Oshiro", "oshiro_giant"),
                    new Effect("Seeker", "seeker"),
                    new Effect("Snowballs", "snowballs"),
                    new Effect("Badeline", "chaser"),
                    new Effect("Kill Player", "kill"),
                    new Effect("Wind", "wind"),
                    new Effect("Taunting Laughter", "laughter")
                };
                return effects;
            }
        }
    }
}
