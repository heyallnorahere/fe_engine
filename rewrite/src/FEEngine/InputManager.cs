using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FEEngine
{
    public class InputManager
    {
        public struct State
        {
            public bool Up;
            public bool Down;
            public bool Left;
            public bool Right;
            public bool Quit;
            public void Reset()
            {
                Up = false;
                Down = false;
                Left = false;
                Right = false;
                Quit = false;
            }
        }
        [JsonObject]
        public struct KeyBindings
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public ConsoleKey Up { get; set; }
            [JsonConverter(typeof(StringEnumConverter))]
            public ConsoleKey Down { get; set; }
            [JsonConverter(typeof(StringEnumConverter))]
            public ConsoleKey Left { get; set; }
            [JsonConverter(typeof(StringEnumConverter))]
            public ConsoleKey Right { get; set; }
            [JsonConverter(typeof(StringEnumConverter))]
            public ConsoleKey Quit { get; set; }
        }
        public static KeyBindings Bindings { get; set; }
        private static void ParseKey(ConsoleKey key)
        {
            // why am i allowed to do this
            var keys = new Dictionary<ConsoleKey, Ref<bool>>
            {
                [Bindings.Up] = new Ref<bool>(ref state.Up),
                [Bindings.Down] = new Ref<bool>(ref state.Down),
                [Bindings.Left] = new Ref<bool>(ref state.Left),
                [Bindings.Right] = new Ref<bool>(ref state.Right),
                [Bindings.Quit] = new Ref<bool>(ref state.Quit)
            };
            foreach (var pair in keys)
            {
                if (key == pair.Key)
                {
                    pair.Value.Get() = true;
                }
            }
        }
        public static void Init()
        {
            KeyBindings bindings = new();
            bindings.Up = ConsoleKey.W;
            bindings.Down = ConsoleKey.S;
            bindings.Left = ConsoleKey.A;
            bindings.Right = ConsoleKey.D;
            bindings.Quit = ConsoleKey.Q;
            Bindings = bindings;
        }
        internal static void Update()
        {
            state.Reset();
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                ParseKey(keyInfo.Key);
            }
        }
        public static State GetState()
        {
            return state;
        }
        public static void ReadBindings(string path)
        {
            Bindings = JsonSerializer.Deserialize<KeyBindings>(path);
        }
        public static void WriteBindings(string path)
        {
            JsonSerializer.Serialize(path, Bindings);
        }
        private InputManager() { }
        private static State state = new();
    }
}
