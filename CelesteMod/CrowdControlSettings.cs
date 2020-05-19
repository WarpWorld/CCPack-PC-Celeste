using YamlDotNet.Serialization;

namespace Celeste.Mod.CrowdControl
{
    public class CrowdControlSettings : EverestModuleSettings
    {
        [SettingIgnore] // Ignore in the Mod Options.
        [YamlMember(Alias = "Enabled")] // Name of the property in the settings file.
        protected bool _Enabled { get; set; } = false;
        public bool Enabled {
            get => _Enabled;
            set {
                if (_Enabled == value)
                    return;

                if (value)
                    CrowdControlHelper.Add();
                else
                    CrowdControlHelper.Remove();

                _Enabled = value;
            }
        }

    }
}
