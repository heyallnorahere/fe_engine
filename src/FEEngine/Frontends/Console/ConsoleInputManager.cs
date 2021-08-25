using System;
using System.Collections.Generic;

namespace FEEngine.Frontends.Console
{
    internal class ConsoleInputManager : InputManager
    {
        private static Key Convert(ConsoleKey consoleKey)
        {
            foreach (Key key in Enum.GetValues<Key>())
            {
                if (key.ToString() == consoleKey.ToString())
                {
                    return key;
                }
            }
            throw new ArgumentException($"The given key ({consoleKey}) is not in the global Key enum!");
        }
        private void ParseKey(ConsoleKey consoleKey)
        {
            // why am i allowed to do this
            var keys = new Dictionary<Key, Action>
            {
                [Bindings.Up] = () => { mState.Up = true; },
                [Bindings.Down] = () => { mState.Down = true; },
                [Bindings.Left] = () => { mState.Left = true; },
                [Bindings.Right] = () => { mState.Right = true; },
                [Bindings.Quit] = () => { mState.Quit = true; },
                [Bindings.OK] = () => { mState.OK = true; },
                [Bindings.Back] = () => { mState.Back = true; }
            };
            Key key = Convert(consoleKey);
            foreach (var pair in keys)
            {
                if (key == pair.Key)
                {
                    pair.Value();
                }
            }
        }
        internal override void Update()
        {
            mState.Reset();
            while (System.Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = System.Console.ReadKey(true);
                ParseKey(keyInfo.Key);
            }
        }
    }
}
