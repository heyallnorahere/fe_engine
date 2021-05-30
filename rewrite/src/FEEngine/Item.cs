using System;
using Newtonsoft.Json;
using FEEngine.Scripting;

namespace FEEngine
{
    /// <summary>
    /// An object that represents an item in a <see cref="Unit"/>'s inventory
    /// </summary>
    [JsonObject]
    public class Item : RegisteredObjectBase<Item>
    {
        public delegate void OnUseHandler(Item item, Unit itemParent);
        /// <summary>
        /// Specifies if <see cref="Use"/> can be called on this object
        /// </summary>
        public bool Usable { get; set; }
        /// <summary>
        /// Specifies if <see cref="Use"/> has been called on this object
        /// </summary>
        public bool Used { get; private set; }
        /// <summary>
        /// Specifies if this object is a weapon (not implemented yet)
        /// </summary>
        public bool IsWeapon { get; protected set; }
        /// <summary>
        /// The name of this item
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The holder of this item
        /// </summary>
        [JsonIgnore]
        public Unit Parent { get; set; }
        /// <summary>
        /// The <see cref="Type.AssemblyQualifiedName"/> of this item's behavior type
        /// </summary>
        public string BehaviorName { get; set; }
        /// <summary>
        /// Attaches a <see cref="IItemBehavior"/> onto the item
        /// </summary>
        /// <typeparam name="B">The type of <see cref="IItemBehavior"/> to add</typeparam>
        /// <param name="behavior">The behavior instance</param>
        public void AttachBehavior<B>(B behavior) where B : IItemBehavior, new()
        {
            mBehavior = behavior;
            if (mBehavior == null)
            {
                BehaviorName = null;
            }
            else
            {
                mBehavior.Parent = this;
                BehaviorName = typeof(B).AssemblyQualifiedName;
            }
        }
        /// <summary>
        /// Gets the <see cref="Item"/>'s attached <see cref="IItemBehavior"/>
        /// </summary>
        /// <typeparam name="B">The type of <see cref="IItemBehavior"/> to cast to</typeparam>
        /// <returns>The <see cref="Item"/>'s behavior instance</returns>
        public B GetBehavior<B>() where B : IItemBehavior
        {
            return (B)mBehavior;
        }
        /// <summary>
        /// Uses the item
        /// </summary>
        /// <returns>If the action succeeded</returns>
        public bool Use()
        {
            if (!Usable || Used)
            {
                return false;
            }
            mBehavior?.OnUse();
            return true;
        }
        public Item(bool usable, string behaviorName = null, string name = "RESERVE")
        {
            Usable = usable;
            IsWeapon = false;
            Name = name;
            Parent = null;
            Used = false;
            BehaviorName = behaviorName;
            if (BehaviorName == null)
            {
                mBehavior = null;
            }
            else
            {
                Type behaviorType = Type.GetType(BehaviorName);
                if (behaviorType == null)
                {
                    mBehavior = null;
                    return;
                }
                var constructor = behaviorType.GetConstructor(new Type[0]);
                mBehavior = (IItemBehavior)constructor.Invoke(new object[0]);
                mBehavior.Parent = this;
            }
        }
        private IItemBehavior mBehavior;
    }
}
