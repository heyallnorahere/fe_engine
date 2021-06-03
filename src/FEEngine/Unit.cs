using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using FEEngine.Classes;
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
        /// <summary>
        /// Gets the <see cref="Color"/> associated with the passed <see cref="UnitAffiliation"/>
        /// </summary>
        /// <param name="affiliation">A enum value, corresponding to a unit's allegiance</param>
        /// <returns>The associated <see cref="Color"/></returns>
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
        /// See <see cref="FEEngine.Class"/>
        /// </summary>
        [JsonIgnore]
        public Class Class
        {
            get => mClass;
            set
            {
                mClass = value ?? new DefaultClass();
                // todo: call events on reclass
            }
        }
        public string ClassName
        {
            get
            {
                return Class.GetType().AssemblyQualifiedName;
            }
            set
            {
                string typeName = value ?? typeof(DefaultClass).AssemblyQualifiedName;
                Type type = Type.GetType(typeName);
                if (type != null)
                {
                    Class = (Class)type.GetConstructor(new Type[0]).Invoke(new object[0]);
                }
            }
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
        /// The unit's <see cref="Stats"/>, plus boosts from things such as classes, and equipment
        /// </summary>
        [JsonIgnore]
        public UnitStats BoostedStats
        {
            get
            {
                UnitStats classBoosts = Class.StatBoosts;
                return new()
                {
                    HP = Stats.HP + classBoosts.HP,
                    Str = Stats.Str + classBoosts.Str,
                    Mag = Stats.Mag + classBoosts.Mag,
                    Dex = Stats.Dex + classBoosts.Dex,
                    Spd = Stats.Spd + classBoosts.Spd,
                    Lck = Stats.Lck + classBoosts.Lck,
                    Def = Stats.Def + classBoosts.Def,
                    Res = Stats.Res + classBoosts.Res,
                    Cha = Stats.Cha + classBoosts.Cha,
                    Mv = Stats.Mv + classBoosts.Mv
                };
            }
        }
        /// <summary>
        /// The position of the <see cref="Unit"/> on the map
        /// </summary>
        public IVec2<int> Position { get; private set; }
        /// <summary>
        /// A list, containing <see cref="Register{T}"/> indexes of <see cref="Item"/>s. Do not use this to add items, use <see cref="Add(Item)"/> instead
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
            CurrentMovement = BoostedStats.Mv;
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
        public override void OnDeserialized()
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
            // todo: use pathfinding algorithm to figure out how much movement to consume/refund
            switch (movementType)
            {
                case MovementType.ConsumeMovement:
                    CurrentMovement -= deltaLength;
                    break;
                case MovementType.RefundMovement:
                    CurrentMovement += deltaLength;
                    if (CurrentMovement > BoostedStats.Mv)
                    {
                        CurrentMovement = BoostedStats.Mv;
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
            if (mBehavior?.Update() ?? Affiliation != UnitAffiliation.Player)
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
            UnitStats boostedStats = BoostedStats;
            UnitStats otherBoostedStats = toAttack.BoostedStats;
            // check if the attacker can move, and if the recipient is a valid target
            Item myWeapon = EquippedWeapon;
            if (!CanMove || IsAllied(toAttack) || myWeapon == null)
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
            Item otherWeapon = toAttack.EquippedWeapon;
            bool iAmInRange = false;
            if (otherWeapon != null) {
                IVec2<int> otherRange = otherWeapon.WeaponStats.Range;
                iAmInRange = distance > otherRange.X && distance < otherRange.Y;
            }
            int attackSpeed = boostedStats.Spd - myWeapon.WeaponStats.Weight;
            int otherAttackSpeed = otherBoostedStats.Spd - otherWeapon?.WeaponStats.Weight ?? 0;
            AttackImpl(toAttack, myWeapon, otherWeapon);
            if (iAmInRange)
            {
                toAttack.AttackImpl(this, otherWeapon, myWeapon);
            }
            if (attackSpeed - otherAttackSpeed >= 4)
            {
                AttackImpl(toAttack, myWeapon, otherWeapon);
            }
            else if (otherAttackSpeed - attackSpeed >= 4 && iAmInRange)
            {
                toAttack.AttackImpl(this, otherWeapon, myWeapon);
            }
            CanMove = false;
            return true;
        }
        private struct AttackPacket
        {
            public int Might { get; set; }
            public int Hit { get; set; }
            public int Crit { get; set; }
        }
        private struct AttackResult
        {
            public int Might { get; set; }
            public bool DidHit { get; set; }
            public bool DidCrit { get; set; }
        }
        private void AttackImpl(Unit toAttack, Item myWeapon, Item otherWeapon)
        {
            AttackPacket packet = CreateAttackPacket(toAttack.BoostedStats, myWeapon);
            AttackResult result = ParseAttackPacket(packet);
            toAttack.ReceiveAttackResult(result, this);
        }
        private AttackPacket CreateAttackPacket(UnitStats otherStats, Item myWeapon)
        {
            UnitStats boostedStats = BoostedStats;
            AttackPacket packet = new();
            WeaponStats weaponStats = myWeapon.WeaponStats;
            var isMagic = weaponStats.Type switch
            {
                WeaponType.WhiteMagic => true,
                WeaponType.BlackMagic => true,
                WeaponType.DarkMagic => true,
                _ => false,
            };
            int strength = isMagic ? boostedStats.Mag : boostedStats.Str;
            int defense = isMagic ? otherStats.Res : otherStats.Def;
            packet.Might = weaponStats.Attack + strength - defense;
            packet.Hit = weaponStats.HitRate + boostedStats.Dex - otherStats.Dex;
            packet.Crit = weaponStats.CritRate + boostedStats.Lck - otherStats.Lck;
            return packet;
        }
        private static AttackResult ParseAttackPacket(AttackPacket packet)
        {
            AttackResult result = new();
            Random randomNumberGenerator = new();
            const int maxValue = 101;
            result.Might = packet.Might;
            result.DidHit = randomNumberGenerator.Next(maxValue) <= packet.Hit;
            result.DidCrit = randomNumberGenerator.Next(maxValue) <= packet.Crit;
            return result;
        }
        private void ReceiveAttackResult(AttackResult result, Unit attacker)
        {
            if (result.DidHit)
            {
                int might = result.Might;
                string message = "{0} dealt {1} damage to {2}!";
                if (result.DidCrit)
                {
                    Logger.Print(Color.Red, "CRITICAL HIT!");
                    message += string.Format(" ({0} * 3)", might);
                    might *= 3;
                }
                CurrentHP -= might;
                Logger.Print(Color.White, message, attacker.Name, might, Name);
            }
            else
            {
                Logger.Print(Color.White, "{0} missed {1}!", attacker.Name, Name);
            }
        }
        /// <summary>
        /// The <see cref="Type.AssemblyQualifiedName"/> of the <see cref="Unit"/>'s behavior type
        /// </summary>
        public string BehaviorName { get; set; }
        [JsonConstructor]
        public Unit(IVec2<int> position, UnitAffiliation affiliation, UnitStats stats, string behaviorName = null, Item equippedWeapon = null, string className = null, string name = "Soldier")
        {
            Inventory = new List<int>();
            Name = name;
            Position = position;
            Affiliation = affiliation;
            Stats = stats;
            BehaviorName = behaviorName;
            ClassName = className; // doesnt matter if its null, ClassName will handle null values
            CurrentHP = BoostedStats.HP;
            RefreshMovement();
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
        private Class mClass;
        private int mEquippedWeaponIndex;
    }
}
