namespace FEEngine.Scripting
{
    public interface IUnitBehavior
    {
        bool Update();
        Unit Parent { get; set; }
    }
}
