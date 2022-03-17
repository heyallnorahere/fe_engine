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

using FEEngine.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;

namespace FEEngine.Cmdline
{
    public static class MapSerializer
    {
        public const string ManifestPrefix = "manifest:";

        private class UserDataConverter : CustomCreationConverter<IUnitUserData>
        {
            public override IUnitUserData Create(Type objectType) => new UnitUserData();
        }

        static MapSerializer()
        {
            sSerializer = JsonSerializer.CreateDefault();

            sSerializer.Formatting = Formatting.Indented;
            sSerializer.NullValueHandling = NullValueHandling.Include;
            sSerializer.MissingMemberHandling = MissingMemberHandling.Error;
            sSerializer.DefaultValueHandling = DefaultValueHandling.Populate;

            sSerializer.Converters.Add(new UserDataConverter());
            sSerializer.Converters.Add(new StringEnumConverter());
        }

        private static IItem GetItem(JsonItemDesc desc, ItemDatabase database)
        {
            if (!database.ContainsKey(desc.DatabaseID))
            {
                throw new ArgumentException($"Invalid item: {desc.DatabaseID}!");
            }

            PrototypeDelegate? dataCallback = null;
            if (desc.UsesRemaining > 0)
            {
                dataCallback = (ref ICreationDesc data) =>
                {
                    if (data is ItemDesc itemDesc)
                    {
                        itemDesc.UsesRemaining = desc.UsesRemaining;
                        data = itemDesc;
                    }
                };
            }

            var prototype = database[desc.DatabaseID];
            IItem? item = prototype.Instantiate(dataCallback);
            if (item == null)
            {
                throw new ArgumentException("Invalid item description!");
            }

            return item;
        }

        /// <summary>
        /// Deserializes a map from a descriptor string.
        /// </summary>
        /// <param name="descriptor">
        /// Instructs the serializer how to find the data.
        /// If it begins with <see cref="ManifestPrefix">this prefix</see>,
        /// it tries to find an EmbeddedResource matching that name.
        /// </param>
        /// <returns>The resolved map.</returns>
        /// <exception cref="ArgumentException" />
        /// <exception cref="IOException" />
        public static IMap Deserialize(string descriptor)
        {
            Stream? stream = null;
            if (descriptor.StartsWith(ManifestPrefix))
            {
                var manifestName = descriptor[ManifestPrefix.Length..];

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    if (assembly.IsDynamic)
                    {
                        continue;
                    }

                    var manifestStream = assembly.GetManifestResourceStream(manifestName);
                    if (manifestStream != null)
                    {
                        stream = manifestStream;
                        break;
                    }
                }

                if (stream == null)
                {
                    throw new ArgumentException($"No manifest resource: {manifestName}");
                }
            }
            else
            {
                stream = new FileStream(descriptor, FileMode.Open, FileAccess.Read);
                if (!stream.CanRead)
                {
                    throw new IOException($"Cannot read file: {descriptor}");
                }
            }

            return Deserialize(stream);
        }

        /// <summary>
        /// Deserializes a <see cref="IMap">map</see> from a JSON stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>The resolved map.</returns>
        /// <exception cref="ArgumentException" />
        public static IMap Deserialize(Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Cannot read the stream!");
            }

            var streamReader = new StreamReader(stream);
            var jsonReader = new JsonTextReader(streamReader);
            var jsonMapDesc = sSerializer.Deserialize<JsonMapDesc>(jsonReader);

            var mapDesc = new MapDesc
            {
                Size = jsonMapDesc.Size,
                Name = jsonMapDesc.Name
            };

            var instance = Program.Instance;
            var factory = instance.Factory;

            var map = factory.Create<IMap>(mapDesc);
            if (map == null)
            {
                throw new ArgumentException("Invalid map description!");
            }

            foreach (var jsonUnitDesc in jsonMapDesc.Units)
            {
                var inventory = new List<IItem>();
                foreach (var itemDesc in jsonUnitDesc.Inventory)
                {
                    IItem item = GetItem(itemDesc, instance.Items);
                    inventory.Add(item);
                }

                IItem? equippedWeapon = null;
                if (jsonUnitDesc.EquippedWeapon != null)
                {
                    equippedWeapon = GetItem(jsonUnitDesc.EquippedWeapon!.Value, instance.Items);
                }

                var unitDesc = new UnitDesc
                {
                    Name = jsonUnitDesc.Name,
                    InitialInventory = inventory,
                    EquippedWeapon = equippedWeapon,
                    StartingPosition = jsonUnitDesc.Position,
                    Stats = jsonUnitDesc.Stats,
                    UserData = jsonUnitDesc.UserData,
                };

                IUnit? unit = factory.Create<IUnit>(unitDesc);
                if (unit == null)
                {
                    throw new ArgumentException("Invalid unit description!");
                }

                if (jsonUnitDesc.HPRemaining > 0)
                {
                    unit.HP = jsonUnitDesc.HPRemaining;
                }

                map.AddUnit(unit);
            }

            return map;
        }

        private static readonly JsonSerializer sSerializer;
    }
}
