/*
   Copyright 2022 Nora Beda

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.IO;

namespace FEEngine.Cmdline
{
    public class Program
    {
        /// <summary>
        /// The currently-running process.
        /// </summary>
        public static Program Instance { get; }

        static Program()
        {
            Instance = new Program();
        }

        public static void Main(string[] args)
        {
            if (Console.IsInputRedirected || Console.IsOutputRedirected)
            {
                throw new IOException("FEEngine cannot run in this environment!");
            }

            Instance.Run(args);
        }

        private Program()
        {
            mFactory = null;
            mItems = null;
            mMap = null;
            mRunning = false;
        }

        private void Run(string[] args)
        {
            // get factory
            mFactory = Engine.GetFactory();
            if (mFactory == null)
            {
                throw new Exception("No default factory found!");
            }

            // load items
            mItems = new ItemDatabase();

            // parse arguments
            string mapDescriptor;
            if (args.Length > 0)
            {
                mapDescriptor = args[0];
            }
            else
            {
                mapDescriptor = $"{MapSerializer.ManifestPrefix}FEEngine.Cmdline.Resources.Maps.Default.json";
            }

            // load map
            mMap = MapSerializer.Deserialize(mapDescriptor);

            mRunning = true;
            Renderer.OnCtrlC += () => mRunning = false;

            Vector position = (0, 0);
            const char character = 'a';

            Renderer.OnInput += keyInfo =>
            {
                Vector originalPosition = position;
                switch (keyInfo.Key)
                {
                    case ConsoleKey.D:
                        position.X++;
                        break;
                    case ConsoleKey.A:
                        if (position.X > 0)
                        {
                            position.X--;
                        }
                        break;
                    case ConsoleKey.S:
                        position.Y++;
                        break;
                    case ConsoleKey.W:
                        if (position.Y > 0)
                        {
                            position.Y--;
                        }
                        break;
                }

                if (position != originalPosition)
                {
                    Renderer.Draw(originalPosition, ' ');
                    Renderer.Draw(position, character);
                }
            };

            while (mRunning)
            {
                Renderer.Draw(position, character);
                Renderer.HandleInputs();
            }
        }

        public Factory Factory => mFactory!;
        public ItemDatabase Items => mItems!;
        public IMap Map => mMap!;
        public bool Running => mRunning;

        private Factory? mFactory;
        private ItemDatabase? mItems;
        private IMap? mMap;
        private bool mRunning;
    }
}