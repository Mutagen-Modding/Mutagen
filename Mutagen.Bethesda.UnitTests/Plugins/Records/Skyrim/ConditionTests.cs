using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class ConditionTests : ASpecificCaseTest<Condition, IConditionGetter>
{
    public override ModPath Path => TestDataPathing.SkyrimConditionWithTwoStrings;
    public override GameRelease Release => GameRelease.SkyrimSE;
    
    public override void TestItem(IConditionGetter item)
    {
        var data = item.Data as IConditionParametersGetter;
        data.ShouldNotBeNull();
        data!.StringParameter1.ShouldBe("Hello");
        data.StringParameter2.ShouldBe("World");
    }
}