﻿using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    // ReSharper disable once UnusedMember.Global
    public class EffectMirrorWorld : Effect
    {
        public override uint ID { get; } = 14;

        public override string Code { get; } = "mirrorworld";

        public override EffectType Type { get; } = EffectType.Timed;

        public override TimeSpan Duration { get; } = TimeSpan.FromSeconds(30);

        public override string[] Mutex { get; } = { "dpad", "screen" };

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Active || (!(Engine.Scene is Level level)) || (Player == null)) { return; }

            Input.MoveX.Inverted = true;
            Input.Aim.InvertedX = true;
            SaveData.Instance.Assists.MirrorMode = true;
        }

        public override bool Stop()
        {
            base.Stop();
            Input.MoveX.Inverted = false;
            Input.Aim.InvertedX = false;
            SaveData.Instance.Assists.MirrorMode = false;
            return true;
        }
    }
}
