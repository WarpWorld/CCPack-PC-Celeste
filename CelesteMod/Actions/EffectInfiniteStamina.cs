﻿using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.CrowdControl.Actions
{
    public class EffectInfiniteStamina : Effect
    {
        public override string Code { get; } = "stamina";

        public override EffectType Type { get; } = EffectType.Timed;

        public override TimeSpan Duration { get; } = TimeSpan.FromSeconds(30);

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Player.RefillStamina();
        }
    }
}
