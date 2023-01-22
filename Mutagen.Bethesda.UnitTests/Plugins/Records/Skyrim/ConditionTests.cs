using FluentAssertions;
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
        var data = item.Data as IConditionStringParameterGetter;
        data.Should().NotBeNull();
        data!.FirstStringParameter.Should().Be("Hello");
        data.SecondStringParameter.Should().Be("World");
    }
}