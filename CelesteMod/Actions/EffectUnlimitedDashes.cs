using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.CrowdControl.Actions
{
    public class EffectUnlimitedDashes : Effect
    {
        public override string Code { get; } = "dashes";

        public override EffectType Type { get; } = EffectType.Timed;

        public override TimeSpan Duration { get; } = TimeSpan.FromSeconds(30);

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Player.RefillDash();
        }
    }
}
