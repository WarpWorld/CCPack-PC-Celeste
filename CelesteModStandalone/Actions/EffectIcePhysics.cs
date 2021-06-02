using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    // ReSharper disable once UnusedMember.Global
    public class EffectIcePhysics : Effect
    {
        public override uint ID { get; } = 7;

        public override string Code { get; } = "icephysics";

        public override EffectType Type { get; } = EffectType.Timed;

        public override TimeSpan Duration { get; } = TimeSpan.FromSeconds(30);

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Active || (!(Engine.Scene is Level level)) || (Player == null)) { return; }

            SaveData.Instance.Assists.LowFriction = true;
        }

        public override bool Stop()
        {
            base.Stop();
            SaveData.Instance.Assists.LowFriction = false;
            return true;
        }
    }
}
