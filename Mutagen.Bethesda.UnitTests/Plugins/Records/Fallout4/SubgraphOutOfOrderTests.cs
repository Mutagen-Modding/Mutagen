using Shouldly;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing;
using Noggog.Testing.Extensions;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Fallout4;

public class SubgraphOutOfOrderTests : ASpecificCaseTest<Subgraph, ISubgraphGetter>
{
    public override ModPath Path => new(ModKey.FromFileName("Fallout4.esm"), TestDataPathing.SubgraphOutOfOrder);
    public override GameRelease Release => GameRelease.Fallout4;
    public override bool TestPassthrough => false;
    
    public override void TestItem(ISubgraphGetter subgraph)
    {       
        subgraph.ActorKeywords.ShouldEqualEnumerable(
            new FormLink<IKeywordGetter>(FormKey.Factory("030B00:Fallout4.esm")),
            new FormLink<IKeywordGetter>(FormKey.Factory("030B01:Fallout4.esm")));
        subgraph.BehaviorGraph.ShouldBe(@"Actors\Shared\Behaviors\AlienSharedCoreWrappingBehavior.hkx");
        subgraph.AnimationPaths.ShouldEqualEnumerable(
            @"Actors\Alien\Animations\Injured\BothLegs",
            @"Actors\Alien\Animations");
        subgraph.Role.ShouldBe(Subgraph.SubgraphRole.Weapon);
        subgraph.Perspective.ShouldBe(Perspective.Third);
    }
}