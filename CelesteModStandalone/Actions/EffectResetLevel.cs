using CrowdControl.Client.Binary;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    // ReSharper disable once UnusedMember.Global
    public class EffectResetLevel : Effect
    {
        public override uint ID { get; } = 17;

        public override string Code { get; } = "reset";

        public override bool Start(SchedulerContext context)
        {
            base.Start(context);
            if (!Active || (!(Engine.Scene is Level level)) || (!Player.Active)) { return false; }

            SaveData.Instance.LastArea = AreaKey.None;
            SaveData.Instance.LastArea_Safe = AreaKey.None;
            if (level.StartPosition.HasValue) { Player.Position = level.StartPosition.Value; }
            Player.Die(Vector2.Zero, true, true);
            return true;
        }
    }
}