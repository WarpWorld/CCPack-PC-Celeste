using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    public class EffectZoom: Effect
    {
        public override string Code { get; } = "zoom";

        public override EffectType Type { get; } = EffectType.Timed;

        public override TimeSpan Duration { get; } = TimeSpan.FromSeconds(30);

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Active || (!(Engine.Scene is Level level)) || (Player == null)) { return; }

            level.Camera.Zoom = 2;
            level.Camera.Approach(Player.Position, 0.1f);
        }

        public override void End()
        {
            base.End();
            if (!Active || (!(Engine.Scene is Level level)) || (Player == null)) { return; }

            level.Camera.Zoom = 1f;
        }
    }
}
