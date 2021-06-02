namespace FEEngine.Classes
{
    internal class DefaultClass : Class
    {
        public DefaultClass()
        {
            mMovementProperties = Defaults.DefaultBeginnerFootProperties;
        }
        public override MovementProperties MovementProperties => mMovementProperties;
        public override string Name => "None";
        private MovementProperties mMovementProperties;
    }
}
