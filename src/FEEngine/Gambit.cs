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
        /// Represents a null <see cref="Gambit"/>; should not be passed to <see cref="Gambit(GambitType)"/>
        /// </summary>
        Null
    }
    /// <summary>
    /// An action to be attached to a <see cref="Battalion"/>
    /// </summary>
    public abstract class Gambit
    {
        public Gambit(GambitType gambitType)
        {
            mGambitType = gambitType;
        }
        /// <summary>
        /// The type of this gambit
        /// </summary>
        public GambitType GambitType => mGambitType;
        public bool IsGambitOffensive => mGambitType == GambitType.PhysicalAttack || mGambitType == GambitType.MagicAttack;
        internal void Use(Unit thisUnit, Unit targetUnit, Battalion battalion)
        {
            GambitArgs args = new()
            {
                ThisUnit = thisUnit,
                TargetUnit = targetUnit,
                Battalion = battalion
            };
            Use(args);
            thisUnit.CanMove = false;
        }
        protected abstract void Use(GambitArgs args);
        protected struct GambitArgs
        {
            public Unit ThisUnit { get; internal set; }
            public Unit TargetUnit { get; internal set; }
            public Battalion Battalion { get; internal set; }
        }
        public abstract int MaxUses { get; }
        public abstract Vector2 Range { get; }
        private readonly GambitType mGambitType;
    }
}
