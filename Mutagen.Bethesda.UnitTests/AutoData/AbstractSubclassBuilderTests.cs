using FluentAssertions;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.AutoData;

public class AbstractSubclassBuilderTests
{
    [Theory]
    [MutagenModAutoData]
    public void Typical(ANpcLevel npcLevel)
    {
        npcLevel.GetType().Should().Be(typeof(NpcLevel));
    }
}