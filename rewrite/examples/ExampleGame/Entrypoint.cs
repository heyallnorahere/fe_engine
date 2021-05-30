﻿using System;
using System.IO;
using FEEngine;
using FEEngine.Math;
using FEEngine.Scripting;

namespace ExampleGame
{
    public class TestUnitBehavior : IUnitBehavior
    {
        public Unit Parent { get; set; }
        public bool Update()
        {
            Console.WriteLine("Testing...");
            return true;
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
            Player player = new(game);
            game.Renderer.Root.Center = map;
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
