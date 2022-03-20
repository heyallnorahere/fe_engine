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

namespace FEEngine.Cmdline.ClientData
{
    public enum ClientActionID
    {
        Move,
        Item,
        Wait,

    }

    [JsonObject(MissingMemberHandling = MissingMemberHandling.Error)]
    public sealed class UnitClientData : IUnitClientData
    {
        public UnitClientData()
        {
            PhaseActions = new List<ClientActionID>();
        }

        public Allegiance Allegiance { get; set; }
        public bool HasMoved { get; set; }
        public List<ClientActionID> PhaseActions { get; set; }

        [JsonIgnore]
        public bool IsValid => true;
    }
}
