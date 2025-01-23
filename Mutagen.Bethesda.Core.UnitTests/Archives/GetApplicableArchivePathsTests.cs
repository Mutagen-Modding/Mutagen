using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
using Mutagen.Bethesda.Testing.Fakes;
using Noggog;
using Xunit;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Archives;

public class GetApplicableArchivePathsTests
{
    private const string SomeExplicitListingBsa = "SomeExplicitListing.bsa";
    private const string UnusedExplicitListingBsa = "SomeExplicitListing2.bsa";
    private const string SkyrimBsa = "Skyrim.bsa";
    private const string MyModBsa = "MyMod.bsa";
    
    #region No ModKey
    
    [Theory, MutagenContainerAutoData]
    public void NoModKey(
        IFileSystem fs,
        ManualArchiveIniListings manualArchiveIniListings,
        IDataDirectoryProvider dataDir,
        GetApplicableArchivePaths sut)
    {
        manualArchiveIniListings.SetTo(SomeExplicitListingBsa, UnusedExplicitListingBsa);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, MyModBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SkyrimBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SomeExplicitListingBsa), string.Empty);
        var applicable = sut.Get()
            .ToArray();
        applicable.Should().StartWith(new FilePath(
            Path.Combine(dataDir.Path, SomeExplicitListingBsa)));
        applicable.Should().BeEquivalentTo(new FilePath[]
        {
            Path.Combine(dataDir.Path, SomeExplicitListingBsa),
            Path.Combine(dataDir.Path, SkyrimBsa),
            Path.Combine(dataDir.Path, MyModBsa)
        });
    }

    #endregion

    #region GetApplicableArchivePaths
    [Theory, MutagenContainerAutoData]
    public void Empty(
        ManualArchiveIniListings manualArchiveIniListings,
        GetApplicableArchivePaths sut)
    {
        manualArchiveIniListings.SetTo(SomeExplicitListingBsa, UnusedExplicitListingBsa);
        sut.Get(TestConstants.Skyrim)
            .Should().BeEmpty();
    }

    [Theory, MutagenContainerAutoData]
    public void NullModKey(
        IFileSystem fs,
        ManualArchiveIniListings manualArchiveIniListings,
        IDataDirectoryProvider dataDir,
        GetApplicableArchivePaths sut)
    {
        manualArchiveIniListings.SetTo(SomeExplicitListingBsa, UnusedExplicitListingBsa);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SkyrimBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SomeExplicitListingBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, MyModBsa), string.Empty);
        var applicable = sut.Get(ModKey.Null)
            .Should().BeEmpty();
    }

    [Theory, MutagenContainerAutoData]
    public void BaseMod(
        IFileSystem fs,
        ManualArchiveIniListings manualArchiveIniListings,
        IDataDirectoryProvider dataDir,
        GetApplicableArchivePaths sut)
    {
        manualArchiveIniListings.SetTo(SomeExplicitListingBsa, UnusedExplicitListingBsa);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SkyrimBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SomeExplicitListingBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, MyModBsa), string.Empty);
        var applicable = sut.Get(TestConstants.Skyrim)
            .ToArray();
        applicable.Should().Equal(new FilePath[]
        {
            Path.Combine(dataDir.Path, SomeExplicitListingBsa),
            Path.Combine(dataDir.Path, SkyrimBsa),
        });
    }

    [Theory, MutagenContainerAutoData]
    public void Typical(
        IFileSystem fs,
        ManualArchiveIniListings manualArchiveIniListings,
        IDataDirectoryProvider dataDir,
        GetApplicableArchivePaths sut)
    {
        manualArchiveIniListings.SetTo(SomeExplicitListingBsa, UnusedExplicitListingBsa);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, $"{TestConstants.MasterModKey2.Name}.bsa"), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SomeExplicitListingBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, MyModBsa), string.Empty);
        var applicable = sut.Get(TestConstants.MasterModKey2)
            .ToArray();
        applicable.Should().Equal(new FilePath[]
        {
            Path.Combine(dataDir.Path, SomeExplicitListingBsa),
            Path.Combine(dataDir.Path, $"{TestConstants.MasterModKey2.Name}.bsa")
        });
    }
    #endregion
}