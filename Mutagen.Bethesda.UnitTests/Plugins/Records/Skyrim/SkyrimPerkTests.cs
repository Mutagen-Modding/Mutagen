using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Noggog.Testing.Extensions;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class SkyrimPerkTests : ASpecificCaseTest<Perk, IPerkGetter>
{
    public override ModPath Path => TestDataPathing.SkyrimPerkFunctionParametersTypeNone;
    public override GameRelease Release => GameRelease.SkyrimSE;
    public override bool TestPassthrough => false;

    public override void TestItem(IPerkGetter item)
    {
        item.Effects.ShouldHaveCount(2);
    }
}