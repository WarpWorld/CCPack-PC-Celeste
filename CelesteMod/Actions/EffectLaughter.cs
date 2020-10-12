using System.Linq;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    public class EffectLaughter : Effect
    {
        public override string Code { get; } = "laughter";

        public GrannyLaughSfx Laughter;

        public override void Update()
        {
            base.Update();
            var lEntity = Laughter.Entity;
            if (!Active || (!(Engine.Scene is Level level)) || level.Entities.Contains(lEntity) || level.Entities.GetToAdd().Contains(lEntity)) { return; }

            level.Add((Laughter = new GrannyLaughSfx(Player.Sprite)).Entity);
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
