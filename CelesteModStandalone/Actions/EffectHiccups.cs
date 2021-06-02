using System;
using CrowdControl.Client.Binary;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    // ReSharper disable once UnusedMember.Global
    public class EffectHiccups: Effect
    {
        public override uint ID { get; } = 6;

        public override string Code { get; } = "hiccups";

        public override EffectType Type { get; } = EffectType.Timed;

        public override TimeSpan Duration { get; } = TimeSpan.FromSeconds(30);

        private static readonly TimeSpan HICCUP_INTERVAL = TimeSpan.FromSeconds(2);
        private TimeSpan _last_hiccup = (-HICCUP_INTERVAL);

        public override bool Start(SchedulerContext context)
        {
            base.Start(context);
            _last_hiccup = (-HICCUP_INTERVAL);
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Active || (!(Engine.Scene is Level level)) || (Player == null)) { return; }

            if ((Elapsed - _last_hiccup) > HICCUP_INTERVAL)
            {
                _last_hiccup = Elapsed;
                Player.HiccupJump();
            }
        }
    }
}
