using System;
using System.IO;
using FEEngine;
using FEEngine.Math;
using FEEngine.Menus;
using FEEngine.Scripting;

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
    public static class Entrypoint
    {
        public static void Main(string[] args) // todo: add args parameter
        {
            if (!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }
            Game game = new("data/bindings.json");
            game.SetupRegisters();
            InitRegister<Item>("data/items.json", game);
            InitRegister<Unit>("data/units.json", game);
            InitRegister<Map>("data/maps.json", game, () =>
            {
                game.Registry.GetRegister<Map>().Add(new(20, 10));
                return true;
            });
            InitRegister<Tile>("data/tiles.json", game);
            InitRegister<Battalion>("data/battalions.json", game);
            Player player = new(game);
            game.Renderer.Root.Center = new BorderedObject(new Map.MapRenderer(game));
            game.Renderer.Root.AddChild(new BorderedObject(new Logger.RenderAgent()), BorderLayout.Alignment.Bottom);
            game.Renderer.Root.AddChild(new BorderedMenu(player.VerifyValue(UIController.FindMenu<UnitContextMenu>())), BorderLayout.Alignment.Right);
            game.Renderer.Root.AddChild(new BorderedMenu(player.VerifyValue(UIController.FindMenu<TileInfoMenu>())), BorderLayout.Alignment.Left);
            Logger.Print(Color.Green, "Successfully initialized!");
            game.Loop(player);
            game.Registry.SerializeRegister<Battalion>("data/battalions.json");
            game.Registry.SerializeRegister<Tile>("data/tiles.json");
            game.Registry.SerializeRegister<Map>("data/maps.json");
            game.Registry.SerializeRegister<Unit>("data/units.json");
            game.Registry.SerializeRegister<Item>("data/items.json");
        }
        private static void InitRegister<T>(string filename, Game game, Func<bool>? beforeSerializationCallback = null, Func<bool>? afterDeserializationCallback = null) where T : class, IRegisteredObject<T>
        {
            if (File.Exists(filename))
            {
                game.Registry.DeserializeRegister<T>(filename);
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
    }
}
