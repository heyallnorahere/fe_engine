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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FEEngine
{
    public static class ManifestItemParser
    {
        public delegate void KeyParser(ref string key);
        public delegate bool ResourceQualifier(string key);

        [JsonObject]
        private struct JsonWeaponRange
        {
            public int Min { get; set; }
            public int Max { get; set; }
        }

        [JsonObject]
        private struct JsonWeaponData
        {
            public int Might { get; set; }
            public int Weight { get; set; }
            public int Hit { get; set; }
            public int Crit { get; set; }
            public JsonWeaponRange Range { get; set; }
            public WeaponType Type { get; set; }
        }

        [JsonObject(ItemNullValueHandling = NullValueHandling.Include)]
        private struct JsonItemData
        {
            public string Name { get; set; }
            public string? ItemBehavior { get; set; }
            public JsonWeaponData? WeaponData { get; set; }
            public int MaxUses { get; set; }
            public int UsesRemaining { get; set; }
        }

        private static ItemBehavior? GetBehavior(string name)
        {
            int separator = name.IndexOf(';');
            if (separator < 0)
            {
                return null;
            }

            var typeName = name[..separator];
            if (typeName.Length == 0)
            {
                return null;
            }

            Type? type = Type.GetType(typeName);
            if (type == null)
            {
                return null;
            }

            var methodName = name[(separator + 1)..];
            if (methodName.Length == 0)
            {
                return null;
            }

            var parameterTypes = new Type[] { typeof(IItem), typeof(IUnit) };
            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, parameterTypes);
            if (method == null)
            {
                return null;
            }

            if (method.ReturnType != typeof(void))
            {
                return null;
            }

            return (ItemBehavior)Delegate.CreateDelegate(typeof(ItemBehavior), method);
        }

        private static ItemDesc Convert(JsonItemData data)
        {
            var itemData = new ItemData
            {
                Name = data.Name,
                Behavior = null,
                WeaponData = null,
                MaxUses = data.MaxUses
            };

            if (data.ItemBehavior != null)
            {
                var behavior = GetBehavior(data.ItemBehavior);
                if (behavior == null)
                {
                    throw new ArgumentException("Invalid item behavior!");
                }

                itemData.Behavior = behavior;
            }

            if (data.WeaponData != null)
            {
                var weaponData = data.WeaponData.Value;
                itemData.WeaponData = new WeaponData
                {
                    Might = weaponData.Might,
                    Weight = weaponData.Weight,
                    Hit = weaponData.Hit,
                    Crit = weaponData.Crit,
                    MinRange = weaponData.Range.Min,
                    MaxRange = weaponData.Range.Max,
                    Type = weaponData.Type
                };
            }

            return new ItemDesc
            {
                Data = itemData,
                UsesRemaining = data.UsesRemaining
            };
        }

        public struct Callbacks
        {
            public KeyParser? KeyParser;
            public ResourceQualifier? ResourceQualifier;
        }

        public static IReadOnlyDictionary<string, FactoryPrototype<IItem>> Load(Assembly assembly, Factory factory, Callbacks? callbacks = null)
        {
            var prototypes = new Dictionary<string, FactoryPrototype<IItem>>();

            string[] names = assembly.GetManifestResourceNames();
            foreach (string name in names)
            {
                if (!name.EndsWith(".json"))
                {
                    continue;
                }

                bool validResource = callbacks?.ResourceQualifier?.Invoke(name) ?? false;
                if (!validResource)
                {
                    continue;
                }

                Stream? stream = assembly.GetManifestResourceStream(name);
                if (stream == null)
                {
                    continue;
                }

                var streamReader = new StreamReader(stream);
                var jsonReader = new JsonTextReader(streamReader);

                var serializer = JsonSerializer.CreateDefault();
                serializer.MissingMemberHandling = MissingMemberHandling.Error;

                try
                {
                    var data = serializer.Deserialize<JsonItemData>(jsonReader);

                    var itemDesc = Convert(data);
                    var prototype = factory.CreatePrototype<IItem>(itemDesc);

                    if (prototype != null)
                    {
                        string key = name;
                        callbacks?.KeyParser?.Invoke(ref key);
                        prototypes.Add(key, prototype);
                    }
                }
                catch (JsonSerializationException)
                {
                    continue;
                }
            }

            return prototypes;
        }
    }
}
