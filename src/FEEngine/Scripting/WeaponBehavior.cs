using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEEngine.Scripting
{
    public enum WeaponBehaviorEvent
    {
        /// <summary>
        /// After an attack is calculated
        /// </summary>
        OnCalculation,
        /// <summary>
        /// After two units attack each other
        /// </summary>
        AfterExchange
    }
    /// <summary>
    /// The object passed to a <see cref="IWeaponBehavior"/> on invoke
    /// </summary>
    public abstract class WeaponBehaviorArgs
    {
        public WeaponBehaviorArgs(WeaponBehaviorEvent @event, Unit attacker)
        {
            Event = @event;
            Attacker = attacker;
        }
        /// <summary>
        /// The event that was triggered
        /// </summary>
        public WeaponBehaviorEvent Event { get; private set; }
        /// <summary>
        /// The attacking unit
        /// </summary>
        public Unit Attacker { get; private set; }
    }
    /// <summary>
    /// A derived object of <see cref="WeaponBehaviorArgs"/> that is passed when the <see cref="WeaponBehaviorEvent.OnCalculation"/> event is triggered
    /// </summary>
    public sealed class WeaponOnCalculationArgs : WeaponBehaviorArgs
    {
        internal WeaponOnCalculationArgs(Unit attacker, Unit enemyUnit) : base(WeaponBehaviorEvent.OnCalculation, attacker)
        {
            Enemy = enemyUnit;
        }
        /// <summary>
        /// The attacked unit
        /// </summary>
        public Unit Enemy { get; private set; }
        /// <summary>
        /// The <see cref="Unit.AttackPacket"/> that will be processed; can be altered
        /// </summary>
        public Ref<Unit.AttackPacket> Packet { get; internal set; }
    }
    /// <summary>
    /// A derived object of <see cref="WeaponBehaviorArgs"/> that is passed when the <see cref="WeaponBehaviorEvent.AfterExchange"/> event is triggered
    /// </summary>
    public sealed class WeaponAfterExchangeArgs : WeaponBehaviorArgs
    {
        internal WeaponAfterExchangeArgs(Unit attacker, Unit enemyUnit) : base(WeaponBehaviorEvent.AfterExchange, attacker)
        {
            Enemy = enemyUnit;
            TimesAttacked = 0;
        }
        /// <summary>
        /// The attacked unit
        /// </summary>
        public Unit Enemy { get; private set; }
        /// <summary>
        /// Counts how many times this unit hit with a weapon
        /// </summary>
        public int TimesAttacked { get; internal set; }
    }
    /// <summary>
    /// Apply this attribute to your class to tell the engine when to invoke your behavior
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class WeaponBehaviorTriggerAttribute : Attribute
    {
        public WeaponBehaviorTriggerAttribute(WeaponBehaviorEvent @event)
        {
            Event = @event;
        }
        /// <summary>
        /// The trigger event
        /// </summary>
        public WeaponBehaviorEvent Event { get; private set; }
    }
    /// <summary>
    /// A behavior to attach to a weapon <see cref="Item"/>
    /// </summary>
    public abstract class WeaponBehavior
    {
        internal static void Invoke(WeaponBehaviorEvent @event, WeaponBehaviorArgs args, WeaponBehavior behavior)
        {
            if (behavior == null)
            {
                return;
            }
            Type behaviorType = behavior.GetType();
            Attribute[] attributes = Attribute.GetCustomAttributes(behaviorType);
            bool hasBeenTriggered = false;
            foreach (Attribute attribute in attributes)
            {
                if (attribute is WeaponBehaviorTriggerAttribute triggerAttribute)
                {
                    if (triggerAttribute.Event == @event)
                    {
                        hasBeenTriggered = true;
                        break;
                    }
                }
            }
            if (hasBeenTriggered)
            {
                behavior.Invoke(args);
            }
        }
        protected abstract void Invoke(WeaponBehaviorArgs args);
    }
}
