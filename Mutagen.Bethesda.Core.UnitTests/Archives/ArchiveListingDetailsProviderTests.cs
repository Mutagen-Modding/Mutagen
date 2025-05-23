using AutoFixture.Xunit2;
using Shouldly;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Mutagen.Bethesda.Testing.Fakes;
using Noggog;
using Noggog.Testing.Extensions;
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
        getArchiveIniListings.SetTo("ArchiveA.bsa", "ArchiveA - Textures.bsa", "ArchiveB.bsa");
        var sut = lazySut.Value;
        sut.Contains("ArchiveA - Textures.bsa").ShouldBeTrue();
        sut.Contains("ArchiveB.bsa").ShouldBeTrue();
        sut.Contains("ModC.bsa").ShouldBeTrue();
        sut.Contains("ModD.bsa").ShouldBeTrue();
        var list = new FileName[]
        {
            "ArchiveA.bsa",
            "ArchiveA - Textures.bsa",
            "ArchiveB.bsa",
            "ModC.bsa",
            "ModD.bsa",
        };
        list.Order(sut.GetComparerFor(null));
        list.ShouldEqual(
            "ArchiveA.bsa",
            "ArchiveA - Textures.bsa",
            "ArchiveB.bsa",
            "ModC.bsa",
            "ModD.bsa");
    }
}