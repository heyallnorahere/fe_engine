﻿/*
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

using FEEngine.Cmdline.UI;
using FEEngine.Cmdline.UI.Views;
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
            mUIRoot = null;
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

            // create ui root
            mUIRoot = new UIRoot((150, 50));

            // create basic border layout
            var layout = new BorderLayout();
            mUIRoot.Node = layout;

            var mapView = new MapView();
            layout.Center = mapView;

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

            // temporary cursor pos controller
            Renderer.OnInput += keyData =>
            {
                switch (keyData.Key)
                {
                    case ConsoleKey.A:
                        {
                            var cursorPos = mapView.CursorPos;
                            if (cursorPos.X > 0)
                            {
                                cursorPos.X--;
                                mapView.CursorPos = cursorPos;
                            }
                        }
                        break;
                    case ConsoleKey.D:
                        {
                            var cursorPos = mapView.CursorPos;
                            if (cursorPos.X < mMap.Size.X - 1)
                            {
                                cursorPos.X++;
                                mapView.CursorPos = cursorPos;
                            }
                        }
                        break;
                    case ConsoleKey.W:
                        {
                            var cursorPos = mapView.CursorPos;
                            if (cursorPos.Y > 0)
                            {
                                cursorPos.Y--;
                                mapView.CursorPos = cursorPos;
                            }
                        }
                        break;
                    case ConsoleKey.S:
                        {
                            var cursorPos = mapView.CursorPos;
                            if (cursorPos.Y < mMap.Size.Y - 1)
                            {
                                cursorPos.Y++;
                                mapView.CursorPos = cursorPos;
                            }
                        }
                        break;
                    case ConsoleKey.C:
                        Renderer.Clear();
                        break;
                }
            };

            bool running = true;
            Renderer.OnCtrlC += () => running = false;

            while (running)
            {
                mUIRoot.Render();
                Renderer.HandleInputs();
            }
        }

        public Factory Factory => mFactory!;
        public ItemDatabase Items => mItems!;
        public IMap Map => mMap!;
        public UIRoot UIRoot => mUIRoot!;

        private Factory? mFactory;
        private ItemDatabase? mItems;
        private IMap? mMap;
        private UIRoot? mUIRoot;
    }
}