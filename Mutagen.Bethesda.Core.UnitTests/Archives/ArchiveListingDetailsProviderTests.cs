using AutoFixture.Xunit2;
using FluentAssertions;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Mutagen.Bethesda.Testing.Fakes;
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
        sut.Empty.Should().BeFalse();
        sut.Contains("ArchiveA.bsa").Should().BeTrue();
        sut.Contains("ArchiveB.bsa").Should().BeTrue();
        sut.Contains("ModC.bsa").Should().BeTrue();
        sut.Contains("ModD.bsa").Should().BeTrue();
        sut.PriorityIndexFor("ArchiveA.bsa").Should().Be(3);
        sut.PriorityIndexFor("ArchiveB.bsa").Should().Be(2);
        sut.PriorityIndexFor("ModC.bsa").Should().Be(1);
        sut.PriorityIndexFor("ModD.bsa").Should().Be(0);
    }
}