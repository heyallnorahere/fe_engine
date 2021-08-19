using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FEEngine
{
    /// <summary>
    /// The class that handles all the inputs
    /// </summary>
    public class InputManager
    {
        /// <summary>
        /// An object that specifies which buttons have been pressed
        /// </summary>
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
            /// <summary>
            /// The button that corresponds to the "Up" binding
            /// </summary>
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
            /// <summary>
            /// The button that corresponds to the "Down" binding
            /// </summary>
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
            /// <summary>
            /// The button that corresponds to the "Left" binding
            /// </summary>
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
            /// <summary>
            /// The button that corresponds to the "Right" binding
            /// </summary>
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
            /// <summary>
            /// The button that corresponds to the "Quit" binding
            /// </summary>
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
            /// <summary>
            /// The button that corresponds to the "OK" binding
            /// </summary>
            public bool OK
            {
                get => mOK.Pressed;
                set
                {
                    if (!value)
                    {
                        mOK.Pressed = false;
                    }
                    double currentTime = 0;
                    if (ShouldSetValue(mOK, ref currentTime))
                    {
                        mOK.LastPressed = currentTime;
                        mOK.Pressed = value;
                    }
                }
            }
            /// <summary>
            /// The button that corresponds to the "Back" binding
            /// </summary>
            public bool Back
            {
                get => mBack.Pressed;
                set
                {
                    if (!value)
                    {
                        mBack.Pressed = false;
                    }
                    double currentTime = 0;
                    if (ShouldSetValue(mBack, ref currentTime))
                    {
                        mBack.LastPressed = currentTime;
                        mBack.Pressed = value;
                    }
                }
            }
            internal void Reset()
            {
                mUp.Reset();
                mDown.Reset();
                mLeft.Reset();
                mRight.Reset();
                mQuit.Reset();
                mOK.Reset();
                mBack.Reset();
            }
            internal State()
            {
                mUp = new();
                mDown = new();
                mLeft = new();
                mRight = new();
                mQuit = new();
                mOK = new();
                mBack = new();
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
            private readonly ButtonState mUp, mDown, mLeft, mRight, mQuit, mOK, mBack;
        }
        [JsonObject]
        public struct KeyBindings
        {
            /// <summary>
            /// Default: <see cref="ConsoleKey.W"/>
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public ConsoleKey Up { get; set; }
            /// <summary>
            /// Default: <see cref="ConsoleKey.S"/>
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public ConsoleKey Down { get; set; }
            /// <summary>
            /// Default: <see cref="ConsoleKey.A"/>
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public ConsoleKey Left { get; set; }
            /// <summary>
            /// Default: <see cref="ConsoleKey.D"/>
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public ConsoleKey Right { get; set; }
            /// <summary>
            /// Default: <see cref="ConsoleKey.Q"/>
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public ConsoleKey Quit { get; set; }
            /// <summary>
            /// Default: <see cref="ConsoleKey.Enter"/>
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public ConsoleKey OK { get; set; }
            /// <summary>
            /// Default: <see cref="ConsoleKey.Escape"/>
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public ConsoleKey Back { get; set; }
        }
        /// <summary>
        /// The current <see cref="KeyBindings"/>
        /// </summary>
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
                [Bindings.Quit] = () => { state.Quit = true; },
                [Bindings.OK] = () => { state.OK = true; },
                [Bindings.Back] = () => { state.Back = true; }
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
            bindings.OK = ConsoleKey.Enter;
            bindings.Back = ConsoleKey.Escape;
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
        /// <summary>
        /// Gets the current <see cref="State"/>
        /// </summary>
        /// <returns>The current <see cref="State"/></returns>
        public static State GetState()
        {
            return state;
        }
        /// <summary>
        /// Reads the bindings from the specified JSON file
        /// </summary>
        /// <param name="path">The path of the file to read from</param>
        public static void ReadBindings(string path)
        {
            Bindings = JsonSerializer.DeserializeFile<KeyBindings>(path);
        }
        /// <summary>
        /// Writes the bindings to the specified JSON file
        /// </summary>
        /// <param name="path">The path of the file to write to</param>
        public static void WriteBindings(string path)
        {
            JsonSerializer.Serialize(path, Bindings);
        }
        private InputManager() { }
        private readonly static State state = new();
    }
}
