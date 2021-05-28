using System;
using System.IO;
using FEEngine;
using FEEngine.Scripting;

namespace ExampleGame
{
    public class TestUnitBehavior : IUnitBehavior
    {
        public Unit Parent { get; set; }
        public void Update()
        {
            Console.WriteLine("Testing...");
        }
    }
    public class TestItemBehavior : IItemBehavior
    {
        public Item Parent { get; set; }
        public void OnUse()
        {
            Console.WriteLine("Testing again, but from IItemBehavior");
        }
    }
    public class Entrypoint
    {
        public static void Main()
        {
            Main(true);
        }
        public static void Main(bool debug) // todo: add args parameter
        {
            Game.Debug = debug;
            Game game = new Game();
            game.SetupRegisters();
            if (!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }
            InitRegister<Item>("data/items.json", game);
            InitRegister<Unit>("data/units.json", game);
            InitRegister<Map>("data/maps.json", game, () =>
            {
                var map = new Map(20, 10);
                game.Registry.GetRegister<Map>().Add(map);
                return true;
            });
            Player player = new(game);
            Console.WriteLine("Successfully initialized!");
            game.Loop(player);
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
