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

namespace FEEngine.Cmdline
{
    public class Program
    {
        public static Program Instance { get; }
        static Program()
        {
            Instance = new Program();
        }

        public static int Main(string[] args)
        {
            if (Console.IsInputRedirected || Console.IsOutputRedirected)
            {
                Console.WriteLine("FEEngine cannot run in this environment!");
                return 1;
            }

            return Instance.Run(args);
        }

        private Program()
        {
            mFactory = null;
            mItems = null;
        }

        private int Run(string[] args)
        {
            mFactory = Engine.GetFactory();
            if (mFactory == null)
            {
                return 1;
            }

            mItems = new ItemDatabase();
            return 0;
        }

        public Factory Factory => mFactory ?? throw new NullReferenceException();
        public ItemDatabase Items => mItems ?? throw new NullReferenceException();

        private Factory? mFactory;
        private ItemDatabase? mItems;
    }
}