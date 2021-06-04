using System;

namespace FEEngine
{
    public enum SkillTriggerEvent
    {
        /// <summary>
        /// When a <see cref="Unit"/> is reclassed
        /// </summary>
        OnReclass,
        /// <summary>
        /// When a <see cref="Unit"/> moves (<see cref="Unit.Move(Math.IVec2{int}, Unit.MovementType)"/>)
        /// </summary>
        OnMove,
        /// <summary>
        /// When a <see cref="Unit"/> attacks another unit
        /// </summary>
        OnAttack,
        /// <summary>
        /// After two <see cref="Unit"/>s attack each other (an "exchange")
        /// </summary>
        AfterExchange,
    }
    /// <summary>
    /// Specifies when this <see cref="Skill"/> is to be called. Use multiple instances to call at multiple times
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SkillTriggerAttribute : Attribute
    {
        public SkillTriggerEvent TriggerEvent { get; set; }
        public SkillTriggerAttribute(SkillTriggerEvent triggerEvent)
        {
            TriggerEvent = triggerEvent;
        }
    }
    /// <summary>
    /// A skill to be attached to a <see cref="Unit"/> or <see cref="Class"/>
    /// </summary>
    public abstract class Skill
    {
        internal static void Invoke(Skill skill, SkillTriggerEvent @event, Unit caller, SkillEventArgs eventArgs)
        {
            Type type = skill.GetType();
            Attribute[] attributes = Attribute.GetCustomAttributes(type);
            bool canInvoke = false;
            foreach (Attribute attribute in attributes)
            {
                if (attribute is SkillTriggerAttribute triggerAttribute)
                {
                    if (triggerAttribute.TriggerEvent == @event)
                    {
                        canInvoke = true;
                        break;
                    }
                }
            }
            if (canInvoke)
            {
                skill.Invoke(caller, eventArgs);
            }
        }
        protected virtual void Invoke(Unit caller, SkillEventArgs eventArgs) { }
        /// <summary>
        /// Stats granted by this <see cref="Skill"/> (for example, Str +2)
        /// </summary>
        public abstract Unit.UnitStats StatBoosts { get; }
        /// <summary>
        /// The friendly name of this <see cref="Skill"/>
        /// </summary>
        public abstract string FriendlyName { get; }
    }
}
