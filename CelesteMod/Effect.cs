﻿using System;
using Monocle;

namespace Celeste.Mod.CrowdControl
{
    public abstract class Effect
    {
        public abstract string Code { get; }

        protected bool _active = false;
        private readonly object _activity_lock = new object();

        private TimeSpan _time_remaining = TimeSpan.Zero;

        protected Player Player => CrowdControlHelper.Instance.Player;

        public virtual EffectType Type { get; }

        public enum EffectType : byte
        {
            Instant = 0,
            Timed = 1
        }

        public bool Active
        {
            get => _active;
            private set
            {
                if (_active == value) { return; }
                _active = value;
                if (value) { Start(); }
                else { End(); }
            }
        }

        public virtual void Load() { }

        public virtual void Unload() { }

        public virtual void Start() { }

        public virtual void End() { }

        public virtual void Update() { }

        public virtual void Draw() { }

        public virtual bool IsReady() => (Engine.Scene is Level) && (Player.Active);

        public bool TryStart()
        {
            lock (_activity_lock)
            {
                if (Active || (!IsReady())) { return false; }
                Active = true;
                return true;
            }
        }

        public bool TryStart(TimeSpan duration)
        {
            _time_remaining = duration;
            return TryStart();
        }

        public bool TryStop()
        {
            lock (_activity_lock)
            {
                if (!Active) { return false; }
                Active = false;
                return true;
            }
        }
    }
}