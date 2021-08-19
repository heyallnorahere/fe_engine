using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        public static UnitStats CreateStats(int hp = 0, int str = 0, int mag = 0, int dex = 0, int spd = 0, int lck = 0, int def = 0, int res = 0, int cha = 0, int mv = 0)
        {
            return new()
            {
                HP = hp,
                Str = str,
                Mag = mag,
                Dex = dex,
                Spd = spd,
                Lck = lck,
                Def = def,
                Res = res,
                Cha = cha,
                Mv = mv
            };
        }
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
            return affiliation switch
            {
                UnitAffiliation.Player => Color.Blue,
                UnitAffiliation.Enemy => Color.Red,
                UnitAffiliation.ThirdEnemy => Color.Yellow,
                UnitAffiliation.Ally => Color.Green,
                _ => Color.White,
            };
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
        [JsonObject, StructLayout(LayoutKind.Sequential)]
        public struct UnitStats
        {
            public int HP, Str, Mag, Dex, Spd, Lck, Def, Res, Cha, Mv;
            public UnitStats(UnitStats stats)
            {
                HP = stats.HP;
                Str = stats.Str;
                Mag = stats.Mag;
                Dex = stats.Dex;
                Spd = stats.Spd;
                Lck = stats.Lck;
                Def = stats.Def;
                Res = stats.Res;
                Cha = stats.Cha;
                Mv = stats.Mv;
            }
            public static UnitStats operator+(UnitStats a, UnitStats b)
            {
                return new UnitStats
                {
                    HP = a.HP + b.HP,
                    Str = a.Str + b.Str,
                    Mag = a.Mag + b.Mag,
                    Dex = a.Dex + b.Dex,
                    Spd = a.Spd + b.Spd,
                    Lck = a.Lck + b.Lck,
                    Def = a.Def + b.Def,
                    Res = a.Res + b.Res,
                    Cha = a.Cha + b.Cha,
                    Mv = a.Mv + b.Mv
                };
            }
        }
        [JsonObject, StructLayout(LayoutKind.Sequential)]
        public struct EvaluatedUnitStats
        {
            public int Atk, Prt, Rsl, AS, Hit, Avo, Crit, CritAvo;
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
                SkillEventArgs eventArgs = new SkillReclassArgs(mClass.GetType());
                CallEvent(SkillTriggerEvent.OnReclass, eventArgs);
            }
        }
        /// <summary>
        /// Do not touch; this will be set by <see cref="JsonConverter"/>
        /// </summary>
        public string ClassName
        {
            get
            {
                return this.VerifyValue(Class.GetType().AssemblyQualifiedName);
            }
            set
            {
                Type? type = Type.GetType(value);
                if (type != null)
                {
                    Class = Class.GetClass(type);
                }
            }
        }
        /// <summary>
        /// The register index of the unit's equipped <see cref="Battalion"/>
        /// </summary>
        public int BattalionIndex
        {
            get => mBattalionIndex;
            set => mBattalionIndex = value;
        }
        [JsonIgnore]
        public Battalion? Battalion
        {
            get
            {
                if (mBattalionIndex == -1)
                {
                    return null;
                }
                else
                {
                    Register<Battalion> battalionRegister = this.VerifyValue(mRegister).Parent.GetRegister<Battalion>();
                    return battalionRegister[mBattalionIndex];
                }
            }
            set
            {
                mBattalionIndex = value?.RegisterIndex ?? -1;
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
                UnitStats stats = new UnitStats(Stats);
                stats += mClass.StatBoosts;
                foreach (Skill skill in mSkills)
                {
                    stats += skill.StatBoosts;
                }
                foreach (Skill? skill in mClass.ClassSkills)
                {
                    if (skill != null)
                    {
                        stats += skill.StatBoosts;
                    }
                }
                if (mRegister != null)
                {
                    BattalionStatBoosts? battalionStatBoosts = Battalion?.StatBoosts;
                    stats.Cha += battalionStatBoosts?.CharmBoost ?? 0;
                }
                return stats;
            }
        }
        private struct StatEvaluator
        {
            private bool IsMagic(WeaponType type)
            {
                return type switch
                {
                    WeaponType.WhiteMagic => true,
                    WeaponType.BlackMagic => true,
                    WeaponType.DarkMagic => true,
                    _ => false
                };
            }
            public StatEvaluator(Unit unit)
            {
                mUnit = unit;
                mStats = new();
                Reset();
            }
            private void Reset()
            {
                mStats = new(mUnit.BoostedStats);
            }
            public void Evaluate(Ref<EvaluatedUnitStats> stats, Unit enemy, EvaluatedUnitStats? battalionStatBoosts)
            {
                {
                    SkillBeforeStatEvaluationArgs eventArgs = new(enemy);
                    eventArgs.Stats = new Ref<UnitStats>(ref mStats);
                    mUnit.CallEvent(SkillTriggerEvent.BeforeStatEvaluation, eventArgs);
                }
                ref EvaluatedUnitStats evaluatedStats = ref stats.Value;
                evaluatedStats.Atk = GetAtk() + (battalionStatBoosts?.Atk ?? 0);
                evaluatedStats.Prt = GetPrt() + (battalionStatBoosts?.Prt ?? 0);
                evaluatedStats.Rsl = GetRsl() + (battalionStatBoosts?.Rsl ?? 0);
                evaluatedStats.AS = GetAS() + (battalionStatBoosts?.AS ?? 0);
                evaluatedStats.Hit = GetHit() + (battalionStatBoosts?.Hit ?? 0);
                evaluatedStats.Avo = GetAvo(enemy) + (battalionStatBoosts?.Avo ?? 0);
                evaluatedStats.Crit = GetCrit() + (battalionStatBoosts?.Crit ?? 0);
                evaluatedStats.CritAvo = GetCritAvo() + (battalionStatBoosts?.CritAvo ?? 0);
                {
                    SkillAfterStatEvaluationArgs eventArgs = new(enemy);
                    eventArgs.EvaluatedStats = stats;
                    mUnit.CallEvent(SkillTriggerEvent.AfterStatEvaluation, eventArgs);
                }
            }
            private int GetAtk()
            {
                if (mUnit.mEquippedWeaponIndex == -1)
                {
                    return 0;
                }
                Item? equippedWeapon = mUnit.EquippedWeapon;
                WeaponStats? weaponStats = equippedWeapon?.WeaponStats;
                bool isMagic = IsMagic(this.VerifyValue(weaponStats).Type);
                return this.VerifyValue(weaponStats).Attack + (isMagic ? mStats.Mag : mStats.Str);
            }
            private int GetPrt()
            {
                int prt = mStats.Def;
                Item? equippedItem = mUnit.EquippedItem;
                if (equippedItem != null)
                {
                    prt += this.VerifyValue(equippedItem.EquipmentStats).Protection;
                }
                Battalion? battalion = mUnit.Battalion;
                if (battalion != null)
                {
                    BattalionStatBoosts? statBoosts = battalion.StatBoosts;
                    if (statBoosts != null)
                    {
                        prt += statBoosts.EvaluatedStatBoosts.Prt;
                    }
                }
                return prt;
            }
            private int GetRsl()
            {
                int rsl = mStats.Res;
                Item? equippedItem = mUnit.EquippedItem;
                if (equippedItem != null)
                {
                    rsl += this.VerifyValue(equippedItem.EquipmentStats).Resilience;
                }
                Battalion? battalion = mUnit.Battalion;
                if (battalion != null)
                {
                    BattalionStatBoosts? statBoosts = battalion.StatBoosts;
                    if (statBoosts != null)
                    {
                        rsl += statBoosts.EvaluatedStatBoosts.Rsl;
                    }
                }
                return rsl;
            }
            private int GetAS()
            {
                int burden = 0;
                Item? weapon = mUnit.EquippedWeapon;
                if (weapon != null)
                {
                    burden += this.VerifyValue(weapon.WeaponStats).Weight;
                }
                Item? equippedItem = mUnit.EquippedItem;
                if (equippedItem != null)
                {
                    burden += this.VerifyValue(equippedItem.EquipmentStats).Weight;
                }
                burden -= (mStats.Str / 5);
                if (burden < 0)
                {
                    burden = 0;
                }
                return mStats.Spd - burden;
            }
            private int GetHit()
            {
                if (mUnit.mEquippedWeaponIndex == -1)
                {
                    return 0;
                }
                Item weapon = this.VerifyValue(mUnit.EquippedWeapon);
                bool isMagic = IsMagic(this.VerifyValue(weapon.WeaponStats).Type);
                int dex = mStats.Dex;
                return (isMagic ? (dex + mStats.Lck) / 2 : dex) + this.VerifyValue(weapon.WeaponStats).HitRate;
            }
            private int GetAvo(Unit enemy)
            {
                if (enemy == null)
                {
                    return 0;
                }
                // were just gonna assume the enemy has a weapon, or this wouldnt be called
                Item enemyWeapon = this.VerifyValue(enemy.EquippedWeapon);
                bool isMagic = IsMagic(this.VerifyValue(enemyWeapon.WeaponStats).Type);
                int factor = isMagic ? (mStats.Spd + mStats.Lck) / 2 : GetAS();
                return factor; // todo: add more parameters
            }
            private int GetCrit()
            {
                if (mUnit.mEquippedWeaponIndex == -1)
                {
                    return 0;
                }
                Item weapon = this.VerifyValue(mUnit.EquippedWeapon);
                int crit = this.VerifyValue(weapon.WeaponStats).CritRate;
                crit += (mStats.Dex + mStats.Lck) / 2;
                return crit;
            }
            private int GetCritAvo()
            {
                return mStats.Lck; // todo: add combat art crit avoid
            }
            private readonly Unit mUnit;
            private UnitStats mStats;
        }
        /// <summary>
        /// Get the effective stats of the current <see cref="Unit"/>
        /// </summary>
        /// <returns>The <see cref="EvaluatedUnitStats"/></returns>
        public EvaluatedUnitStats GetEvaluatedStats(Unit enemy)
        {
            EvaluatedUnitStats evaluatedStats = new();
            StatEvaluator evaluator = new(this);
            Battalion? battalion = Battalion;
            BattalionStatBoosts? battalionStatBoosts = battalion?.StatBoosts;
            EvaluatedUnitStats? battalionBoosts = battalionStatBoosts?.EvaluatedStatBoosts;
            evaluator.Evaluate(new Ref<EvaluatedUnitStats>(ref evaluatedStats), enemy, battalionBoosts);
            return evaluatedStats;
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
        /// A list, containing <see cref="Type.AssemblyQualifiedName"/>s of <see cref="Skill"/> types.
        /// </summary>
        public List<string> SkillNames
        {
            get
            {
                List<string> typeNames = new();
                foreach (Skill skill in mSkills)
                {
                    typeNames.Add(this.VerifyValue(skill.GetType().AssemblyQualifiedName));
                }
                return typeNames;
            }
            set
            {
                mSkills.Clear();
                foreach (string skillName in value)
                {
                    Skill skill = Skill.GetSkill(skillName);
                    mSkills.Add(skill);
                }
            }
        }
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
        public Item? EquippedWeapon
        {
            get
            {
                if (mEquippedWeaponIndex == -1)
                {
                    return null;
                }
                else
                {
                    Register<Item> itemRegister = this.VerifyValue(mRegister).Parent.GetRegister<Item>();
                    return itemRegister[mEquippedWeaponIndex];
                }
            }
            set
            {
                if (value == null)
                {
                    Item? previouslyEquipped = EquippedWeapon;
                    mEquippedWeaponIndex = -1;
                    if (previouslyEquipped != null)
                    {
                        previouslyEquipped.Parent = null;
                    }
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
        /// The equipped <see cref="Item"/>. Prefer this over <see cref="EquippedItemIndex"/>.
        /// </summary>
        [JsonIgnore]
        public Item? EquippedItem
        {
            get
            {
                if (mEquippedItemIndex == null)
                {
                    return null;
                }
                else
                {
                    int inventoryIndex = mEquippedItemIndex ?? throw new NullReferenceException();
                    int registerIndex = Inventory[inventoryIndex];
                    Register<Item> itemRegister = this.VerifyValue(mRegister).Parent.GetRegister<Item>();
                    return itemRegister[registerIndex];
                }
            }
            set
            {
                if (value == null)
                {
                    mEquippedItemIndex = null;
                }
                else
                {
                    int registerIndex = value.RegisterIndex;
                    if (!Inventory.Contains(registerIndex))
                    {
                        throw new ArgumentException("The equipped item must be in the unit's inventory!");
                    }
                    int inventoryIndex = Inventory.IndexOf(registerIndex);
                    Register<Item> itemRegister = this.VerifyValue(mRegister).Parent.GetRegister<Item>();
                    Item item = itemRegister[inventoryIndex];
                    if (item.EquipmentStats == null)
                    {
                        throw new ArgumentException("The equipped item must have valid equipment stats!");
                    }
                    mEquippedItemIndex = inventoryIndex;
                }
            }
        }
        /// <summary>
        /// The inventory index of the equipped <see cref="Item"/>
        /// </summary>
        public int? EquippedItemIndex
        {
            get => mEquippedItemIndex;
            set => mEquippedItemIndex = value;
        }
        /// <summary>
        /// The <see cref="Map"/> the <see cref="Unit"/> was placed on
        /// </summary>
        [JsonIgnore]
        public Map? Parent { get; set; }
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
        public B? GetBehavior<B>() where B : IUnitBehavior, new()
        {
            return (B?)mBehavior;
        }
        /// <summary>
        /// Resets the <see cref="Unit"/>'s current movement
        /// </summary>
        public void RefreshMovement(bool keepCurrentMovement = false)
        {
            if (!keepCurrentMovement)
            {
                CurrentMovement = BoostedStats.Mv;
            }
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
        /// <summary>
        /// Adds a <see cref="Skill"/> to the <see cref="Unit"/>'s skill list
        /// </summary>
        /// <typeparam name="T">The type of skill to add</typeparam>
        /// <returns>If the skill was successfully added</returns>
        public bool AddSkill<T>() where T : Skill, new()
        {
            if (mSkills.Count >= 5)
            {
                return false;
            }
            mSkills.Add(new T());
            return true;
        }
        public override void OnDeserialized()
        {
            Register<Item> itemRegister = this.VerifyValue(mRegister?.Parent?.GetRegister<Item>());
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
            if (MathUtil.IsVectorOutOfBounds(newPos, this.VerifyValue(Parent?.Dimensions)))
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
            SkillEventArgs eventArgs = new SkillMoveArgs(Position);
            CallEvent(SkillTriggerEvent.OnMove, eventArgs);
            return true;
        }
        /// <summary>
        /// Does nothing; just disables any further action for this turn
        /// </summary>
        public void Wait()
        {
            CanMove = false;
        }
        internal void UpdateWeapon()
        {
            Item? equippedWeapon = EquippedWeapon;
            if ((equippedWeapon?.WeaponStats?.Durability ?? 1) <= 0)
            {
                mEquippedWeaponIndex = -1; // just stop referencing it
                Logger.Print(Color.Yellow, "{0} broke...", this.VerifyValue(equippedWeapon).Name);
            }
        }
        internal void Update()
        {
            Register<Item> itemRegister = this.VerifyValue(mRegister?.Parent?.GetRegister<Item>());
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
        private void CallEvent(SkillTriggerEvent @event, SkillEventArgs eventArgs)
        {
            List<Skill> skills = new();
            skills.AddRange(mSkills);
            foreach (Skill? skill in mClass.ClassSkills)
            {
                if (skill != null)
                {
                    skills.Add(skill);
                }
            }
            foreach (Skill skill in skills)
            {
                Skill.Invoke(skill, @event, this, eventArgs);
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
            EvaluatedUnitStats evaluatedStats = GetEvaluatedStats(toAttack);
            EvaluatedUnitStats otherEvaluatedStats = toAttack.GetEvaluatedStats(this);
            // check if the attacker can move, and if the recipient is a valid target
            Item? myWeapon = EquippedWeapon;
            if (!CanMove || IsAllied(toAttack) || myWeapon == null)
            {
                return false;
            }
            int distance = MathUtil.SubVectors(Position, toAttack.Position).TaxicabLength();
            WeaponStats stats = this.VerifyValue(myWeapon.WeaponStats);
            IVec2<int> range = stats.Range;
            if (distance < range.X || distance > range.Y)
            {
                return false;
            }
            Item? otherWeapon = toAttack.EquippedWeapon;
            bool iAmInRange = false;
            if (otherWeapon != null) {
                IVec2<int> otherRange = this.VerifyValue(otherWeapon.WeaponStats).Range;
                iAmInRange = distance > otherRange.X && distance < otherRange.Y;
            }
            WeaponAfterExchangeArgs afterExchangeArgs = new(this, toAttack);
            WeaponAfterExchangeArgs otherAfterExchangeArgs = new(toAttack, this);
            AttackImpl(toAttack, myWeapon, evaluatedStats, otherEvaluatedStats, afterExchangeArgs);
            if (toAttack.CurrentHP <= 0)
            {
                goto exit;
            }
            if (iAmInRange && otherWeapon != null)
            {
                toAttack.AttackImpl(this, otherWeapon, otherEvaluatedStats, evaluatedStats, otherAfterExchangeArgs);
                if (CurrentHP <= 0)
                {
                    goto exit;
                }
            }
            if (evaluatedStats.AS - otherEvaluatedStats.AS >= 4)
            {
                AttackImpl(toAttack, myWeapon, evaluatedStats, otherEvaluatedStats, afterExchangeArgs);
                if (toAttack.CurrentHP <= 0)
                {
                    goto exit;
                }
            }
            else if (otherEvaluatedStats.AS - evaluatedStats.AS >= 4 && iAmInRange && otherWeapon != null)
            {
                toAttack.AttackImpl(this, otherWeapon, otherEvaluatedStats, evaluatedStats, otherAfterExchangeArgs);
            }
        exit:
            if (myWeapon.WeaponStats?.Behavior != null)
            {
                WeaponBehavior.Invoke(WeaponBehaviorEvent.AfterExchange, afterExchangeArgs, myWeapon.WeaponStats.Behavior);
            }
            if (otherWeapon?.WeaponStats?.Behavior != null)
            {
                WeaponBehavior.Invoke(WeaponBehaviorEvent.AfterExchange, otherAfterExchangeArgs, otherWeapon.WeaponStats.Behavior);
            }
            CanMove = false;
            return true;
        }
        /// <summary>
        /// A structure containing the might, hit rate, and critical rate of an attack
        /// </summary>
        public struct AttackPacket
        {
            public int Might;
            public int Hit;
            public int Crit;
        }
        private struct AttackResult
        {
            public int Might { get; set; }
            public bool DidHit { get; set; }
            public bool DidCrit { get; set; }
        }
        private unsafe void AttackImpl(Unit toAttack, Item myWeapon, EvaluatedUnitStats myStats, EvaluatedUnitStats otherStats, WeaponAfterExchangeArgs afterExchangeArgs)
        {
            AttackPacket packet = CreateAttackPacket(myStats, otherStats, myWeapon);
            SkillAttackArgs eventArgs = new(toAttack);
            eventArgs.Might = &packet.Might;
            eventArgs.HitRate = &packet.Hit;
            eventArgs.CritRate = &packet.Crit;
            CallEvent(SkillTriggerEvent.OnAttack, eventArgs);
            WeaponOnCalculationArgs weaponArgs = new(this, toAttack);
            weaponArgs.Packet = &packet;
            if (myWeapon.WeaponStats?.Behavior != null)
            {
                WeaponBehavior.Invoke(WeaponBehaviorEvent.OnCalculation, weaponArgs, this.VerifyValue(myWeapon.WeaponStats?.Behavior));
            }
            AttackResult result = ParseAttackPacket(packet);
            toAttack.ReceiveAttackResult(result, this);
            this.VerifyValue(myWeapon.WeaponStats).Durability--;
            if (result.DidHit)
            {
                afterExchangeArgs.TimesAttacked++;
            }
        }
        private AttackPacket CreateAttackPacket(EvaluatedUnitStats myStats, EvaluatedUnitStats otherStats, Item myWeapon)
        {
            AttackPacket packet = new();
            WeaponStats weaponStats = this.VerifyValue(myWeapon.WeaponStats);
            var isMagic = weaponStats.Type switch
            {
                WeaponType.WhiteMagic => true,
                WeaponType.BlackMagic => true,
                WeaponType.DarkMagic => true,
                _ => false,
            };
            int defense = isMagic ? otherStats.Rsl : otherStats.Prt;
            packet.Might = weaponStats.Attack + myStats.Atk - defense;
            packet.Hit = myStats.Hit - otherStats.Avo;
            packet.Crit = myStats.Crit - otherStats.Avo;
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
        public string? BehaviorName { get; set; }
        [JsonConstructor]
        public Unit(IVec2<int> position, UnitAffiliation affiliation, UnitStats stats, string? behaviorName = null, Item? equippedWeapon = null, string? className = null, int? equippedItemIndex = null, string name = "Soldier")
        {
            Inventory = new();
            Name = name;
            Position = position;
            Affiliation = affiliation;
            Stats = stats;
            BehaviorName = behaviorName;
            mSkills = new();
            ClassName = className ?? this.VerifyValue(typeof(DefaultClass).AssemblyQualifiedName);
            if (mClass == null)
            {
                throw new NullReferenceException();
            }
            CurrentHP = BoostedStats.HP;
            RefreshMovement();
            mBattalionIndex = -1;
            mEquippedItemIndex = equippedItemIndex;
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
                Type? behaviorType = Type.GetType(BehaviorName);
                if (behaviorType == null)
                {
                    mBehavior = null;
                    return;
                }
                var constructor = behaviorType.GetConstructor(new Type[0]);
                mBehavior = (IUnitBehavior)this.VerifyValue(constructor?.Invoke(new object[0]));
                mBehavior.Parent = this;
            }
        }
        private IUnitBehavior? mBehavior;
        private Class mClass;
        private readonly List<Skill> mSkills;
        private int mEquippedWeaponIndex, mBattalionIndex;
        private int? mEquippedItemIndex;
    }
}
