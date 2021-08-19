namespace FEEngine.Classes
{
    internal class DefaultClass : Class
    {
        public DefaultClass()
        {
            mMovementProperties = ClassUtil.DefaultBeginnerFootProperties;
            mStats = Unit.CreateStats();
        }
        public override MovementProperties MovementProperties => mMovementProperties;
        public override string Name => "None";
        public override Unit.UnitStats StatBoosts => mStats;
        private MovementProperties mMovementProperties;
        private Unit.UnitStats mStats;
    }
}
