using FluentAssertions;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.AutoData;

public class DataRelativePathBuilderTests
{
    [Theory]
    [MutagenAutoData]
    public void Typical(
        DataRelativePath dataRelative)
    {
        Path.IsPathRooted(dataRelative.Path).Should().BeFalse();
    }
}