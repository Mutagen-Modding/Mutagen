using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Testing;
using Noggog.Testing.Extensions;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Oblivion;

public class OblivionRegionTests : ASpecificCaseTest<Region, IRegionGetter>
{
    public override ModPath Path => TestDataPathing.OblivionRegion;
    public override GameRelease Release => GameRelease.Oblivion;
    public override bool TestPassthrough => false;
    
    public override void TestItem(IRegionGetter item)
    {
        item.Sounds.ShouldNotBeNull();
        item.Sounds!.Sounds.ShouldBeEmpty();
        item.Sounds!.MusicType.ShouldBe(MusicType.Default);
        item.MapName.ShouldNotBeNull();
        item.MapName!.Map.ShouldBe("Frostcrag Spire");
        item.Weather.ShouldNotBeNull();
        item.Weather!.Weathers.ShouldHaveCount(1);
        item.Weather!.Weathers![0].Weather.FormKey.ShouldBe(FormKey.Factory("038EF1:OblivionRegion.esp"));
        item.Weather!.Weathers![0].Chance.ShouldBe(100);
    }
}