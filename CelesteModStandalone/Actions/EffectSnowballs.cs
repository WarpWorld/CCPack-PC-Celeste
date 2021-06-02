using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    // ReSharper disable once UnusedMember.Global
    public class EffectSnowballs : Effect
    {
        public override uint ID { get; } = 20;

        public override string Code { get; } = "snowballs";

        public override EffectType Type { get; } = EffectType.Timed;

        public override TimeSpan Duration { get; } = TimeSpan.FromSeconds(30);

        public Snowball Snowball;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Active || (!(Engine.Scene is Level level)) || level.Entities.Contains(Snowball) || level.Entities.GetToAdd().Contains(Snowball)) { return; }

            Snowball = new Snowball();
            level.Add(Snowball);
        }

        public override bool Stop()
        {
            base.Stop();
            if ((Snowball == null) || (!(Engine.Scene is Level level))) { return false; }

            level.Remove(Snowball);
            Snowball = null;
            return true;
        }
    }
}
