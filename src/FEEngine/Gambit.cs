using System;

namespace FEEngine
{
    public enum GambitType
    {
        /// <summary>
        /// A physical attack
        /// </summary>
        PhysicalAttack,
        /// <summary>
        /// A magical attack
        /// </summary>
        MagicAttack,
        /// <summary>
        /// A support action (heal, rally, etc.)
        /// </summary>
        Support,
        /// <summary>
        /// Represents a null <see cref="Gambit"/>; should not be passed to <see cref="GambitActionAttribute"/>
        /// </summary>
        Null
    }
    /// <summary>
    /// Determines what type of <see cref="Gambit"/> the assigned class is
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class GambitActionAttribute : Attribute
    {
        public GambitActionAttribute(GambitType gambitType)
        {
            GambitType = gambitType;
        }
        public GambitType GambitType { get; private set; }
    }
    /// <summary>
    /// An action to be attached to a <see cref="Battalion"/>
    /// </summary>
    public abstract class Gambit
    {
        /// <summary>
        /// The type of this gambit
        /// </summary>
        public GambitType GambitType
        {
            get
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(GetType());
                foreach (Attribute attr in attributes)
                {
                    if (attr is GambitActionAttribute attribute)
                    {
                        return attribute.GambitType;
                    }
                }
                return GambitType.Null;
            }
        }
        internal void Use(Unit thisUnit, Unit targetUnit, Battalion battalion)
        {
            GambitArgs args = new()
            {
                ThisUnit = thisUnit,
                TargetUnit = targetUnit,
                Battalion = battalion
            };
            Use(args);
        }
        protected abstract void Use(GambitArgs args);
        protected struct GambitArgs
        {
            public Unit ThisUnit { get; internal set; }
            public Unit TargetUnit { get; internal set; }
            public Battalion Battalion { get; internal set; }
        }
        public abstract int MaxUses { get; }
    }
}
