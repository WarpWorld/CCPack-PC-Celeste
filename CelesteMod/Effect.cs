namespace Celeste.Mod.CrowdControl
{
    public abstract class Effect
    {
        public abstract string Code { get; }

        protected bool _active = false;
        private readonly object _activity_lock = new object();

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

        public virtual bool IsReady() => true;

        public bool TryStart()
        {
            lock (_activity_lock)
            {
                if (Active || (!IsReady())) { return false; }
                Active = true;
                return true;
            }
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