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
using System.Collections.Generic;

namespace FEEngine.Json
{
    [JsonObject(MissingMemberHandling = MissingMemberHandling.Ignore)]
    public struct JsonItemDesc
    {
        [JsonProperty(Required = Required.Always)]
        public string DatabaseID { get; set; }

        public int UsesRemaining { get; set; }
    }

    [JsonObject(MissingMemberHandling = MissingMemberHandling.Ignore)]
    public struct JsonUnitDesc
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<JsonItemDesc> Inventory { get; set; }

        [JsonProperty(Required = Required.AllowNull)]
        public JsonItemDesc? EquippedWeapon { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Vector Position { get; set; }

        [JsonProperty(Required = Required.Always)]
        public UnitStats Stats { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int HPRemaining { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public IUnitClientData? ClientData { get; set; }
    }

    [JsonObject(MissingMemberHandling = MissingMemberHandling.Ignore)]
    public struct JsonMapDesc
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string? Name { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Vector Size { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<JsonUnitDesc> Units { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public IMapClientData? ClientData { get; set; }
    }
}
