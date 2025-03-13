using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing;
using Noggog;
using Noggog.Testing.Extensions;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class Fallout4LeveledItemChanceNoneTests : ASpecificCaseTest<LeveledItem, ILeveledItemGetter>
{
    public override ModPath Path => TestDataPathing.Fallout4LeveledItems;
    public override GameRelease Release => GameRelease.Fallout4;
    
    public override void TestItem(ILeveledItemGetter item)
    {
        item.Entries.ShouldHaveCount(1);
        item.Entries![0].Data.ShouldNotBeNull();
        item.Entries[0].Data!.ChanceNone.ShouldBe(new Percent(0.83));
    }
}

public class Fallout4LeveledItemChanceNoneOverflowTests : ASpecificCaseTest<LeveledItem, ILeveledItemGetter>
{
    public override ModPath Path => TestDataPathing.Fallout4LeveledItemsOverflow;
    public override GameRelease Release => GameRelease.Fallout4;
    public override bool TestPassthrough => false;

    public override void TestItem(ILeveledItemGetter item)
    {
        item.Entries.ShouldHaveCount(1);
        item.Entries![0].Data.ShouldNotBeNull();
        item.Entries[0].Data!.ChanceNone.ShouldBe(new Percent(0));
    }
}
