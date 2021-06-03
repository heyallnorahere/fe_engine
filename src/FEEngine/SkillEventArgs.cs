using System;
using FEEngine.Math;

namespace FEEngine
{
    /// <summary>
    /// An object passed to a <see cref="Skill"/>
    /// </summary>
    public abstract class SkillEventArgs
    {
        protected SkillEventArgs(SkillTriggerEvent @event)
        {
            Event = @event;
        }
        /// <summary>
        /// The type of event that has been triggered
        /// </summary>
        public SkillTriggerEvent Event { get; private set; }
    }
    /// <summary>
    /// A reclass event
    /// </summary>
    public class SkillReclassArgs : SkillEventArgs
    {
        internal SkillReclassArgs(Type newClass) : base(SkillTriggerEvent.OnReclass)
        {
            NewClass = newClass;
        }
        public Type NewClass { get; private set; }
    }
    /// <summary>
    /// This event is triggered when the <see cref="Unit"/> moves
    /// </summary>
    public class SkillMoveArgs : SkillEventArgs
    {
        internal SkillMoveArgs(IVec2<int> newPos) : base(SkillTriggerEvent.OnMove)
        {
            NewPosition = newPos;
        }
        public IVec2<int> NewPosition { get; private set; }
    }
    /// <summary>
    /// This event is triggered when the <see cref="Unit"/> attacks or counterattacks
    /// </summary>
    public class SkillAttackArgs : SkillEventArgs
    {
        internal SkillAttackArgs(Unit enemyUnit) : base(SkillTriggerEvent.OnAttack)
        {
            Enemy = enemyUnit;
        }
        /// <summary>
        /// The <see cref="Unit"/> that is being attacked
        /// </summary>
        public Unit Enemy { get; private set; }
        /// <summary>
        /// A reference to the attack's might
        /// </summary>
        public Ref<int> Might { get; internal set; }
        /// <summary>
        /// A reference to the attack's hit rate
        /// </summary>
        public Ref<int> HitRate { get; internal set; }
        /// <summary>
        /// A reference to the attack's critical rate
        /// </summary>
        public Ref<int> CritRate { get; internal set; }
    }
}
