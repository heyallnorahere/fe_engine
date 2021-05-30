namespace FEEngine.Scripting
{
    /// <summary>
    /// An interface for behaviors that attach to an <see cref="Item"/> instance
    /// </summary>
    public interface IItemBehavior
    {
        /// <summary>
        /// Gets called when <see cref="Item.Use"/> is called, given the correct conditions
        /// </summary>
        void OnUse();
        /// <summary>
        /// The behavior's parent <see cref="Item"/>
        /// </summary>
        Item Parent { get; set; }
    }
}
