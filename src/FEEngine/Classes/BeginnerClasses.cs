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
        public static Unit.UnitStats CreateStatBoosts(int hp = 0, int str = 0, int mag = 0, int dex = 0, int spd = 0, int lck = 0, int def = 0, int res = 0, int cha = 0, int mv = 0)
        {
            return new()
            {
                HP = hp,
                Str = str,
                Mag = mag,
                Dex = dex,
                Spd = spd,
                Lck = lck,
                Def = def,
                Res = res,
                Cha = cha,
                Mv = mv
            };
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
            mStats = ClassUtil.CreateStatBoosts(spd: 2);
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
            mStats = ClassUtil.CreateStatBoosts(def: 2);
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
            mStats = ClassUtil.CreateStatBoosts(str: 2);
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
            mStats = ClassUtil.CreateStatBoosts(mag: 2);
        }
        public override MovementProperties MovementProperties => mMovementProperties;
        public override string Name => nameof(Monk);
        public override Unit.UnitStats StatBoosts => mStats;
        private MovementProperties mMovementProperties;
        private Unit.UnitStats mStats;
    }
}
