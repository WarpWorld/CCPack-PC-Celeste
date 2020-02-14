using System;
using System.Collections.Generic;
using CrowdControl.Common;
using CrowdControl.Games.Merge;
using ConnectorType = CrowdControl.Common.ConnectorType;

namespace CrowdControl.Games.Packs
{
    public class Celeste : UnityMergePack
    {
        public override Game Game { get; } = new Game(5, "Celeste", "Celeste", "PC", ConnectorType.NullConnector);

        public override List<Effect> Effects
        {
            get
            {
                List<Effect> effects = new List<Effect>
                {
                    new Effect("Oshiro", "oshiro_normal"),
                    new Effect("Giant Oshiro", "oshiro_giant"),
                    new Effect("Seeker", "seeker"),
                    new Effect("Snowballs", "snowballs"),
                    new Effect("Badeline", "badeline"),
                    new Effect("Kill Player", "kill"),
                    new Effect("Wind", "wind"),
                    new Effect("Taunting Laughter", "laughter")
                };
                return effects;
            }
        }

        public override string Host => "127.0.0.1";
        public override ushort Port => 58430;

        public Celeste(IPlayer player, Func<CrowdControlBlock, bool> responseHandler, Action<object> statusUpdateHandler) : base(player, responseHandler, statusUpdateHandler) { }

        public override Type MergeType => typeof(CelesteMerge);
        public override string EmbedClass => "Celeste";
        public override string EmbedMethod => "Initialize";
        public override string AssemblyPath => "Celeste.exe";

        public class CelesteMerge : UnityMerge
        {
            protected override Response HandleEffect(Request req)
            {
                switch (req.code)
                {
                    case "oshiro":
                    {
                        //do something
                        return new Response { id = req.id, status = EffectResult.Success };
                    }
                }
                return new Response { id = req.id, status = EffectResult.Unavailable };
            }

            public override string Host => "127.0.0.1";
            public override ushort Port => 58430;
        }
    }
}
