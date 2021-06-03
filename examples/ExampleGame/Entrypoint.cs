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
        public Item Parent { get; set; }
        public void OnUse()
        {
            Parent.Parent.Move(new Vec2I(0, 1), Unit.MovementType.IgnoreMovement);
        }
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
            attackArgs.Might.Value *= 3; // essentially a crit
            attackArgs.CritRate.Value = 100; // then triple that again
        }
    }
    public class Entrypoint
    {
        public static void Main()
        {
            Game.HasNativeImplementation = false;
            Main(true);
        }
        public static void Main(bool debug) // todo: add args parameter
        {
            Game.Debug = debug;
            if (!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }
            Game game = new("data/bindings.json");
            game.SetupRegisters();
            Map map = null;
            InitRegister<Item>("data/items.json", game);
            InitRegister<Unit>("data/units.json", game);
            InitRegister<Map>("data/maps.json", game, () =>
            {
                map = new(20, 10);
                game.Registry.GetRegister<Map>().Add(map);
                return true;
            }, () =>
            {
                try
                {
                    map = game.Registry.GetRegister<Map>()[0];
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
            InitRegister<Tile>("data/tiles.json", game);
            Player player = new(game);
            game.Renderer.Root.Center = new BorderedObject(map);
            game.Renderer.Root.AddChild(new BorderedObject(new Logger.RenderAgent()), BorderLayout.Alignment.Bottom);
            game.Renderer.Root.AddChild(new BorderedMenu(UIController.FindMenu<UnitContextMenu>()), BorderLayout.Alignment.Right);
            game.Renderer.Root.AddChild(new BorderedMenu(UIController.FindMenu<TileInfoMenu>()), BorderLayout.Alignment.Left);
            Logger.Print(Color.Green, "Successfully initialized!");
            game.Loop(player);
            game.Registry.SerializeRegister<Tile>("data/tiles.json");
            game.Registry.SerializeRegister<Map>("data/maps.json");
            game.Registry.SerializeRegister<Unit>("data/units.json");
            game.Registry.SerializeRegister<Item>("data/items.json");
        }
        private static void InitRegister<T>(string filename, Game game, Func<bool> beforeSerializationCallback = null, Func<bool> afterDeserializationCallback = null) where T : class, IRegisteredObject<T>
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
