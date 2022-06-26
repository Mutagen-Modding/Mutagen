using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class IScriptedTests
{
    [Fact]
    public void SomethingTest()
    {
        Assert.True(new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE) is IScripted);
        Assert.False(new Cell(TestConstants.Form1, SkyrimRelease.SkyrimSE) is IScripted);
    }
}