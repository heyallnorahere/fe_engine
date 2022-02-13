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

using FEEngine.Internal;
using System.Collections.Generic;

namespace FEEngine
{
    public static class Engine
    {
        static Engine()
        {
            mFactories = new Dictionary<string, IFactory>();
            RegisterFactory<DefaultFactory>("Default");
        }

        public static bool RegisterFactory<T>(string name) where T : IFactory, new()
        {
            if (mFactories.ContainsKey(name))
            {
                return false;
            }

            mFactories.Add(name, new T());
            return true;
        }

        public static IFactory? GetFactory(string name = "Default")
        {
            IFactory? factory = null;
            if (mFactories.ContainsKey(name))
            {
                factory = mFactories[name];
            }

            return factory;
        }

        private static readonly Dictionary<string, IFactory> mFactories;
    }
}
