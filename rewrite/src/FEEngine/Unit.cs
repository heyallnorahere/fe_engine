using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using FEEngine.Math;
using FEEngine.Scripting;

namespace FEEngine
{
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
            ConsumeMovement,
            IgnoreMovement,
            RefundMovement
        }
        [JsonObject]
        public struct UnitStats
        {
            public int HP, Str, Mag, Dex, Spd, Lck, Def, Res, Cha, Mv;
        }
        public string Name { get; set; }
        public UnitStats Stats { get; set; }
        public IVec2<int> Position { get; private set; }
        public List<int> Inventory { get; private set; }
        [JsonIgnore]
        public int CurrentMovement { get; private set; }
        [JsonIgnore]
        public bool CanMove { get; private set; }
        [JsonIgnore]
        public int CurrentHP { get; private set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public UnitAffiliation Affiliation { get; set; }
        [JsonIgnore]
        public Map Parent { get; set; }
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
        public B GetBehavior<B>() where B : IUnitBehavior, new()
        {
            return (B)mBehavior;
        }
        public void RefreshMovement()
        {
            CurrentMovement = Stats.Mv;
            CanMove = true;
        }
        public void Add(Item item)
        {
            item.Parent = this;
            Inventory.Add(item.RegisterIndex);
        }
        public override void OnDeserialization()
        {
            Register<Item> itemRegister = mRegister.Parent.GetRegister<Item>();
            foreach (int index in Inventory)
            {
                itemRegister[index].Parent = this;
            }
        }
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
            if (deltaLength > CurrentMovement)
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
        public void Wait()
        {
            CanMove = false;
        }
        public void Update()
        {
            if (mBehavior?.Update() ?? false)
            {
                CanMove = false;
            }
        }
        public string BehaviorName { get; set; }
        [JsonConstructor]
        public Unit(IVec2<int> position, UnitAffiliation affiliation, UnitStats stats, string behaviorName = null, string name = "Soldier")
        {
            Inventory = new List<int>();
            Name = name;
            Position = position;
            Affiliation = affiliation;
            Stats = stats;
            BehaviorName = behaviorName;
            RefreshMovement();
            CurrentHP = Stats.HP;
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
    }
}
