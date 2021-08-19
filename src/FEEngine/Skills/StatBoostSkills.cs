namespace FEEngine.Skills
{
    public class StrengthPlus2 : Skill
    {
        public override Unit.UnitStats StatBoosts => Unit.CreateStats(str: 2);
        public override string FriendlyName => "Str +2";
    }
    public class MagicPlus2 : Skill
    {
        public override Unit.UnitStats StatBoosts => Unit.CreateStats(mag: 2);
        public override string FriendlyName => "Mag +2";
    }
    public class DefensePlus2 : Skill
    {
        public override Unit.UnitStats StatBoosts => Unit.CreateStats(def: 2);
        public override string FriendlyName => "Def +2";
    }
    public class SpeedPlus2 : Skill
    {
        public override Unit.UnitStats StatBoosts => Unit.CreateStats(spd: 2);
        public override string FriendlyName => "Spd +2";
    }
}
