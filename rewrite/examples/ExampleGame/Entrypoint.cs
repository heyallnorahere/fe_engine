using System;
using FEEngine;
using FEEngine.Math;
using Newtonsoft.Json;

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
        public static void Main() // todo: add args parameter
        {
            Game game = new Game();
            game.SetupRegisters();
            Map map = new Map(10, 10);
            game.Registry.GetRegister<Map>().Add(map);
            Player player = new Player(game);
            game.Loop(player);
        }
    }
}
