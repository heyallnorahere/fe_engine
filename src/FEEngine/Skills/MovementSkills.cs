using System;

// descriptions taken from https://fireemblem.fandom.com/wiki/List_of_Skills_in_Fire_Emblem:_Three_Houses
namespace FEEngine.Skills
{
    /// <summary>
    /// Allows unit to move again after completing certain actions, if there is movement remaining.
    /// </summary>
    [SkillTrigger(SkillTriggerEvent.AfterExchange)]
    [SkillTrigger(SkillTriggerEvent.OnWait)]
    public class Canto : Skill
    {
        internal struct UnitData
        {
            public bool EffectActive { get; set; }
        }
        public override Unit.UnitStats StatBoosts => Unit.CreateStats();
        public override string FriendlyName => nameof(Canto);
        protected override void Invoke(Unit caller, SkillEventArgs eventArgs)
        {
            ref object? attachedData = ref GetData(caller);
            if (attachedData == null)
            {
                attachedData = new UnitData
                {
                    EffectActive = false
                };
            }
            switch (eventArgs.Event)
            {
                case SkillTriggerEvent.AfterExchange:
                    {
                        if (attachedData is UnitData unitData)
                        {
                            unitData.EffectActive = true;
                            attachedData = unitData;
                        }
                    }
                    caller.RefreshMovement(true);
                    // todo: prevent the caller unit from acting
                    break;
                case SkillTriggerEvent.OnWait:
                    {
                        if (attachedData is UnitData unitData)
                        {
                            if (unitData.EffectActive)
                            {
                                unitData.EffectActive = false;
                            }
                        }
                    }
                    break;
                default:
                    throw new ArgumentException($"This skill was not subscribed to event: {eventArgs.Event}");
            }
        }
        public override bool CanAttack(Unit unit)
        {
            object? attachedData = GetData(unit);
            if (attachedData is UnitData unitData)
            {
                return !unitData.EffectActive;
            }
            else
            {
                return true;
            }
        }
    }
}
