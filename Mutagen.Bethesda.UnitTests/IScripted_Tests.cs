using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class IScripted_Tests
    {
        [Fact]
        public void SomethingTest()
        {
            Assert.True(new Npc(Utility.Form1, SkyrimRelease.SkyrimSE) is IScripted);
            Assert.False(new Cell(Utility.Form1, SkyrimRelease.SkyrimSE) is IScripted);
        }
    }

}
