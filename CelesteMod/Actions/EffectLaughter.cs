using System;
using System.Linq;
using CrowdControl;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    public class EffectLaughter : Effect
    {
        public override string Code { get; } = "laughter";

        public override EffectType Type { get; } = EffectType.Timed;

        public override TimeSpan Duration { get; } = TimeSpan.FromSeconds(15);

        public GrannyLaughSfx Laughter;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var lEntity = Laughter?.Entity;
            if (!Active || (!(Engine.Scene is Level level)) || ((lEntity != null) && ((level.Entities.Contains(lEntity) || level.Entities.GetToAdd().Contains(lEntity))))) { return; }

            //level.Add((Laughter = new GrannyLaughSfx(Player.Sprite)).Entity);
            Laughter = new GrannyLaughSfx(Player.Sprite);
            Log.Message($"Laughter object is {(Laughter != null ? "NOT" : string.Empty)} null.");
            Log.Message($"Laughter entity is {(Laughter.Entity != null ? "NOT" : string.Empty)} null.");
        }

        public override void End()
        {
            base.End();
            if ((Laughter == null) || (!(Engine.Scene is Level level))) { return; }

            level.Remove(Laughter.Entity);
            Laughter = null;
        }
    }
}
