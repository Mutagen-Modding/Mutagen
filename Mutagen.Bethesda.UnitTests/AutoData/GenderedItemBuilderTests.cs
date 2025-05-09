using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Testing.AutoData;
using Shouldly;

namespace Mutagen.Bethesda.UnitTests.AutoData;

public class GenderedItemBuilderTests
{
    [Theory, MutagenAutoData]
    public void Typical(
        GenderedItem<int> g1,
        IGenderedItem<int> g2,
        IGenderedItemGetter<int> g3)
    {
        g2.ShouldBeOfType<GenderedItem<int>>();
        g3.ShouldBeOfType<GenderedItem<int>>();
    }
}