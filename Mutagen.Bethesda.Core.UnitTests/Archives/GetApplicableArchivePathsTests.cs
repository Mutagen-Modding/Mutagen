using System.IO.Abstractions;
using Shouldly;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
using Mutagen.Bethesda.Testing.Fakes;
using Noggog;
using Noggog.Testing.Extensions;
using Xunit;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Archives;

public class GetApplicableArchivePathsTests
{
    private const string SomeExplicitListingBsa = "SomeExplicitListing.bsa";
    private const string SomeExplicitListingBsa2 = "SomeExplicitListing2.bsa";
    private const string UnusedExplicitListingBsa = "UnusedExplicitListingBsa.bsa";
    private const string SkyrimBsa = "Skyrim.bsa";
    private const string MyModBsa = "MyMod.bsa";
    private const string UnlistedModBsa = "UnlistedMod.bsa";
    
    [Theory, MutagenContainerAutoData]
    public void NoModKey(
        IFileSystem fs,
        ManualLoadOrderProvider loadOrderProvider,
        ManualArchiveIniListings manualArchiveIniListings,
        IDataDirectoryProvider dataDir,
        GetApplicableArchivePaths sut)
    {
        loadOrderProvider.SetTo("Skyrim.esm", "MyMod.esp");
        manualArchiveIniListings.SetTo(SomeExplicitListingBsa, UnusedExplicitListingBsa);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, MyModBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, UnlistedModBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SkyrimBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SomeExplicitListingBsa), string.Empty);
        var applicable = sut.Get()
            .ToArray();
        applicable.ShouldEqualEnumerable(
            Path.Combine(dataDir.Path, SomeExplicitListingBsa),
            Path.Combine(dataDir.Path, SkyrimBsa),
            Path.Combine(dataDir.Path, MyModBsa));
    }

    [Theory, MutagenContainerAutoData]
    public void Empty(
        ManualArchiveIniListings manualArchiveIniListings,
        GetApplicableArchivePaths sut)
    {
        manualArchiveIniListings.SetTo(SomeExplicitListingBsa, UnusedExplicitListingBsa);
        sut.Get(TestConstants.Skyrim)
            .ShouldBeEmpty();
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
        sut.Get(ModKey.Null)
            .ShouldBeEmpty();
    }

    [Theory, MutagenContainerAutoData]
    public void BaseMod(
        IFileSystem fs,
        ManualLoadOrderProvider loadOrderProvider,
        ManualArchiveIniListings manualArchiveIniListings,
        IDataDirectoryProvider dataDir,
        GetApplicableArchivePaths sut)
    {
        loadOrderProvider.SetTo("Skyrim.esm");
        manualArchiveIniListings.SetTo(SomeExplicitListingBsa, UnusedExplicitListingBsa);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SkyrimBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SomeExplicitListingBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, MyModBsa), string.Empty);
        var applicable = sut.Get(TestConstants.Skyrim)
            .ToArray();
        applicable.ShouldBe([
            Path.Combine(dataDir.Path, SomeExplicitListingBsa),
            Path.Combine(dataDir.Path, SkyrimBsa)
        ]);
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
        applicable.ShouldBe([
            Path.Combine(dataDir.Path, SomeExplicitListingBsa),
            Path.Combine(dataDir.Path, $"{TestConstants.MasterModKey2.Name}.bsa")
        ]);
    }
    
    [Theory, MutagenContainerAutoData]
    public void SuffixNaming(
        IFileSystem fs,
        ManualArchiveIniListings manualArchiveIniListings,
        IDataDirectoryProvider dataDir,
        GetApplicableArchivePaths sut)
    {
        manualArchiveIniListings.SetTo(SomeExplicitListingBsa, UnusedExplicitListingBsa);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, $"{TestConstants.MasterModKey2.Name}.bsa"), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, $"{TestConstants.MasterModKey2.Name} - Textures.bsa"), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, $"{TestConstants.MasterModKey2.Name} - Meshes.bsa"), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SomeExplicitListingBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, MyModBsa), string.Empty);
        var applicable = sut.Get(TestConstants.MasterModKey2)
            .ToArray();
        applicable.ShouldBe([
            Path.Combine(dataDir.Path, SomeExplicitListingBsa),
            Path.Combine(dataDir.Path, $"{TestConstants.MasterModKey2.Name}.bsa"),
            Path.Combine(dataDir.Path, $"{TestConstants.MasterModKey2.Name} - Textures.bsa"),
            Path.Combine(dataDir.Path, $"{TestConstants.MasterModKey2.Name} - Meshes.bsa")
        ]);
    }
    
    [Theory, MutagenContainerAutoData]
    public void TwoInis(
        IFileSystem fs,
        ManualArchiveIniListings manualArchiveIniListings,
        IDataDirectoryProvider dataDir,
        GetApplicableArchivePaths sut)
    {
        manualArchiveIniListings.SetTo(SomeExplicitListingBsa2, SomeExplicitListingBsa, UnusedExplicitListingBsa);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, $"{TestConstants.MasterModKey2.Name}.bsa"), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SomeExplicitListingBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SomeExplicitListingBsa2), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, MyModBsa), string.Empty);
        var applicable = sut.Get(TestConstants.MasterModKey2)
            .ToArray();
        applicable.ShouldBe([
            Path.Combine(dataDir.Path, SomeExplicitListingBsa2),
            Path.Combine(dataDir.Path, SomeExplicitListingBsa),
            Path.Combine(dataDir.Path, $"{TestConstants.MasterModKey2.Name}.bsa")
        ]);
    }
    
    [Theory, MutagenContainerAutoData]
    public void DuplicateInis(
        IFileSystem fs,
        ManualArchiveIniListings manualArchiveIniListings,
        IDataDirectoryProvider dataDir,
        GetApplicableArchivePaths sut)
    {
        manualArchiveIniListings.SetTo(SomeExplicitListingBsa, SomeExplicitListingBsa, UnusedExplicitListingBsa);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, $"{TestConstants.MasterModKey2.Name}.bsa"), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SomeExplicitListingBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, SomeExplicitListingBsa2), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataDir.Path, MyModBsa), string.Empty);
        var applicable = sut.Get(TestConstants.MasterModKey2)
            .ToArray();
        applicable.ShouldBe([
            Path.Combine(dataDir.Path, SomeExplicitListingBsa),
            Path.Combine(dataDir.Path, $"{TestConstants.MasterModKey2.Name}.bsa")
        ]);
    }
}