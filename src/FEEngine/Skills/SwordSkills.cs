using FEEngine.Classes;

// descriptions taken from https://fireemblem.fandom.com/wiki/List_of_Skills_in_Fire_Emblem:_Three_Houses
namespace FEEngine.Skills
{
    /// <summary>
    /// Grants Hit/Avo +20 when using a sword against axe users.
    /// </summary>
    public class Axebreaker : BreakerSkillBase
    {
        public Axebreaker() : base(WeaponType.Sword, WeaponType.Axe) { }
        public override Unit.UnitStats StatBoosts => ClassUtil.CreateStatBoosts();
        public override string FriendlyName => nameof(Axebreaker);
    }
    /// <summary>
    /// Grants Crit +10 when using a sword.
    /// </summary>
    public class SwordCritPlus10 : CritPlus10Base
    {
        public SwordCritPlus10() : base(WeaponType.Sword) { }
        public override Unit.UnitStats StatBoosts => ClassUtil.CreateStatBoosts();
        public override string FriendlyName => "Sword Critical +10";
    }
    /// <summary>
    /// Grants Atk +5 when using a sword.
    /// </summary>
    public class Swordfaire : FaireSkillBase
    {
        public Swordfaire() : base(WeaponType.Sword) { }
        public override Unit.UnitStats StatBoosts => ClassUtil.CreateStatBoosts();
        public override string FriendlyName => nameof(Swordfaire);
    }
}
