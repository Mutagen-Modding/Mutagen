using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public abstract class AModHeaderOverflowTests
{
    protected abstract ISkyrimModGetter Get(ModPath modPath);

    [Fact]
    public void CanParseMasters()
    {
        var race = Get(new ModPath(ModKey.Null, TestDataPathing.HeaderOverflow));
        race.ModHeader.MasterReferences.Select(x => x.Master.ToString())
            .Should().Equal("Dawnguard.esm");
    }
}

public class DirectModHeaderOverflowTests : AModHeaderOverflowTests
{
    protected override ISkyrimModGetter Get(ModPath modPath)
    {
        return SkyrimMod.CreateFromBinary(modPath, SkyrimRelease.SkyrimSE);
    }
}

public class OverlayModHeaderOverflowTests : AModHeaderOverflowTests
{
    protected override ISkyrimModGetter Get(ModPath modPath)
    {
        return SkyrimMod.CreateFromBinaryOverlay(modPath, SkyrimRelease.SkyrimSE);
    }
}