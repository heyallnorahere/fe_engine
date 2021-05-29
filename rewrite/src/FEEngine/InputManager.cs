using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FEEngine
{
    public class InputManager
    {
        public class State
        {
            /// <summary>
            /// The minimum time between accepted button presses, in seconds
            /// </summary>
            public static double MinimumButtonInterval { get; private set; }
            private static bool ShouldSetValue(ButtonState state, ref double t1)
            {
                double t0 = state.LastPressed;
                t1 = DateTime.Now.TimeOfDay.TotalSeconds;
                return t1 - t0 > MinimumButtonInterval;
            }
            public bool Up
            {
                get => mUp.Pressed;
                set
                {
                    if (!value)
                    {
                        mUp.Pressed = false;
                    }
                    double currentTime = 0;
                    if (ShouldSetValue(mUp, ref currentTime))
                    {
                        mUp.LastPressed = currentTime;
                        mUp.Pressed = value;
                    }
                }
            }
            public bool Down
            {
                get => mDown.Pressed;
                set
                {
                    if (!value)
                    {
                        mDown.Pressed = false;
                    }
                    double currentTime = 0;
                    if (ShouldSetValue(mDown, ref currentTime))
                    {
                        mDown.LastPressed = currentTime;
                        mDown.Pressed = value;
                    }
                }
            }
            public bool Left
            {
                get => mLeft.Pressed;
                set
                {
                    if (!value)
                    {
                        mLeft.Pressed = false;
                    }
                    double currentTime = 0;
                    if (ShouldSetValue(mLeft, ref currentTime))
                    {
                        mLeft.LastPressed = currentTime;
                        mLeft.Pressed = value;
                    }
                }
            }
            public bool Right
            {
                get => mRight.Pressed;
                set
                {
                    if (!value)
                    {
                        mRight.Pressed = false;
                    }
                    double currentTime = 0;
                    if (ShouldSetValue(mRight, ref currentTime))
                    {
                        mRight.LastPressed = currentTime;
                        mRight.Pressed = value;
                    }
                }
            }
            public bool Quit
            {
                get => mQuit.Pressed;
                set
                {
                    if (!value)
                    {
                        mQuit.Pressed = false;
                    }
                    double currentTime = 0;
                    if (ShouldSetValue(mQuit, ref currentTime))
                    {
                        mQuit.LastPressed = currentTime;
                        mQuit.Pressed = value;
                    }
                }
            }
            public void Reset()
            {
                mUp.Reset();
                mDown.Reset();
                mLeft.Reset();
                mRight.Reset();
                mQuit.Reset();
            }
            public State()
            {
                mUp = new();
                mDown = new();
                mLeft = new();
                mRight = new();
                mQuit = new();
            }
            static State()
            {
                // default interval is 0.05 seconds
                MinimumButtonInterval = 0.05;
            }
            private class ButtonState
            {
                public bool Pressed { get; set; }
                public double LastPressed { get; set; }
                public void Reset()
                {
                    Pressed = false;
                }
                public ButtonState()
                {
                    Pressed = false;
                    LastPressed = 0;
                }
            }
            private readonly ButtonState mUp, mDown, mLeft, mRight, mQuit;
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
        private delegate void SetStateCallback();
        private static void ParseKey(ConsoleKey key)
        {
            // why am i allowed to do this (edit: im no longer using Ref<>... whoops)
            var keys = new Dictionary<ConsoleKey, SetStateCallback>
            {
                [Bindings.Up] = () => { state.Up = true; },
                [Bindings.Down] = () => { state.Down = true; },
                [Bindings.Left] = () => { state.Left = true; },
                [Bindings.Right] = () => { state.Right = true; },
                [Bindings.Quit] = () => { state.Quit = true; }
            };
            foreach (var pair in keys)
            {
                if (key == pair.Key)
                {
                    pair.Value();
                }
            }
        }
        static InputManager()
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
