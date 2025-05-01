using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing;
using Noggog.Testing.Extensions;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.CoreCases;

public class CountDisagreesWithRealityTests : ASpecificCaseTest<Npc, INpcGetter>
{
    public override ModPath Path => TestDataPathing.CountDisagreesWithReality;
    public override GameRelease Release => GameRelease.Fallout4;
    public override bool TestPassthrough => false;
    
    public override void TestItem(INpcGetter item)
    {
        item.Items.ShouldHaveCount(1);
        item.Class.FormKey.ShouldBe(FormKey.Factory("123456:CountDisagreesWithReality.esp"));
    }
}
