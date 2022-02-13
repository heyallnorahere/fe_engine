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

using System.Collections.Generic;

namespace FEEngine
{
    public struct MapDesc : ICreationDesc
    {
        /// <summary>
        /// The size of this map. Must be at least 1x1.
        /// </summary>
        public Vector Size = (0, 0);
        public string? DebugName = null;

        public bool Verify() => Size.X >= 1 && Size.Y >= 1;
    }

    [FactoryInterface]
    public interface IMap
    {
        public Vector Size { get; }
        public string? DebugName { get; }

        public IReadOnlyCollection<IUnit> Units { get; }
    }
}
