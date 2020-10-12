using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    public class EffectKill : Effect
    {
        public override string Code { get; } = "kill";

        public override void Start()
        {
            base.Start();
            if (!Active || (!(Engine.Scene is Level level)) || (!Player.Active)) { return; }

            Player.Die(Vector2.Zero, true, true);
        }
    }
}
