// descriptions taken from https://fireemblem.fandom.com/wiki/List_of_Skills_in_Fire_Emblem:_Three_Houses
namespace FEEngine.Skills
{
    /// <summary>
    /// Grants Hit/Avo +20 when using a lance against sword users.
    /// </summary>
    public class Swordbreaker : BreakerSkillBase
    {
        public Swordbreaker() : base(WeaponType.Lance, WeaponType.Sword) { }
        public override Unit.UnitStats StatBoosts => Unit.CreateStats();
        public override string FriendlyName => nameof(Swordbreaker);
    }
    /// <summary>
    /// Grants Crit +10 when using a lance.
    /// </summary>
    public class LanceCritPlus10 : CritPlus10Base
    {
        public LanceCritPlus10() : base(WeaponType.Lance) { }
        public override Unit.UnitStats StatBoosts => Unit.CreateStats();
        public override string FriendlyName => "Lance Critical +10";
    }
    /// <summary>
    /// Grants Atk +5 when using a lance.
    /// </summary>
    public class Lancefaire : FaireSkillBase
    {
        public Lancefaire() : base(WeaponType.Lance) { }
        public override Unit.UnitStats StatBoosts => Unit.CreateStats();
        public override string FriendlyName => nameof(Lancefaire);
    }
}
