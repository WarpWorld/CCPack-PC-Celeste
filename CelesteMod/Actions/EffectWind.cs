using System.Linq;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    public class EffectWind : Effect
    {
        public override string Code { get; } = "wind";

        public WindController Wind;

        public override void Update()
        {
            base.Update();
            if (!Active || (!(Engine.Scene is Level level)) || level.Entities.Contains(Wind) || level.Entities.GetToAdd().Contains(Wind)) { return; }

            Wind = new WindController(WindController.Patterns.Alternating);
            level.Add(Wind);
        }

        public override void End()
        {
            base.End();
            if ((Wind == null) || (!(Engine.Scene is Level level))) { return; }

            level.Remove(Wind);
            Wind = null;
        }
    }
}
