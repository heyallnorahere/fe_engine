namespace FEEngine.Skills
{
    [SkillTrigger(SkillTriggerEvent.AfterStatEvaluation)]
    public abstract class BreakerSkillBase : Skill
    {
        public BreakerSkillBase(WeaponType weaponType, WeaponType opponentWeaponType)
        {
            mWeaponType = weaponType;
            mOpponentWeaponType = opponentWeaponType;
        }
        protected override void Invoke(Unit caller, SkillEventArgs eventArgs)
        {
            if (eventArgs.Event == SkillTriggerEvent.AfterStatEvaluation)
            {
                SkillAfterStatEvaluationArgs args = (SkillAfterStatEvaluationArgs)eventArgs;
                Item myWeapon = caller.EquippedWeapon;
                if (args.Enemy != null)
                {
                    Item otherWeapon = args.Enemy.EquippedWeapon;
                    if (myWeapon.WeaponStats.Type == mWeaponType && otherWeapon != null)
                    {
                        if (otherWeapon.WeaponStats.Type == mOpponentWeaponType)
                        {
                            args.EvaluatedStats.Value.Hit += 20;
                            args.EvaluatedStats.Value.Avo += 20;
                        }
                    }
                }
            }
        }
        private readonly WeaponType mWeaponType, mOpponentWeaponType;
    }
    [SkillTrigger(SkillTriggerEvent.AfterStatEvaluation)]
    public abstract class CritPlus10Base : Skill
    {
        public CritPlus10Base(WeaponType weaponType)
        {
            mWeaponType = weaponType;
        }
        protected override void Invoke(Unit caller, SkillEventArgs eventArgs)
        {
            if (eventArgs.Event == SkillTriggerEvent.AfterStatEvaluation)
            {
                SkillAfterStatEvaluationArgs args = (SkillAfterStatEvaluationArgs)eventArgs;
                Item myWeapon = caller.EquippedWeapon;
                if (myWeapon.WeaponStats.Type == mWeaponType)
                {
                    args.EvaluatedStats.Value.Crit += 10;
                }
            }
        }
        private readonly WeaponType mWeaponType;
    }
    [SkillTrigger(SkillTriggerEvent.AfterStatEvaluation)]
    public abstract class FaireSkillBase : Skill
    {
        public FaireSkillBase(WeaponType weaponType)
        {
            mWeaponType = weaponType;
        }
        protected override void Invoke(Unit caller, SkillEventArgs eventArgs)
        {
            if (eventArgs.Event == SkillTriggerEvent.AfterStatEvaluation)
            {
                SkillAfterStatEvaluationArgs args = (SkillAfterStatEvaluationArgs)eventArgs;
                Item myWeapon = caller.EquippedWeapon;
                if (myWeapon.WeaponStats.Type == mWeaponType)
                {
                    args.EvaluatedStats.Value.Atk += 5;
                }
            }
        }
        private readonly WeaponType mWeaponType;
    }
}
