namespace Celeste.Mod.CrowdControl.Actions
{
    public class EffectInfiniteStamina : Effect
    {
        public override string Code { get; } = "stamina";

        public override void Update()
        {
            base.Update();
            Player.RefillStamina();
        }
    }
}
