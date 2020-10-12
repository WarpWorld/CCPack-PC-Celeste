using System.Linq;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    public class EffectSnowballs : Effect
    {
        public override string Code { get; } = "snowballs";

        public Snowball Snowball;

        public override void Update()
        {
            base.Update();
            if (!Active || (!(Engine.Scene is Level level)) || level.Entities.Contains(Snowball) || level.Entities.GetToAdd().Contains(Snowball)) { return; }

            Snowball = new Snowball();
            level.Add(Snowball);
        }

        public override void End()
        {
            base.End();
            if ((Snowball == null) || (!(Engine.Scene is Level level))) { return; }

            level.Remove(Snowball);
            Snowball = null;
        }
    }
}
