using FluentAssertions;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class FormLinkTests
{
    [Fact]
    public void EqualityToActualRecord()
    {
        var npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        var link = npc.AsLink();
        npc.Should().Be(link);
        link.Should().Be(npc);
    }
}