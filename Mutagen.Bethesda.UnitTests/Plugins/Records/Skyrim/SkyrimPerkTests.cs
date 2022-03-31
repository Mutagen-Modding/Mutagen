using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class SkyrimPerkTests : ASpecificCaseTest<Perk, IPerkGetter>
{
    public override ModPath Path => TestDataPathing.SkyrimPerkFunctionParametersTypeNone;
    public override GameRelease Release => GameRelease.SkyrimSE;
        
    public override void TestItem(IPerkGetter item)
    {
        item.Effects.Should().HaveCount(2);
    }
}