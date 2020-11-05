using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    public class EffectOshiro : Effect
    {
        public override string Code { get; } = "oshiro";

        public override EffectType Type { get; } = EffectType.Timed;

        public override TimeSpan Duration { get; } = TimeSpan.FromSeconds(30);

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

        public override void End()
        {
            base.End();
            if ((Oshiro == null) || (!(Engine.Scene is Level level))) { return; }

            level.Remove(Oshiro);
            Oshiro = null;
        }
    }

    public class EffectGiantOshiro : EffectOshiro
    {
        public override string Code { get; } = "oshiro_giant";

        private static readonly Vector2 SCALE = Vector2.One * 2;

        public override AngryOshiro NewOshiro(Vector2 position)
        {
            AngryOshiro result = new AngryOshiro(position, false) {Sprite = {Scale = SCALE} };
            return result;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Oshiro != null) { Oshiro.Sprite.Scale = SCALE; }
        }
    }
}
