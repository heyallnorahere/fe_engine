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
        public SkillTriggerEvent Event { get; }
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
        public Type NewClass { get; }
    }
    /// <summary>
    /// This event is triggered when the <see cref="Unit"/> moves
    /// </summary>
    public class SkillMoveArgs : SkillEventArgs
    {
        internal SkillMoveArgs(IVec2<int> newPos, bool retreated) : base(SkillTriggerEvent.OnMove)
        {
            NewPosition = newPos;
            Retreated = retreated;
        }
        public IVec2<int> NewPosition { get; }
        public bool Retreated { get; }
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
        public Unit Enemy { get; }
        /// <summary>
        /// A reference to the attack's might
        /// </summary>
        public Ref<int>? Might { get; internal set; }
        /// <summary>
        /// A reference to the attack's hit rate
        /// </summary>
        public Ref<int>? HitRate { get; internal set; }
        /// <summary>
        /// A reference to the attack's critical rate
        /// </summary>
        public Ref<int>? CritRate { get; internal set; }
    }
    /// <summary>
    /// This event is triggered when <see cref="Unit.Wait"/> is called
    /// </summary>
    public class SkillWaitArgs : SkillEventArgs
    {
        internal SkillWaitArgs() : base(SkillTriggerEvent.OnWait) { }
    }
    public class SkillAfterExchangeArgs : SkillEventArgs
    {
        internal SkillAfterExchangeArgs(Unit other) : base(SkillTriggerEvent.AfterExchange)
        {
            OtherUnit = other;
        }
        public Unit OtherUnit { get; }
    }
    /// <summary>
    /// This event is triggered before a <see cref="Unit"/>'s stats are evaluated
    /// </summary>
    public class SkillBeforeStatEvaluationArgs : SkillEventArgs
    {
        internal SkillBeforeStatEvaluationArgs(Unit enemyUnit) : base(SkillTriggerEvent.BeforeStatEvaluation)
        {
            Enemy = enemyUnit;
        }
        /// <summary>
        /// The enemy that the unit is targeting; can be null
        /// </summary>
        public Unit Enemy { get; }
        /// <summary>
        /// The reference to the stats that will be evaluated; can be altered
        /// </summary>
        public Ref<Unit.UnitStats>? Stats { get; internal set; }
    }
    /// <summary>
    /// This event is triggered after a <see cref="Unit"/>'s stats are evaluated
    /// </summary>
    public class SkillAfterStatEvaluationArgs : SkillEventArgs
    {
        internal SkillAfterStatEvaluationArgs(Unit enemyUnit) : base(SkillTriggerEvent.AfterStatEvaluation)
        {
            Enemy = enemyUnit;
        }
        /// <summary>
        /// The enemy that the unit is targeting; can be null
        /// </summary>
        public Unit Enemy { get; }
        /// <summary>
        /// The reference to the evaluated stats; can be altered
        /// </summary>
        public Ref<Unit.EvaluatedUnitStats>? EvaluatedStats { get; internal set; }
    }
}
