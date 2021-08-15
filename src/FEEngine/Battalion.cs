using System;
using Newtonsoft.Json;

namespace FEEngine
{
    [JsonObject]
    public class BattalionStatBoosts
    {
        public Unit.EvaluatedUnitStats EvaluatedStatBoosts { get; set; }
        public int CharmBoost { get; set; }
    }
    public enum BattalionMovementType
    {
        /// <summary>
        /// On foot
        /// </summary>
        Infantry,
        /// <summary>
        /// Heavily armored
        /// </summary>
        Armored,
        /// <summary>
        /// On a horse
        /// </summary>
        Cavalry,
        /// <summary>
        /// On a flying creature (pegasus, wyvern, etc.)
        /// </summary>
        Flying
    }
    public class Battalion : RegisteredObjectBase<Battalion>
    {
        [JsonConstructor]
        public Battalion()
        {
            mGambit = null;
            Durability = 0;
            StatBoosts = null;
            Name = "Generic Battalion";
            RemainingUses = 0;
        }
        public string? GambitName
        {
            get
            {
                return mGambit?.GetType()?.AssemblyQualifiedName;
            }
            set
            {
                if (value == null)
                {
                    mGambit = null;
                }
                else
                {
                    Type? type = Type.GetType(value);
                    bool isDerived;
                    Type? currentType = type;
                    while (true)
                    {
                        Type? baseType = currentType?.BaseType;
                        if (baseType == typeof(Gambit) || baseType == typeof(object))
                        {
                            isDerived = baseType == typeof(Gambit);
                            break;
                        }
                        currentType = baseType;
                    }
                    if (!isDerived)
                    {
                        mGambit = null;
                        return;
                    }
                    mGambit = (Gambit?)type?.GetConstructor(new Type[0])?.Invoke(new object[0]);
                    RemainingUses = this.VerifyValue(mGambit).MaxUses;
                }
            }
        }
        public int Durability { get; set; }
        public BattalionStatBoosts? StatBoosts { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public Unit? Parent { get; set; }
        [JsonIgnore]
        public int RemainingUses { get; private set; }
        /// <summary>
        /// Sets the child <see cref="Gambit"/>
        /// </summary>
        /// <typeparam name="T">The type of object to assign</typeparam>
        public void SetGambit<T>() where T : Gambit, new()
        {
            mGambit = new T();
            RemainingUses = mGambit.MaxUses;
        }
        internal void UseGambit(Unit target)
        {
            if (RemainingUses <= 0)
            {
                return;
            }
            mGambit?.Use(this.VerifyValue(Parent), target, this);
            RemainingUses--;
        }
        /// <summary>
        /// Gets the child <see cref="Gambit"/>'s type
        /// </summary>
        /// <returns>See summary</returns>
        public GambitType GetGambitType()
        {
            return mGambit?.GambitType ?? GambitType.Null;
        }
        private Gambit? mGambit;
    }
}
