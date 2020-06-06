using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    public class EffectOshiro : Effect
    {
        public override string Code { get; } = "oshiro";

        public AngryOshiro Oshiro;

        public virtual AngryOshiro NewOshiro(Vector2 position) => new AngryOshiro(position, false);

        public override void Load()
        {
            On.Celeste.AudioState.Apply += OnAudioStateApply;
        }

        public override void Unload()
        {
            On.Celeste.AudioState.Apply -= OnAudioStateApply;
        }

        public override void Start()
        {
        }

        public override void Update()
        {
            if (!Active || (!(Engine.Scene is Level level)) || level.Entities.Contains(Oshiro) || level.Entities.GetToAdd().Contains(Oshiro)) { return; }

            Vector2 position = new Vector2(level.Bounds.Left - 32f, level.Bounds.Top + level.Bounds.Height / 2f);
            Oshiro = NewOshiro(position);
            level.Add(Oshiro);
        }

        public override void End()
        {
            if ((Oshiro == null) || (!(Engine.Scene is Level level))) { return; }

            level.Remove(Oshiro);
            Oshiro = null;
        }

        public void OnAudioStateApply(On.Celeste.AudioState.orig_Apply orig, AudioState state)
        {
            // If we're about to play the oldsite chase song while we've got an old state clone,
            // undo the changes by reapplying the clone, then don't apply the changes.
            /*if (Music != null && state.Music.Event == Sfxs.music_oldsite_chase) {
                state.Music = Music;
                Music = null;
                return;
            }*/

            orig(state);
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

        public override void Update()
        {
            base.Update();
            if (Oshiro != null) { Oshiro.Sprite.Scale = SCALE; }
        }
    }
}
