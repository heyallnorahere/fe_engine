namespace FEEngine.Classes
{
    internal partial class ClassUtil
    {
        public static MovementProperties DefaultBeginnerFootProperties
        {
            get
            {
                return new MovementProperties
                {
                    MovementType = MovementType.Infantry,
                    IsArmored = false
                };
            }
        }
    }
    /// <summary>
    /// Beginner sword class
    /// </summary>
    public class Myrmidon : Class
    {
        public Myrmidon()
        {
            mMovementProperties = ClassUtil.DefaultBeginnerFootProperties;
            mStats = Unit.CreateStats(spd: 2);
        }
        public override MovementProperties MovementProperties => mMovementProperties;
        public override string Name => nameof(Myrmidon);
        public override Unit.UnitStats StatBoosts => mStats;
        private MovementProperties mMovementProperties;
        private Unit.UnitStats mStats;
    }
    /// <summary>
    /// Beginner lance class
    /// </summary>
    public class Soldier : Class
    {
        public Soldier()
        {
            mMovementProperties = ClassUtil.DefaultBeginnerFootProperties;
            mStats = Unit.CreateStats(def: 2);
        }
        public override MovementProperties MovementProperties => mMovementProperties;
        public override string Name => nameof(Soldier);
        public override Unit.UnitStats StatBoosts => mStats;
        private MovementProperties mMovementProperties;
        private Unit.UnitStats mStats;
    }
    /// <summary>
    /// Beginner axe, bow, and gauntlet class
    /// </summary>
    public class Fighter : Class
    {
        public Fighter()
        {
            mMovementProperties = ClassUtil.DefaultBeginnerFootProperties;
            mStats = Unit.CreateStats(str: 2);
        }
        public override MovementProperties MovementProperties => mMovementProperties;
        public override string Name => nameof(Fighter);
        public override Unit.UnitStats StatBoosts => mStats;
        private MovementProperties mMovementProperties;
        private Unit.UnitStats mStats;
    }
    /// <summary>
    /// Beginner magic class
    /// </summary>
    public class Monk : Class
    {
        public Monk()
        {
            mMovementProperties = ClassUtil.DefaultBeginnerFootProperties;
            mStats = Unit.CreateStats(mag: 2);
        }
        public override MovementProperties MovementProperties => mMovementProperties;
        public override string Name => nameof(Monk);
        public override Unit.UnitStats StatBoosts => mStats;
        private MovementProperties mMovementProperties;
        private Unit.UnitStats mStats;
    }
}
