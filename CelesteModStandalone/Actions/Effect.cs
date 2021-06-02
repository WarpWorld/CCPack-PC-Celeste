using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using CrowdControl;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.CrowdControl.Actions
{
    public abstract class Effect : global::CrowdControl.Client.Binary.Effect
    {
        public TimeSpan Elapsed { get; set; }

        public virtual void Update(GameTime gameTime) => Elapsed += gameTime.ElapsedGameTime;

        public virtual void Draw(GameTime gameTime) { }

        public virtual string Group { get; }

        protected Player Player => CrowdControlHelper.Instance.Player;
        public override bool IsReady() => (Engine.Scene is Level) && (Player.Active);
    }
}