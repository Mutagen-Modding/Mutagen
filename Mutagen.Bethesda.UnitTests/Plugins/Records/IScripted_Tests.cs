using Mutagen.Bethesda.Core.UnitTests;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records
{
    public class IScripted_Tests
    {
        [Fact]
        public void SomethingTest()
        {
            Assert.True(new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE) is IScripted);
            Assert.False(new Cell(TestConstants.Form1, SkyrimRelease.SkyrimSE) is IScripted);
        }
    }

}
