using Celeste.Mod.UI;
using YamlDotNet.Serialization;

namespace Celeste.Mod.CrowdControl
{
    public class CrowdControlSettings : EverestModuleSettings
    {
        [SettingIgnore] // Ignore in the Mod Options.
        [YamlMember(Alias = "Enabled")] // Name of the property in the settings file.
        protected bool _Enabled { get; set; } = false;
        public bool Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled == value)
                    return;

                if (value)
                    CrowdControlHelper.Add();
                else
                    CrowdControlHelper.Remove();

                _Enabled = value;
            }
        }

        [SettingIgnore]
        [YamlMember(Alias = "ServiceKey")]
        public string ServiceKey { get; set; }

        //public string TestTest { get; set; }

        [YamlIgnore]
        public TextMenu.Button TempToken { get; protected set; }

        public void CreateTempTokenEntry(TextMenu menu, bool inGame)
        {
            string result = string.Empty;
            menu.Add(
                (TempToken = new TextMenu.Button("Enter Activation Code"))
                .Pressed(() => {
                    Audio.Play("event:/ui/main/savefile_rename_start");
                    menu.SceneAs<Overworld>().Goto<OuiModOptionString>().Init<OuiModOptions>(
                        string.Empty,
                        v => result = v,
                        s => { if (s) { CrowdControlHelper.Instance.OnUserKeyInput(result); } },
                        6, 8
                    );
                })
            );
            TempToken.Disabled = inGame || CrowdControlHelper.Instance.Connected;
        }
    }
}