namespace Celeste.Mod.CrowdControl.Actions
{
    public class EffectUnlimitedDashes : Effect
    {
        public override string Code { get; } = "dashes";

        public override void Update()
        {
            base.Update();
            Player.RefillDash();
        }
    }
}
