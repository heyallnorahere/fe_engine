namespace FEEngine.Skills
{
    /// <summary>
    /// Grants Hit/Avo +20 when using an axe against lance users.
    /// </summary>
    public class Lancebreaker : BreakerSkillBase
    {
        public Lancebreaker() : base(WeaponType.Axe, WeaponType.Lance) { }
        public override Unit.UnitStats StatBoosts => Unit.CreateStats();
        public override string FriendlyName => nameof(Lancebreaker);
    }
    /// <summary>
    /// Grants Crit +10 when using an axe.
    /// </summary>
    public class AxeCritPlus10 : CritPlus10Base
    {
        public AxeCritPlus10() : base(WeaponType.Axe) { }
        public override Unit.UnitStats StatBoosts => Unit.CreateStats();
        public override string FriendlyName => "Axe Critical +10";
    }
    /// <summary>
    /// Grants Atk +5 when using an axe.
    /// </summary>
    public class Axefaire : FaireSkillBase
    {
        public Axefaire() : base(WeaponType.Axe) { }
        public override Unit.UnitStats StatBoosts => Unit.CreateStats();
        public override string FriendlyName => nameof(Axefaire);
    }
}
