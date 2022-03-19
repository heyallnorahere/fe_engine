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

using FEEngine.Cmdline.ClientData;
using System.IO;

namespace FEEngine.Cmdline
{
    public static class Extensions
    {
        public static PhaseManager? GetPhaseManager(this IMap map)
        {
            if (map.ClientData is MapClientData clientData)
            {
                return clientData.PhaseManager;
            }

            return null;
        }

        public static Allegiance? GetAllegiance(this IUnit unit)
        {
            if (unit.ClientData is UnitClientData clientData)
            {
                return clientData.Allegiance;
            }

            return null;
        }

        public static bool HasMoved(this IUnit unit)
        {
            if (unit.ClientData is UnitClientData clientData)
            {
                return clientData.HasMoved;
            }

            return false;
        }

        public static TextWriter CreateWriter(this Stream stream)
        {
            if (!stream.CanWrite)
            {
                throw new IOException("Cannot write to the passed stream!");
            }

            return new StreamWriter(stream)
            {
                AutoFlush = true
            };
        }
    }
}
