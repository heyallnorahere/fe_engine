// descriptions taken from https://fireemblem.fandom.com/wiki/List_of_Skills_in_Fire_Emblem:_Three_Houses
namespace FEEngine.Skills
{
    /// <summary>
    /// Allows unit to move again after completing certain actions, if there is movement remaining.
    /// </summary>
    [SkillTrigger(SkillTriggerEvent.AfterExchange)]
    public class Canto : Skill
    {
        public override Unit.UnitStats StatBoosts => Unit.CreateStats();
        public override string FriendlyName => nameof(Canto);
        protected override void Invoke(Unit caller, SkillEventArgs eventArgs)
        {
            caller.RefreshMovement(true);
            // todo: prevent the caller unit from acting
        }
    }
}
