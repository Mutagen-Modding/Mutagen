using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Testing;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Oblivion;

public class OblivionRegionTests : ASpecificCaseTest<Region, IRegionGetter>
{
    public override ModPath Path => TestDataPathing.OblivionRegion;
    public override GameRelease Release => GameRelease.Oblivion;
    public override bool TestPassthrough => false;
    
    public override void TestItem(IRegionGetter item)
    {
        item.Sounds.Should().NotBeNull();
        item.Sounds!.Sounds.Should().BeEmpty();
        item.Sounds!.MusicType.Should().Be(MusicType.Default);
        item.MapName.Should().NotBeNull();
        item.MapName!.Map.Should().Be("Frostcrag Spire");
        item.Weather.Should().NotBeNull();
        item.Weather!.Weathers.Should().HaveCount(1);
        item.Weather!.Weathers![0].Weather.FormKey.Should().Be(FormKey.Factory("038EF1:OblivionRegion.esp"));
        item.Weather!.Weathers![0].Chance.Should().Be(100);
    }
}