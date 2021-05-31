using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using FEEngine.Math;
using FEEngine.Scripting;

namespace FEEngine
{
    /// <summary>
    /// A unit, to be placed on a <see cref="Map"/>
    /// </summary>
    [JsonObject]
    public class Unit : RegisteredObjectBase<Unit>
    {
        public enum UnitAffiliation
        {
            Player,
            Enemy,
            ThirdEnemy,
            Ally
        }
        public static Color GetColorForAffiliation(UnitAffiliation affiliation)
        {
            switch (affiliation)
            {
                case UnitAffiliation.Player:
                    return Color.Blue;
                case UnitAffiliation.Enemy:
                    return Color.Red;
                case UnitAffiliation.ThirdEnemy:
                    return Color.Yellow;
                case UnitAffiliation.Ally:
                    return Color.Green;
                default:
                    return Color.White;
            }
        }
        public enum MovementType
        {
            /// <summary>
            /// Consumes movement points
            /// </summary>
            ConsumeMovement,
            /// <summary>
            /// Does not consume any movement points
            /// </summary>
            IgnoreMovement,
            /// <summary>
            /// Restores movement points
            /// </summary>
            RefundMovement
        }
        [JsonObject]
        public struct UnitStats
        {
            public int HP, Str, Mag, Dex, Spd, Lck, Def, Res, Cha, Mv;
        }
        /// <summary>
        /// The name of the <see cref="Unit"/>
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The <see cref="Unit"/>'s stats
        /// </summary>
        public UnitStats Stats { get; set; }
        /// <summary>
        /// The position of the <see cref="Unit"/> on the map
        /// </summary>
        public IVec2<int> Position { get; private set; }
        /// <summary>
        /// A list, containing <see cref="Register{T}"/> indexes of <see cref="Item"/>s
        /// </summary>
        public List<int> Inventory { get; private set; }
        /// <summary>
        /// How many tiles the <see cref="Unit"/> can move, until the next turn
        /// </summary>
        [JsonIgnore]
        public int CurrentMovement { get; private set; }
        /// <summary>
        /// Whether or not the <see cref="Unit"/> can move
        /// </summary>
        [JsonIgnore]
        public bool CanMove { get; private set; }
        /// <summary>
        /// The <see cref="Unit"/>'s current HP (hit points)
        /// </summary>
        [JsonIgnore]
        public int CurrentHP { get; private set; }
        /// <summary>
        /// The allegiance of the <see cref="Unit"/>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public UnitAffiliation Affiliation { get; set; }
        /// <summary>
        /// The object of the equipped weapon. Use this to equip/unequip weapons.
        /// </summary>
        [JsonIgnore]
        public Item EquippedWeapon
        {
            get
            {
                if (mEquippedWeaponIndex == -1)
                {
                    return null;
                }
                else
                {
                    Register<Item> itemRegister = mRegister.Parent.GetRegister<Item>();
                    return itemRegister[mEquippedWeaponIndex];
                }
            }
            set
            {
                if (value == null)
                {
                    Item previouslyEquipped = EquippedWeapon;
                    mEquippedWeaponIndex = -1;
                    previouslyEquipped.Parent = null;
                }
                else
                {
                    if (!Inventory.Contains(value.RegisterIndex) || !value.IsWeapon)
                    {
                        return; // the value is not a valid equippable weapon
                    }
                    else
                    {
                        // transfer the item from the units inventory into the equipped weapon slot
                        Inventory.Remove(value.RegisterIndex);
                        mEquippedWeaponIndex = value.RegisterIndex;
                    }
                }
            }
        }
        /// <summary>
        /// The <see cref="Register{T}"/> index of the equipped weapon. Do not set this, as Newtonsoft.Json will do so
        /// </summary>
        public int EquippedWeaponIndex
        {
            get => mEquippedWeaponIndex;
            set => mEquippedWeaponIndex = value;
        }
        /// <summary>
        /// The <see cref="Map"/> the <see cref="Unit"/> was placed on
        /// </summary>
        [JsonIgnore]
        public Map Parent { get; set; }
        /// <summary>
        /// Attaches a <see cref="IUnitBehavior"/> onto the unit
        /// </summary>
        /// <typeparam name="B">The type of <see cref="IUnitBehavior"/> to add</typeparam>
        /// <param name="behavior">The behavior instance</param>
        public void AttachBehavior<B>(B behavior) where B : IUnitBehavior, new()
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
        /// Gets the <see cref="Unit"/>'s attached <see cref="IUnitBehavior"/>
        /// </summary>
        /// <typeparam name="B">The type of <see cref="IUnitBehavior"/> to cast to</typeparam>
        /// <returns>The <see cref="Unit"/>'s behavior instance</returns>
        public B GetBehavior<B>() where B : IUnitBehavior, new()
        {
            return (B)mBehavior;
        }
        /// <summary>
        /// Resets the <see cref="Unit"/>'s current movement
        /// </summary>
        public void RefreshMovement()
        {
            CurrentMovement = Stats.Mv;
            CanMove = true;
        }
        /// <summary>
        /// Adds an <see cref="Item"/> to the <see cref="Unit"/>'s inventory
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to add</param>
        public void Add(Item item)
        {
            item.Parent = this;
            Inventory.Add(item.RegisterIndex);
        }
        [OnDeserialized]
        private void OnDeserialization()
        {
            Register<Item> itemRegister = mRegister.Parent.GetRegister<Item>();
            foreach (int index in Inventory)
            {
                itemRegister[index].Parent = this;
            }
        }
        /// <summary>
        /// Moves the <see cref="Unit"/> to the specified position
        /// </summary>
        /// <param name="newPos">The position to move to</param>
        /// <param name="movementType">How to deal with the <see cref="Unit"/>'s movement points</param>
        /// <returns>Whether or not the <see cref="Unit"/> succeeded to move</returns>
        public bool Move(IVec2<int> newPos, MovementType movementType = MovementType.ConsumeMovement)
        {
            if (!CanMove)
            {
                return false;
            }
            IVec2<int> delta = MathUtil.SubVectors(newPos, Position);
            if (MathUtil.IsVectorOutOfBounds(newPos, Parent.Dimensions))
            {
                return false;
            }
            int deltaLength = delta.TaxicabLength();
            if (movementType == MovementType.ConsumeMovement && deltaLength > CurrentMovement)
            {
                return false;
            }
            switch (movementType)
            {
                case MovementType.ConsumeMovement:
                    CurrentMovement -= deltaLength;
                    break;
                case MovementType.RefundMovement:
                    CurrentMovement += deltaLength;
                    if (CurrentMovement > Stats.Mv)
                    {
                        CurrentMovement = Stats.Mv;
                    }
                    break;
            }
            Position = newPos;
            return true;
        }
        /// <summary>
        /// Does nothing; just disables any further action for this turn
        /// </summary>
        public void Wait()
        {
            CanMove = false;
        }
        internal void Update()
        {
            Register<Item> itemRegister = mRegister.Parent.GetRegister<Item>();
            foreach (int index in Inventory)
            {
                Item item = itemRegister[index];
                if (item.Used)
                {
                    Inventory.Remove(index);
                }
            }
            if (mBehavior?.Update() ?? false)
            {
                CanMove = false;
            }
        }
        /// <summary>
        /// Checks if the specified unit is allied to the calling one
        /// </summary>
        /// <param name="other">The unit to check</param>
        /// <returns>If the specified unit is allied</returns>
        public bool IsAllied(Unit other)
        {
            Dictionary<UnitAffiliation, bool> alliedDict, playerAllyAllegiance;
            alliedDict = new(); // were just going to initialize this to an empty dictionary
            playerAllyAllegiance = new()
            {
                [UnitAffiliation.Player] = true,
                [UnitAffiliation.Ally] = true,
                [UnitAffiliation.Enemy] = false,
                [UnitAffiliation.ThirdEnemy] = false
            };
            switch (Affiliation)
            {
                case UnitAffiliation.Player:
                    alliedDict = playerAllyAllegiance;
                    break;
                case UnitAffiliation.Ally:
                    alliedDict = playerAllyAllegiance;
                    break;
                case UnitAffiliation.Enemy:
                    alliedDict = new()
                    {
                        [UnitAffiliation.Player] = false,
                        [UnitAffiliation.Ally] = false,
                        [UnitAffiliation.Enemy] = true,
                        [UnitAffiliation.ThirdEnemy] = false
                    };
                    break;
                case UnitAffiliation.ThirdEnemy:
                    alliedDict = new()
                    {
                        [UnitAffiliation.Player] = false,
                        [UnitAffiliation.Ally] = false,
                        [UnitAffiliation.Enemy] = false,
                        [UnitAffiliation.ThirdEnemy] = true
                    };
                    break;
            }
            return alliedDict[other.Affiliation];
        }
        /// <summary>
        /// Attacks an enemy unit
        /// </summary>
        /// <param name="toAttack">The <see cref="Unit"/> to attack</param>
        /// <returns>Whether the attack succeeded</returns>
        public bool Attack(Unit toAttack)
        {
            // check if the attacker can move, and if the recipient is a valid target
            if (!CanMove || IsAllied(toAttack) || EquippedWeapon == null)
            {
                return false;
            }
            int distance = MathUtil.SubVectors(Position, toAttack.Position).TaxicabLength();
            WeaponStats stats = EquippedWeapon.WeaponStats;
            IVec2<int> range = stats.Range;
            if (distance < range.X || distance > range.Y)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// The <see cref="Type.AssemblyQualifiedName"/> of the <see cref="Unit"/>'s behavior type
        /// </summary>
        public string BehaviorName { get; set; }
        [JsonConstructor]
        public Unit(IVec2<int> position, UnitAffiliation affiliation, UnitStats stats, string behaviorName = null, Item equippedWeapon = null, string name = "Soldier")
        {
            Inventory = new List<int>();
            Name = name;
            Position = position;
            Affiliation = affiliation;
            Stats = stats;
            BehaviorName = behaviorName;
            RefreshMovement();
            CurrentHP = Stats.HP;
            if (equippedWeapon == null)
            {
                mEquippedWeaponIndex = -1;
            }
            else
            {
                if (equippedWeapon.IsWeapon)
                {
                    mEquippedWeaponIndex = equippedWeapon.RegisterIndex;
                }
                else
                {
                    mEquippedWeaponIndex = -1;
                }
            }
            if (BehaviorName == null)
            {
                mBehavior = null;
            } 
            else
            {
                // i couldnt think of any other way lmaoooo
                Type behaviorType = Type.GetType(BehaviorName);
                if (behaviorType == null)
                {
                    mBehavior = null;
                    return;
                }
                var constructor = behaviorType.GetConstructor(new Type[0]);
                mBehavior = (IUnitBehavior)constructor.Invoke(new object[0]);
                mBehavior.Parent = this;
            }
        }
        private IUnitBehavior mBehavior;
        private int mEquippedWeaponIndex;
    }
}
