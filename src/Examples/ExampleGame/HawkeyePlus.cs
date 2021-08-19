using FEEngine;

namespace ExampleGame
{
    /// <summary>
    /// This skill is from Awakening
    /// </summary>
    [SkillTrigger(SkillTriggerEvent.OnAttack)]
    public class HawkeyePlus : Skill
    {
        public override Unit.UnitStats StatBoosts => Unit.CreateStats();
        public override string FriendlyName => "Hawkeye+";
        protected override void Invoke(Unit caller, SkillEventArgs eventArgs)
        {
            if (eventArgs is SkillAttackArgs attackArgs)
            {
                this.VerifyValue(attackArgs.HitRate).Value = 100;
            }
        }
    }
}
