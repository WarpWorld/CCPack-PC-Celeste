using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    // ReSharper disable once UnusedMember.Global
    public class EffectFlipScreen : Effect
    {
        public override uint ID { get; } = 3;

        public override string Code { get; } = "flipscreen";

        public override EffectType Type { get; } = EffectType.Timed;

        public override TimeSpan Duration { get; } = TimeSpan.FromSeconds(15);

        public override string[] Mutex { get; } = { "screen" };

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Active || (!(Engine.Scene is Level level)) || (Player == null)) { return; }

            SaveData.Instance.Assists.MirrorMode = true;
        }

        public override bool Stop()
        {
            base.Stop();
            SaveData.Instance.Assists.MirrorMode = false;
            return true;
        }
    }
}
