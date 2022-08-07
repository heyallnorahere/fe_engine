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

            var factory = Engine.GetFactory();
            var map = factory?.Create<IMap>(desc);
            Assert.NotNull(map);

            Assert.Equal(map?.Size, desc.Size);
            Assert.Equal(map?.Name, desc.Name);
        }

        [Theory]
        [InlineData(2, 1, true)]
        [InlineData(1, 2, true)]
        [InlineData(10, 1, false)]
        [InlineData(0, 10, false)]
        public void MoveAction(int x, int y, bool succeeds)
        {
            if (Utilities.SetupTestMap((10, 10), (0, 0), out IMap? map, out IUnit? unit))
            {
                map.AddUnit(unit);

                Vector offset = (x, y);
                Vector newPosition = unit.Position + offset;

                var action = Action.Create(Action.ID.Move, offset);
                Assert.NotNull(action);

                if (action != null)
                {
                    unit.AddAction(action);
                    bool succeeded = map.Flush();

                    Assert.Equal(succeeds, succeeded);
                    if (succeeded)
                    {
                        Assert.Equal(newPosition, unit.Position);
                    }
                }
            }
        }
    }
}
