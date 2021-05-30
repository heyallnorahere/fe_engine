namespace FEEngine.Scripting
{
    /// <summary>
    /// An interface for behaviors that attach to a <see cref="Unit"/> instance
    /// </summary>
    public interface IUnitBehavior
    {
        /// <summary>
        /// Gets called when <see cref="Unit.Update"/> is called
        /// </summary>
        /// <returns>If the update succeeded; if it didn't, then this method will be called again</returns>
        bool Update();
        /// <summary>
        /// The behavior's parent <see cref="Unit"/>
        /// </summary>
        Unit Parent { get; set; }
    }
}
