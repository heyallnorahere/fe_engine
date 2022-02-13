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
        [Fact]
        public void Creation()
        {
            var desc = new UnitDesc
            {
                Name = "Test Unit"
            };

            var unit = Utilities.DefaultFactory.Create<IUnit>(desc);
            Assert.NotNull(unit);

            Assert.Equal(unit?.Name, desc.Name);
        }
    }
}