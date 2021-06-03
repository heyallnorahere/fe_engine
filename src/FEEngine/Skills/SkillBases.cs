namespace FEEngine.Skills
{
    [SkillTrigger(SkillTriggerEvent.OnAttack)]
    public abstract class BreakerSkillBase : Skill
    {
        public BreakerSkillBase(WeaponType weaponType, WeaponType opponentWeaponType)
        {
            mWeaponType = weaponType;
            mOpponentWeaponType = opponentWeaponType;
        }
        protected override void Invoke(Unit caller, SkillEventArgs eventArgs)
        {
            if (eventArgs.Event == SkillTriggerEvent.OnAttack)
            {
                SkillAttackArgs attackArgs = (SkillAttackArgs)eventArgs;
                Item myWeapon = caller.EquippedWeapon;
                Item otherWeapon = attackArgs.Enemy.EquippedWeapon;
                if (myWeapon.WeaponStats.Type == mWeaponType && otherWeapon != null)
                {
                    if (otherWeapon.WeaponStats.Type == mOpponentWeaponType)
                    {
                        attackArgs.HitRate.Value += 20;
                        // todo: add 20 to avo stat, when it gets implemented
                    }
                }
            }
        }
        private readonly WeaponType mWeaponType, mOpponentWeaponType;
    }
    [SkillTrigger(SkillTriggerEvent.OnAttack)]
    public abstract class CritPlus10Base : Skill
    {
        public CritPlus10Base(WeaponType weaponType)
        {
            mWeaponType = weaponType;
        }
        protected override void Invoke(Unit caller, SkillEventArgs eventArgs)
        {
            if (eventArgs.Event == SkillTriggerEvent.OnAttack)
            {
                SkillAttackArgs attackArgs = (SkillAttackArgs)eventArgs;
                Item myWeapon = caller.EquippedWeapon;
                if (myWeapon.WeaponStats.Type == mWeaponType)
                {
                    attackArgs.CritRate.Value += 10;
                }
            }
        }
        private readonly WeaponType mWeaponType;
    }
    [SkillTrigger(SkillTriggerEvent.OnAttack)]
    public abstract class FaireSkillBase : Skill
    {
        public FaireSkillBase(WeaponType weaponType)
        {
            mWeaponType = weaponType;
        }
        protected override void Invoke(Unit caller, SkillEventArgs eventArgs)
        {
            if (eventArgs.Event == SkillTriggerEvent.OnAttack)
            {
                SkillAttackArgs attackArgs = (SkillAttackArgs)eventArgs;
                Item myWeapon = caller.EquippedWeapon;
                if (myWeapon.WeaponStats.Type == mWeaponType)
                {
                    attackArgs.Might.Value += 5;
                }
            }
        }
        private readonly WeaponType mWeaponType;
    }
}
