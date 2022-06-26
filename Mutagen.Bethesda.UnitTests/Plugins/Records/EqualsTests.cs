using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class EqualsTests
{
    [Fact]
    public void FreshEquals()
    {
        var npc1 = new Npc(TestConstants.Form1);
        var npc2 = new Npc(TestConstants.Form1);
        Assert.Equal(npc1, npc2);
    }

    [Fact]
    public void SimpleEquals()
    {
        var npc1 = new Npc(TestConstants.Form1)
        {
            Name = "TEST"
        };
        var npc2 = new Npc(TestConstants.Form1)
        {
            Name = "TEST"
        };
        Assert.Equal(npc1, npc2);
    }
}