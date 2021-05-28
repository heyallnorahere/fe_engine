namespace FEEngine.Scripting
{
    public interface IItemBehavior
    {
        void OnUse();
        Item Parent { get; set; }
    }
}
