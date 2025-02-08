using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Noggog.IO;
using Noggog.Testing.Extensions;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class FormLinkTests
{
    [Fact]
    public void EqualityToActualRecord()
    {
        using var tmp = TempFolder.Factory();
        var npc = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        var link = npc.ToLink();
        npc.ShouldEqual(link);
        link.ShouldEqual(npc);
    }
}