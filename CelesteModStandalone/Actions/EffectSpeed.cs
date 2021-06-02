using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    // ReSharper disable once UnusedMember.Global
    public class EffectSpeed : Effect
    {
        public override uint ID { get; } = 21;

        public override string Code { get; } = "speed";

        public override EffectType Type { get; } = EffectType.Timed;

        public override TimeSpan Duration { get; } = TimeSpan.FromSeconds(30);

        public override string[] Mutex { get; } = { "timerate" };

        public virtual float Rate { get; } = 2f;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Active || (!(Engine.Scene is Level))) { return; }

            Engine.TimeRate = Rate;
        }

        public override bool Stop()
        {
            base.Stop();
            Engine.TimeRate = 1f;
            return true;
        }
    }
}
