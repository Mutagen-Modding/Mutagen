using FluentAssertions;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class ConditionTests : ATypicalCaseTest<IConditionGetter>
{
    public override GameRelease Release => GameRelease.SkyrimSE;

    public ConditionTests()
        : base(TestDataPathing.SkyrimConditionWithTwoStrings)
    {
    }

    public override void Test(IConditionGetter cond)
    {
        var data = cond.Data as IFunctionConditionDataGetter;
        data.Should().NotBeNull();
        data!.ParameterOneString.Should().Be("Hello");
        data.ParameterTwoString.Should().Be("World");
    }

    public override IConditionGetter GetDirect(MutagenFrame frame)
    {
        return Condition.CreateFromBinary(frame, null);
    }

    public override IConditionGetter GetOverlay(OverlayStream stream, BinaryOverlayFactoryPackage package)
    {
        return ConditionBinaryOverlay.ConditionFactory(stream, package);
    }
}