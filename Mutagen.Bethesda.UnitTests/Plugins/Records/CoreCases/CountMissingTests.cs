using FluentAssertions;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.CoreCases;

public class CountMissingTests : ASpecificCaseTest<Npc, INpcGetter>
{
    public override ModPath Path => TestDataPathing.CountMissing;
    public override GameRelease Release => GameRelease.Fallout4;
    
    public override void TestItem(INpcGetter item)
    {
        item.Keywords.Should().HaveCount(2);
        item.Keywords.Select(x => x.FormKey).Should().Equal(
            FormKey.Factory("02332A:CountMissing.esp"),
            FormKey.Factory("02332B:CountMissing.esp"));
    }
}
