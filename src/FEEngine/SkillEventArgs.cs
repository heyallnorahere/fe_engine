using System;
using FEEngine.Math;

namespace FEEngine
{
    public abstract class SkillEventArgs
    {
        protected SkillEventArgs(SkillTriggerEvent @event)
        {
            Event = @event;
        }
        public SkillTriggerEvent Event { get; private set; }
    }
    public class SkillReclassArgs : SkillEventArgs
    {
        internal SkillReclassArgs(Type newClass) : base(SkillTriggerEvent.OnReclass)
        {
            NewClass = newClass;
        }
        public Type NewClass { get; private set; }
    }
    public class SkillMoveArgs : SkillEventArgs
    {
        internal SkillMoveArgs(IVec2<int> newPos) : base(SkillTriggerEvent.OnMove)
        {
            NewPosition = newPos;
        }
        public IVec2<int> NewPosition { get; private set; }
    }
    public class SkillAttackArgs : SkillEventArgs
    {
        internal SkillAttackArgs(Unit enemyUnit) : base(SkillTriggerEvent.OnAttack)
        {
            Enemy = enemyUnit;
        }
        public Unit Enemy { get; private set; }
        public Ref<int> Might { get; internal set; }
        public Ref<int> HitRate { get; internal set; }
        public Ref<int> CritRate { get; internal set; }
    }
}
