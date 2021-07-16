using FluentAssertions;
using Mutagen.Bethesda.Core.UnitTests;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records
{
    public class FormLinkTests
    {
        [Fact]
        public void EqualityToActualRecord()
        {
            var npc = new Npc(Utility.Form1, SkyrimRelease.SkyrimSE);
            var link = npc.AsLink();
            npc.Should().Be(link);
            link.Should().Be(npc);
        }
    }
}