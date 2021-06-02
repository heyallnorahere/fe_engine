namespace FEEngine.Classes
{
    internal partial class Defaults
    {
        public static MovementProperties DefaultBeginnerFootProperties
        {
            get
            {
                return new MovementProperties
                {
                    MovementType = MovementType.Foot,
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
            mMovementProperties = Defaults.DefaultBeginnerFootProperties;
        }
        public override MovementProperties MovementProperties => mMovementProperties;
        public override string Name => nameof(Myrmidon);
        private MovementProperties mMovementProperties;
    }
    /// <summary>
    /// Beginner lance class
    /// </summary>
    public class Soldier : Class
    {
        public Soldier()
        {
            mMovementProperties = Defaults.DefaultBeginnerFootProperties;
        }
        public override MovementProperties MovementProperties => mMovementProperties;
        public override string Name => nameof(Soldier);
        private MovementProperties mMovementProperties;
    }
    /// <summary>
    /// Beginner axe, bow, and gauntlet class
    /// </summary>
    public class Fighter : Class
    {
        public Fighter()
        {
            mMovementProperties = Defaults.DefaultBeginnerFootProperties;
        }
        public override MovementProperties MovementProperties => mMovementProperties;
        public override string Name => nameof(Fighter);
        private MovementProperties mMovementProperties;
    }
    /// <summary>
    /// Beginner magic class
    /// </summary>
    public class Monk : Class
    {
        public Monk()
        {
            mMovementProperties = Defaults.DefaultBeginnerFootProperties;
        }
        public override MovementProperties MovementProperties => mMovementProperties;
        public override string Name => nameof(Monk);
        private MovementProperties mMovementProperties;
    }
}
