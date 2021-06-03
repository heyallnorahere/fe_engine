using System;

namespace FEEngine
{
    public enum SkillTriggerEvent
    {
        OnReclass,
        OnMove,
        OnAttack,
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SkillTriggerAttribute : Attribute
    {
        public SkillTriggerEvent TriggerEvent { get; set; }
        public SkillTriggerAttribute(SkillTriggerEvent @event)
        {
            TriggerEvent = @event;
        }
    }
    public abstract class Skill
    {
        internal static void Invoke(Skill skill, SkillTriggerEvent @event, Unit caller, SkillEventArgs eventArgs)
        {
            Type type = skill.GetType();
            Attribute[] attributes = Attribute.GetCustomAttributes(type);
            SkillTriggerAttribute skillTriggerAttribute = null;
            foreach (Attribute attribute in attributes)
            {
                if (attribute is SkillTriggerAttribute triggerAttribute)
                {
                    skillTriggerAttribute = triggerAttribute;
                    break;
                }
            }
            if (skillTriggerAttribute == null)
            {
                return;
            }
            if (@event == skillTriggerAttribute.TriggerEvent)
            {
                skill.Invoke(caller, eventArgs);
            }
        }
        protected virtual void Invoke(Unit caller, SkillEventArgs eventArgs) { }
        public abstract Unit.UnitStats StatBoosts { get; }
        public abstract string FriendlyName { get; }
    }
}
