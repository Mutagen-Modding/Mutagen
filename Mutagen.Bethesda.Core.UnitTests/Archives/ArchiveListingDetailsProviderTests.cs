using AutoFixture.Xunit2;
using FluentAssertions;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Mutagen.Bethesda.Testing.Fakes;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Archives;

public class ArchiveListingDetailsProviderTests
{
    [Theory, MutagenContainerAutoData]
    public void Typical(
        [Frozen] ManualLoadOrderProvider loadOrderListingsProvider,
        [Frozen] ManualArchiveIniListings getArchiveIniListings,
        Lazy<CachedArchiveListingDetailsProvider> lazySut)
    {
        Substitute.For<ArchiveListingDetailsProviderTests>();
        loadOrderListingsProvider.SetTo("ModC.esm", "ModD.esp");
        getArchiveIniListings.SetTo("ArchiveA.bsa", "ArchiveB.bsa");
        var sut = lazySut.Value;
        sut.Contains("ArchiveA.bsa").Should().BeTrue();
        sut.Contains("ArchiveB.bsa").Should().BeTrue();
        sut.Contains("ModC.bsa").Should().BeTrue();
        sut.Contains("ModD.bsa").Should().BeTrue();
        var list = new FileName[]
        {
            "ArchiveA.bsa",
            "ArchiveB.bsa",
            "ModC.bsa",
            "ModD.bsa",
        };
        list.Order(sut.GetComparerFor(null));
        list.Should().Equal(
            "ArchiveA.bsa",
            "ArchiveB.bsa",
            "ModC.bsa",
            "ModD.bsa");
    }
}