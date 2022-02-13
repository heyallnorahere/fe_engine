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

using Xunit;

namespace FEEngine.Test
{
    public class MapTests
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        [InlineData(4, 4)]
        [InlineData(5, 5)]
        public void Creation(int width, int height)
        {
            var desc = new MapDesc
            {
                Size = (width, height),
                Name = "Test Map"
            };

            var map = Utilities.DefaultFactory.Create<IMap>(desc);
            Assert.NotNull(map);

            Assert.Equal(map?.Size, desc.Size);
            Assert.Equal(map?.Name, desc.Name);
        }

        [Theory]
        [InlineData(1, 1, 0, 0, true)]
        [InlineData(2, 3, 1, 2, true)]
        [InlineData(1, 1, 0, 1, false)]
        [InlineData(1, 1, 1, 0, false)]
        [InlineData(2, 1, 1, 1, false)]
        public void UnitAdding(int width, int height, int x, int y, bool shouldSucceed)
        {
            var mapDesc = new MapDesc
            {
                Size = (width, height),
                Name = "Test Map"
            };

            var unitDesc = new UnitDesc
            {
                Name = "Test Unit",
                StartingPosition = (x, y)
            };

            var factory = Utilities.DefaultFactory;
            var map = factory.Create<IMap>(mapDesc);
            var unit = factory.Create<IUnit>(unitDesc);

            Assert.NotNull(map);
            Assert.NotNull(unit);

            if (map != null && unit != null)
            {
                int index = map.AddUnit(unit);
                if (shouldSucceed)
                {
                    Assert.NotEqual(index, -1);
                }
                else
                {
                    Assert.Equal(index, -1);
                }
            }
        }
    }
}
