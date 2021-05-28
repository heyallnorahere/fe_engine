using System;
using Newtonsoft.Json;
using FEEngine.Scripting;

namespace FEEngine
{
    [JsonObject]
    public class Item : RegistedObjectTemplate<Item>
    {
        public delegate void OnUseHandler(Item item, Unit itemParent);
        public bool Usable { get; set; }
        public bool Used { get; private set; }
        public bool IsWeapon { get; protected set; }
        public string Name { get; set; }
        [JsonIgnore]
        public Unit Parent { get; set; }
        public string BehaviorName { get; set; }
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
        public B GetBehavior<B>() where B : IItemBehavior
        {
            return (B)mBehavior;
        }
        public bool Use()
        {
            if (!Usable || Used)
            {
                return false;
            }
            mBehavior.OnUse();
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
