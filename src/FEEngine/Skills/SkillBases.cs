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
                Item myWeapon = this.VerifyValue(caller.EquippedWeapon);
                if (args.Enemy != null)
                {
                    Item? otherWeapon = args.Enemy.EquippedWeapon;
                    if (this.VerifyValue(myWeapon.WeaponStats).Type == mWeaponType && otherWeapon != null)
                    {
                        if (this.VerifyValue(otherWeapon.WeaponStats).Type == mOpponentWeaponType)
                        {
                            this.VerifyValue(args.EvaluatedStats).Value.Hit += 20;
                            this.VerifyValue(args.EvaluatedStats).Value.Avo += 20;
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
                Item myWeapon = this.VerifyValue(caller.EquippedWeapon);
                if (this.VerifyValue(myWeapon.WeaponStats).Type == mWeaponType)
                {
                    this.VerifyValue(args.EvaluatedStats).Value.Crit += 10;
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
                Item myWeapon = this.VerifyValue(caller.EquippedWeapon);
                if (this.VerifyValue(myWeapon.WeaponStats).Type == mWeaponType)
                {
                    this.VerifyValue(args.EvaluatedStats).Value.Atk += 5;
                }
            }
        }
        private readonly WeaponType mWeaponType;
    }
}
