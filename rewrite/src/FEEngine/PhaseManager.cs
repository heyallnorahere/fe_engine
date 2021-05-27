namespace FEEngine
{
    public class PhaseManager
    {
        public Unit.UnitAffiliation CurrentPhase { get; private set; }
        public PhaseManager(Unit.UnitAffiliation initialPhase = Unit.UnitAffiliation.Player)
        {
            CurrentPhase = initialPhase;
        }
        public void CyclePhase(Map map)
        {
            // i apologize for the spaghetti code
            switch (CurrentPhase)
            {
                case Unit.UnitAffiliation.Player:
                    CurrentPhase = Unit.UnitAffiliation.Enemy;
                    break;
                case Unit.UnitAffiliation.Enemy:
                    CurrentPhase = Unit.UnitAffiliation.ThirdEnemy;
                    break;
                case Unit.UnitAffiliation.ThirdEnemy:
                    CurrentPhase = Unit.UnitAffiliation.Ally;
                    break;
                case Unit.UnitAffiliation.Ally:
                    CurrentPhase = Unit.UnitAffiliation.Player;
                    break;
            }
            foreach (Unit unit in map)
            {
                if (unit.Affiliation == CurrentPhase)
                {
                    unit.RefreshMovement();
                }
            }
        }
    }
}
