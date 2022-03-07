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
    public class ItemTests
    {
        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 1)]
        [InlineData(3, 5)]
        [InlineData(4, 7)]
        [InlineData(5, 1)]
        public void CreationAndUsage(int maxUses, int usesUsed)
        {
            var desc = new ItemDesc
            {
                Data = new ItemData
                {
                    Name = "Test Item",
                    MaxUses = maxUses
                }
            };

            var factory = Engine.GetFactory();
            var item = factory?.Create<IItem>(desc);
            Assert.NotNull(item);

            if (item != null)
            {
                bool unusable = item.OnItemUse(usesUsed);
                bool shouldBeUnusable = (maxUses - usesUsed) <= 0;
                Assert.Equal(shouldBeUnusable, unusable);
            }
        }

        [Theory]
        [InlineData(5, 4)]
        [InlineData(3, 3)]
        [InlineData(7, 6)]
        [InlineData(1, 1)]
        public void Prototypes(int maxUses, int usesRemaining)
        {
            var desc = new ItemDesc
            {
                Data = new ItemData
                {
                    MaxUses = maxUses,
                    Name = "Test Item"
                }
            };

            var factory = Engine.GetFactory();
            var prototype = factory?.CreatePrototype<IItem>(desc);
            Assert.NotNull(prototype);

            var item = prototype?.Instantiate((ref ICreationDesc desc) =>
            {
                if (desc is ItemDesc itemDesc)
                {
                    itemDesc.UsesRemaining = usesRemaining;
                    desc = itemDesc;
                }
            });

            Assert.NotNull(item);
            Assert.Equal(desc.Data.Name, item?.Name);
            Assert.Equal(maxUses, item?.MaxUses);
            Assert.Equal(usesRemaining, item?.UsesRemaining);
        }
    }
}