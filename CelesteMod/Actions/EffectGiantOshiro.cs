using Microsoft.Xna.Framework;

namespace Celeste.Mod.CrowdControl.Actions
{
    // ReSharper disable once UnusedMember.Global
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
