namespace Celeste.Mod.CrowdControl.Actions
{
    // ReSharper disable once UnusedMember.Global
    public class EffectSlow : EffectSpeed
    {
        public override uint ID { get; } = 19;

        public override string Code { get; } = "slow";

        public override float Rate { get; } = 0.5f;
    }
}
