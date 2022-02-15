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

            var item = Utilities.DefaultFactory.Create<IItem>(desc);
            Assert.NotNull(item);

            if (item != null)
            {
                bool unusable = item.OnItemUse(usesUsed);
                bool shouldBeUnusable = (maxUses - usesUsed) <= 0;
                Assert.Equal(shouldBeUnusable, unusable);
            }
        }
    }
}