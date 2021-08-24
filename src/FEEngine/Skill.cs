using System;
using System.Reflection;

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
        /// When a <see cref="Unit"/> waits
        /// </summary>
        OnWait,
        /// <summary>
        /// After two <see cref="Unit"/>s attack each other (an "exchange")
        /// </summary>
        AfterExchange,
        /// <summary>
        /// Before stats are evaluated
        /// </summary>
        BeforeStatEvaluation,
        /// <summary>
        /// After stats are evaluated
        /// </summary>
        AfterStatEvaluation
    }
    /// <summary>
    /// Specifies when this <see cref="Skill"/> is to be called. Use multiple instances to call at multiple times
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class SkillTriggerAttribute : Attribute
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
        public static Skill GetSkill(string name)
        {
            Type? skillType = Type.GetType(name);
            if (!(skillType?.DerivesFrom(typeof(Skill)) ?? false))
            {
                throw new ArgumentException("The given type does not derive from Skill!");
            }
            ConstructorInfo? constructor = skillType?.GetConstructor(Array.Empty<Type>());
            return (Skill)Extensions.VerifyValue(null, constructor?.Invoke(Array.Empty<object>()));
        }
        public static Skill GetSkill(Type type)
        {
            return GetSkill(Extensions.VerifyValue(null, type.AssemblyQualifiedName));
        }
        public static Skill GetSkill<T>() where T : Skill, new()
        {
            return GetSkill(typeof(T));
        }
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
        protected ref object? GetData(Unit unit)
        {
            return ref unit.GetAttachedData(GetType()).Data;
        }
        /// <summary>
        /// Stats granted by this <see cref="Skill"/> (for example, Str +2)
        /// </summary>
        public abstract Unit.UnitStats StatBoosts { get; }
        /// <summary>
        /// The friendly name of this <see cref="Skill"/>
        /// </summary>
        public abstract string FriendlyName { get; }
        /// <summary>
        /// Whether the <see cref="Unit"/> this skill is attached to can attack or not
        /// </summary>
        public virtual bool CanAttack(Unit unit) => true;
    }
}
