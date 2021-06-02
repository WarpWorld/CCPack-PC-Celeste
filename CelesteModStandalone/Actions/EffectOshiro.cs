using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    // ReSharper disable once UnusedMember.Global
    public class EffectOshiro : Effect
    {
        public override uint ID { get; } = 4;

        public override string Code { get; } = "oshiro";

        public override EffectType Type { get; } = EffectType.Timed;

        public override TimeSpan Duration { get; } = TimeSpan.FromSeconds(30);

        public override string[] Mutex { get; } = { "oshiro" };

        public AngryOshiro Oshiro;

        public virtual AngryOshiro NewOshiro(Vector2 position) => new AngryOshiro(position, false);

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Active || (!(Engine.Scene is Level level)) || level.Entities.Contains(Oshiro) || level.Entities.GetToAdd().Contains(Oshiro)) { return; }

            Vector2 position = new Vector2(level.Bounds.Left - 32f, level.Bounds.Top + level.Bounds.Height / 2f);
            Oshiro = NewOshiro(position);
            level.Add(Oshiro);
        }

        public override bool Stop()
        {
            base.Stop();
            if ((Oshiro == null) || (!(Engine.Scene is Level level))) { return false; }

            level.Remove(Oshiro);
            Oshiro = null;
            return true;
        }
    }
}
