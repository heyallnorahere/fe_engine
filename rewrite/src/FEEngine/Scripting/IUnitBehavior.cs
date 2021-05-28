namespace FEEngine.Scripting
{
    public interface IUnitBehavior
    {
        void Update();
        Unit Parent { get; set; }
    }
}
