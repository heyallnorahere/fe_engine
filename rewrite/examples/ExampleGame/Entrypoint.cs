using System;
using System.IO;
using FEEngine;

namespace ExampleGame
{
    public class TestBehavior : FEEngine.Scripting.IUnitBehavior
    {
        public Unit Parent { get; set; }
        public void Update()
        {
            Console.WriteLine("hello");
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
            Map map;
            if (File.Exists("maps.json"))
            {
                game.Registry.DeserializeRegister<Map>("maps.json");
                map = game.Registry.GetRegister<Map>()[0];
            }
            else
            {
                map = new Map(20, 10);
                game.Registry.GetRegister<Map>().Add(map);
                game.Registry.SerializeRegister<Map>("maps.json");
            }
            Player player = new(game);
            Console.WriteLine("Successfully initialized!");
            game.Loop(player);
        }
    }
}
