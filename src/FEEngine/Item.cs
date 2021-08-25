using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using FEEngine.Scripting;

namespace FEEngine
{
    public enum WeaponType
    {
        Sword,
        Axe,
        Lance,
        Bow,
        Gauntlets,
        WhiteMagic,
        BlackMagic,
        DarkMagic
    }
    [JsonObject]
    public class WeaponStats
    {
        public WeaponStats()
        {
            mBehavior = null;
            Range = new Vector2(1);
        }
        /// <summary>
        /// Gets the <see cref="char"/> value associated with the passed weapon data
        /// </summary>
        /// <param name="stats">An object of type <see cref="WeaponStats"/>, containing the weapon's type. Can be null</param>
        /// <returns>The associated <see cref="char"/></returns>
        public static char GetCharacterForWeapon(WeaponStats? stats)
        {
            if (stats != null)
            {
                switch (stats.Type)
                {
                    case WeaponType.Sword:
                        return 'S';
                    case WeaponType.Axe:
                        return 'A';
                    case WeaponType.Lance:
                        return 'L';
                    case WeaponType.Bow:
                        return 'B';
                    case WeaponType.Gauntlets:
                        return 'G';
                    case WeaponType.WhiteMagic:
                        return 'W';
                    case WeaponType.BlackMagic:
                        return 'M';
                    case WeaponType.DarkMagic:
                        return 'D';
                }
            }
            return 'U';
        }
        /// <summary>
        /// The base damage of the weapon
        /// </summary>
        public int Attack { get; set; }
        /// <summary>
        /// The hit rate of the weapon
        /// </summary>
        public int HitRate { get; set; }
        /// <summary>
        /// The critical rate of the weapon
        /// </summary>
        public int CritRate { get; set; }
        /// <summary>
        /// The type of the weapon
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public WeaponType Type { get; set; }
        /// <summary>
        /// The range of the weapon; X is min, Y is max
        /// </summary>
        public Vector2 Range { get; set; }
        /// <summary>
        /// The weight of the weapon
        /// </summary>
        public int Weight { get; set; }
        /// <summary>
        /// The current durability of the weapon
        /// </summary>
        public int Durability { get; set; }
        /// <summary>
        /// The class name of the currently attached <see cref="WeaponBehavior"/>
        /// </summary>
        public string? BehaviorName
        {
            get
            {
                if (mBehavior != null)
                {
                    return this.VerifyValue(mBehavior.GetType().AssemblyQualifiedName);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value == null)
                {
                    mBehavior = null;
                }
                else
                {
                    Type behaviorType = this.VerifyValue(System.Type.GetType(value));
                    mBehavior = (WeaponBehavior)this.VerifyValue(behaviorType.GetConstructor(new Type[0])?.Invoke(new object[0]));
                }
            }
        }
        /// <summary>
        /// The current attached <see cref="WeaponBehavior"/>
        /// </summary>
        [JsonIgnore]
        public WeaponBehavior? Behavior
        {
            get => mBehavior;
            set => mBehavior = value;
        }
        private WeaponBehavior? mBehavior;
    }
    [JsonObject]
    public class EquipmentStats
    {
        public Skill? Effect { get; set; }
        public int Protection { get; set; }
        public int Resilience { get; set; }
        public int Weight { get; set; }
    }
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
        [JsonIgnore]
        public bool Used { get; private set; }
        /// <summary>
        /// Specifies if this <see cref="Item"/> is a weapon
        /// </summary>
        [JsonIgnore]
        public bool IsWeapon { get { return WeaponStats != null; } }
        /// <summary>
        /// The stats of the weapon; is null if the <see cref="Item"/> is not a weapon
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public WeaponStats? WeaponStats { get; set; }
        /// <summary>
        /// The stats equipment stats of the <see cref="Item"/>; is null if the item cannot be equipped as equipment
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public EquipmentStats? EquipmentStats { get; set; }
        /// <summary>
        /// The name of this item
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The holder of this item
        /// </summary>
        [JsonIgnore]
        public Unit? Parent { get; set; }
        /// <summary>
        /// The <see cref="Type.AssemblyQualifiedName"/> of this item's behavior type
        /// </summary>
        public string? BehaviorName { get; set; }
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
        public B? GetBehavior<B>() where B : IItemBehavior
        {
            return (B?)mBehavior;
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
        [JsonConstructor]
        public Item(bool usable, string? behaviorName = null, WeaponStats? weaponStats = null, string name = "RESERVE")
        {
            Usable = usable;
            Name = name;
            WeaponStats = weaponStats;
            Parent = null;
            Used = false;
            BehaviorName = behaviorName;
            if (BehaviorName == null)
            {
                mBehavior = null;
            }
            else
            {
                Type? behaviorType = Type.GetType(BehaviorName);
                if (behaviorType == null)
                {
                    mBehavior = null;
                    return;
                }
                var constructor = behaviorType?.GetConstructor(Array.Empty<Type>());
                mBehavior = (IItemBehavior)this.VerifyValue(constructor?.Invoke(new object[0]));
                mBehavior.Parent = this;
            }
        }
        private IItemBehavior? mBehavior;
    }
}
