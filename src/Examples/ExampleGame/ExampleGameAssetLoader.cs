using System;
using System.IO;
using FEEngine;
using FEEngine.Math;
using FEEngine.Scripting;
using ExampleGame;

[assembly: AssemblyAssetLoader(typeof(ExampleGameAssetLoader))]

namespace ExampleGame
{
    public class TestItemBehavior : IItemBehavior
    {
        public Item Parent
        {
            get => this.VerifyValue(mParent);
            set => mParent = value;
        }
        public void OnUse()
        {
            mParent?.Parent?.Move(new Vec2I(0, 1), Unit.MovementType.IgnoreMovement);
        }
        private Item? mParent;
    }
    [SkillTrigger(SkillTriggerEvent.OnAttack)]
    public class TestSkill : Skill
    {
        public override Unit.UnitStats StatBoosts => new();
        public override string FriendlyName => "Test skill";
        protected override void Invoke(Unit caller, SkillEventArgs eventArgs)
        {
            SkillAttackArgs attackArgs = (SkillAttackArgs)eventArgs;
            Logger.Print(Color.White, "{0} activated!", nameof(TestSkill));
            this.VerifyValue(attackArgs.Might).Value *= 3; // essentially a crit
            this.VerifyValue(attackArgs.CritRate).Value = 100; // then triple that again
        }
    }
    [WeaponBehaviorTrigger(WeaponBehaviorEvent.OnCalculation), WeaponBehaviorTrigger(WeaponBehaviorEvent.AfterExchange)]
    public class TestWeaponBehavior : WeaponBehavior
    {
        protected override void Invoke(WeaponBehaviorArgs args)
        {
            switch (args.Event)
            {
                case WeaponBehaviorEvent.OnCalculation:
                    Logger.Print(Color.White, "on calculation");
                    break;
                case WeaponBehaviorEvent.AfterExchange:
                    Logger.Print(Color.White, "after exchange");
                    break;
            }
        }
    }
    public class ExampleGameAssetLoader : AssetLoader 
    {
        private static void InitRegister<T>(string filename, Game game, Func<bool>? beforeSerializationCallback = null, Func<bool>? afterDeserializationCallback = null) where T : class, IRegisteredObject<T>
        {
            if (File.Exists(filename))
            {
                game.Registry.DeserializeRegister<T>(File.ReadAllText(filename));
                if (!(afterDeserializationCallback?.Invoke() ?? true))
                {
                    throw new Exception();
                }
            }
            else
            {
                if (!(beforeSerializationCallback?.Invoke() ?? true))
                {
                    throw new Exception();
                }
                game.Registry.SerializeRegister<T>(filename);
            }
        }
        public override void Load(Game game)
        {
            mGame = game;
            if (!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }
            InitRegister<Item>("data/items.json", mGame);
            InitRegister<Unit>("data/units.json", mGame);
            InitRegister<Map>("data/maps.json", mGame, () =>
            {
                game.Registry.GetRegister<Map>().Add(new(20, 10));
                return true;
            });
            InitRegister<Tile>("data/tiles.json", mGame);
            InitRegister<Battalion>("data/battalions.json", mGame);
        }
        public override void Unload()
        {
            Registry registry = this.VerifyValue(mGame).Registry;
            registry.SerializeRegister<Battalion>("data/battalions.json");
            registry.SerializeRegister<Tile>("data/tiles.json");
            registry.SerializeRegister<Map>("data/maps.json");
            registry.SerializeRegister<Unit>("data/units.json");
            registry.SerializeRegister<Item>("data/items.json");
        }
        private Game? mGame;
    }
}
