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
    public class UnitTests
    {
        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 0)]
        [InlineData(2, 2)]
        public void Creation(int x, int y)
        {
            var desc = new UnitDesc
            {
                Name = "Test Unit",
                StartingPosition = (x, y)
            };

            var factory = Engine.GetFactory();
            var unit = factory?.Create<IUnit>(desc);
            Assert.NotNull(unit);

            Assert.Equal(desc.Name, unit?.Name);
            Assert.Equal(desc.StartingPosition, unit?.Position);
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

            var factory = Engine.GetFactory();
            var map = factory?.Create<IMap>(mapDesc);
            var unit = factory?.Create<IUnit>(unitDesc);

            Assert.NotNull(map);
            Assert.NotNull(unit);

            if (map != null && unit != null)
            {
                int index = map.AddUnit(unit);
                if (shouldSucceed)
                {
                    Assert.NotEqual(-1, index);
                    Assert.Equal(map.Units[index], unit);
                }
                else
                {
                    Assert.Equal(-1, index);
                    Assert.Equal(0, map.Units.Count);
                }
            }
        }

        [Fact]
        public void Inventory()
        {
            var unitDesc = new UnitDesc
            {
                Name = "Test Unit",
            };

            var itemDesc = new ItemDesc
            {
                Data = new ItemData
                {
                    Name = "Iron Sword",
                    MaxUses = 40,
                    WeaponData = new WeaponData
                    {
                        Might = 5,
                        Weight = 5,
                        Hit = 90,
                        Crit = 0,
                        MinRange = 1,
                        MaxRange = 1,
                        Type = WeaponType.Sword
                    }
                }
            };

            var factory = Engine.GetFactory();
            var unit = factory?.Create<IUnit>(unitDesc);

            var prototype = Utilities.ItemPrototypes["iron-sword"];
            var item = prototype.Instantiate();

            Assert.NotNull(unit);
            Assert.NotNull(item);

            if (unit != null && item != null)
            {
                int inventoryIndex = unit.AddItemToInventory(item);
                Assert.NotEqual(-1, inventoryIndex);
                Assert.Equal(item, unit.Inventory[inventoryIndex]);

                Assert.True(unit.EquipWeapon(inventoryIndex));
                Assert.Equal(item, unit.EquippedWeapon);
            }
        }
    }
}