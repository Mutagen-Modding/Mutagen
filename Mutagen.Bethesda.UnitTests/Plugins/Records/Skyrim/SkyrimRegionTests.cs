using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class SkyrimRegionTests : ASpecificCaseTest<Region, IRegionGetter>
{
    public override ModPath Path => TestDataPathing.SkyrimSoundRegionDataWithNoSecondSubrecord;
    public override GameRelease Release => GameRelease.SkyrimSE;
    public override void TestItem(IRegionGetter item)
    {
        item.Sounds.Should().NotBeNull();
        item.Sounds!.Sounds.Should().BeNull();
    }
}