using System;
using System.Linq;
using CrowdControl;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    // ReSharper disable once UnusedMember.Global
    public class EffectChaser : Effect
    {
        public override uint ID { get; } = 1;

        public override string Code { get; } = "chaser";

        public override EffectType Type { get; } = EffectType.Timed;

        public override TimeSpan Duration { get; } = TimeSpan.FromSeconds(30);

        public BadelineOldsite Chaser;

        public AudioTrackState Music;

        public override void Load()
        {
            base.Load();
            On.Celeste.AudioState.Apply += OnAudioStateApply;
        }

        public override void Unload()
        {
            base.Unload();
            On.Celeste.AudioState.Apply -= OnAudioStateApply;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Player player = Player;
            if (!Active || !(Engine.Scene is Level level) || (player == null)) { return; }

            if (level.Entities.Contains(Chaser) || level.Entities.GetToAdd().Contains(Chaser)) { return; }

            Music = level.Session.Audio.Music.Clone();
            Chaser = new BadelineOldsite(Player.Position + new Vector2(0f, -8f), 0);
            level.Add(Chaser);
            //level.CancelCutscene();
            //Chaser.StartChasingRoutine(level);
        }

        public override bool Stop()
        {
            base.Stop();
            Level level = Engine.Scene as Level;
            if (level == null || Chaser == null) { return false; }

            Audio.Play("event:/char/badeline/disappear", Chaser.Position);

            level.Displacement.AddBurst(Chaser.Center, 0.5f, 24f, 96f, 0.4f);
            level.Particles.Emit(BadelineOldsite.P_Vanish, 12, Chaser.Center, Vector2.One * 6f);

            level.Remove(Chaser);
            Chaser = null;

            return true;
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
}
