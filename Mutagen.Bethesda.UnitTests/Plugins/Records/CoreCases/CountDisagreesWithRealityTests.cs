using FluentAssertions;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.CoreCases;

public class CountDisagreesWithRealityTests : ASpecificCaseTest<Npc, INpcGetter>
{
    public override ModPath Path => TestDataPathing.CountDisagreesWithReality;
    public override GameRelease Release => GameRelease.Fallout4;
    public override bool TestPassthrough => false;
    
    public override void TestItem(INpcGetter item)
    {
        item.Items.Should().HaveCount(1);
        item.Class.FormKey.Should().Be(FormKey.Factory("123456:CountDisagreesWithReality.esp"));
    }
}
