using FEEngine.Classes;

namespace FEEngine.Skills
{
    public class StrengthPlus2 : Skill
    {
        public override Unit.UnitStats StatBoosts => ClassUtil.CreateStatBoosts(str: 2);
        public override string FriendlyName => "Str +2";
    }
    public class MagicPlus2 : Skill
    {
        public override Unit.UnitStats StatBoosts => ClassUtil.CreateStatBoosts(mag: 2);
        public override string FriendlyName => "Mag +2";
    }
    public class DefensePlus2 : Skill
    {
        public override Unit.UnitStats StatBoosts => ClassUtil.CreateStatBoosts(def: 2);
        public override string FriendlyName => "Def +2";
    }
    public class SpeedPlus2 : Skill
    {
        public override Unit.UnitStats StatBoosts => ClassUtil.CreateStatBoosts(spd: 2);
        public override string FriendlyName => "Spd +2";
    }
}
