using System;
using System.IO;
using FEEngine;
using Newtonsoft.Json;

namespace ExampleGame
{
    public class Entrypoint
    {
        public static void Main() // todo: add args parameter
        {
            Game game = new Game();
            game.SetupRegisters();
            Map map = new Map(10, 10);
        }
    }
}
