using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FEEngine
{
    /// <summary>
    /// The object that handles all the inputs
    /// </summary>
    public abstract class InputManager
    {
        public enum Key
        {
            Escape, Enter,
            Q, W, E, R, T, Y, U, I, O, P,
            A, S, D, F, G, H, J, K, L,
            Z, X, C, V, B, N, M
        }
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
            public Key Up { get; set; }
            /// <summary>
            /// Default: <see cref="ConsoleKey.S"/>
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public Key Down { get; set; }
            /// <summary>
            /// Default: <see cref="ConsoleKey.A"/>
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public Key Left { get; set; }
            /// <summary>
            /// Default: <see cref="ConsoleKey.D"/>
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public Key Right { get; set; }
            /// <summary>
            /// Default: <see cref="ConsoleKey.Q"/>
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public Key Quit { get; set; }
            /// <summary>
            /// Default: <see cref="ConsoleKey.Enter"/>
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public Key OK { get; set; }
            /// <summary>
            /// Default: <see cref="ConsoleKey.Escape"/>
            /// </summary>
            [JsonConverter(typeof(StringEnumConverter))]
            public Key Back { get; set; }
        }
        /// <summary>
        /// The current <see cref="KeyBindings"/>
        /// </summary>
        public KeyBindings Bindings { get; set; }
        internal InputManager()
        {
            KeyBindings bindings = new();
            bindings.Up = Key.W;
            bindings.Down = Key.S;
            bindings.Left = Key.A;
            bindings.Right = Key.D;
            bindings.Quit = Key.Q;
            bindings.OK = Key.Enter;
            bindings.Back = Key.Escape;
            Bindings = bindings;
        }
        internal abstract void Update();
        /// <summary>
        /// Gets the current <see cref="State"/>
        /// </summary>
        /// <returns>The current <see cref="State"/></returns>
        public State GetState()
        {
            return mState;
        }
        /// <summary>
        /// Reads the bindings from the specified JSON file
        /// </summary>
        /// <param name="path">The path of the file to read from</param>
        public void ReadBindings(string path)
        {
            Bindings = JsonSerializer.DeserializeFile<KeyBindings>(path);
        }
        /// <summary>
        /// Writes the bindings to the specified JSON file
        /// </summary>
        /// <param name="path">The path of the file to write to</param>
        public void WriteBindings(string path)
        {
            JsonSerializer.Serialize(path, Bindings);
        }
        protected readonly State mState = new();
    }
}
