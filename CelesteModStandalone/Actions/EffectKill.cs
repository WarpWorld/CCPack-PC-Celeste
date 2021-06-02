using CrowdControl.Client.Binary;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    // ReSharper disable once UnusedMember.Global
    public class EffectKill : Effect
    {
        public override uint ID { get; } = 12;

        public override string Code { get; } = "kill";

        public override string[] Mutex { get; } = { "life" };

        public override bool Start(SchedulerContext context)
        {
            base.Start(context);
            if (!Active || (!(Engine.Scene is Level level)) || (!Player.Active)) { return false; }

            Player.Die(Vector2.Zero, true, true);
            return true;
        }
    }
}
