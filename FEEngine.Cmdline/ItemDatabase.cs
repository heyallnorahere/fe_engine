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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace FEEngine.Cmdline
{
    public sealed class ItemDatabase : IReadOnlyDictionary<string, FactoryPrototype<IItem>>
    {
        private static string BaseNamespace => typeof(Resources.Items.Behaviors).Namespace!;

        internal ItemDatabase()
        {
            var factory = Program.Instance.Factory;
            var assembly = Assembly.GetExecutingAssembly();

            mDatabase = ManifestItemParser.Load(assembly, factory, new ManifestItemParser.Callbacks
            {
                KeyParser = KeyParser,
                ResourceQualifier = key => key.StartsWith(BaseNamespace)
            });
        }

        public bool ContainsKey(string key) => mDatabase.ContainsKey(key);
        public bool TryGetValue(string key, out FactoryPrototype<IItem> value) => mDatabase.TryGetValue(key, out value!);
        public FactoryPrototype<IItem> this[string key] => mDatabase[key];
        public IEnumerable<string> Keys => mDatabase.Keys;
        public IEnumerable<FactoryPrototype<IItem>> Values => mDatabase.Values;
        public int Count => mDatabase.Count;
        public IEnumerator<KeyValuePair<string, FactoryPrototype<IItem>>> GetEnumerator() => mDatabase.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => mDatabase.GetEnumerator();

        private readonly IReadOnlyDictionary<string, FactoryPrototype<IItem>> mDatabase;
        private static void KeyParser(ref string key)
        {
            int lastSeparator = key.LastIndexOf('.');
            if (lastSeparator < 0)
            {
                throw new ArgumentException("Invalid resource name!");
            }

            key = key[(BaseNamespace.Length + 1)..lastSeparator];
            key = key.Replace('.', '/');
        }
    }
}
