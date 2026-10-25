using Shouldly;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Mutagen.Bethesda.Testing.Fakes;
using Noggog;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Archives;

public class ArchiveListingDetailsProviderTests
{
    [Theory, MutagenContainerAutoData]
    public void Typical(
        ManualLoadOrderProvider loadOrderListingsProvider,
        ManualArchiveIniListings getArchiveIniListings,
        CachedArchiveListingDetailsProvider sut)
    {
        loadOrderListingsProvider.SetTo("ModC.esm", "ModD.esp");
        getArchiveIniListings.SetTo("ArchiveA.bsa", "ArchiveA - Textures.bsa", "ArchiveB.bsa");
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
        list.ShouldEqualEnumerable(
            "ArchiveA.bsa",
            "ArchiveA - Textures.bsa",
            "ArchiveB.bsa",
            "ModC.bsa",
            "ModD.bsa");
    }
}