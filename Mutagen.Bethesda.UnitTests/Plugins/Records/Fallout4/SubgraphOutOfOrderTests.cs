using FluentAssertions;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class SubgraphOutOfOrderTests
{
    [Fact]
    public void Test()
    {
        using var frame = TestDataPathing.GetReadFrame(TestDataPathing.SubgraphOutOfOrder, GameRelease.Fallout4, ModKey.FromFileName("Fallout4.esm"));
        var subgraph = Subgraph.CreateFromBinary(frame);
        subgraph.ActorKeywords.Should().Equal(
            new FormLink<IKeywordGetter>(FormKey.Factory("030B00:Fallout4.esm")),
            new FormLink<IKeywordGetter>(FormKey.Factory("030B01:Fallout4.esm")));
        subgraph.BehaviorGraph.Should().Be(@"Actors\Shared\Behaviors\AlienSharedCoreWrappingBehavior.hkx");
        subgraph.AnimationPaths.Should().Equal(
            @"Actors\Alien\Animations\Injured\BothLegs",
            @"Actors\Alien\Animations");
        subgraph.Role.Should().Be(Subgraph.SubgraphRole.Weapon);
        subgraph.Perspective.Should().Be(Perspective.Third);
    }
}