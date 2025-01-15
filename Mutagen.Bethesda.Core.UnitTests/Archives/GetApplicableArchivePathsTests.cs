using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Inis;
using Mutagen.Bethesda.Inis.DI;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing;
using Noggog;
using Noggog.Testing.IO;
using Xunit;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Archives;

public class GetApplicableArchivePathsTests
{
    private static readonly string MyDocumentsPath = $"{PathingUtil.DrivePrefix}{Path.Combine("MyDocuments")}";
    private const string SomeExplicitListingBsa = "SomeExplicitListing.bsa";
    private const string UnusedExplicitListingBsa = "SomeExplicitListing2.bsa";
    private const string SkyrimBsa = "Skyrim.bsa";
    private const string MyModBsa = "MyMod.bsa";
    private static readonly string BaseFolder = $"{PathingUtil.DrivePrefix}{Path.Combine("Games", "Skyrim", "Data")}";

    private IFileSystem GetFileSystem()
    {
        var path = new IniPathProvider(
            new GameReleaseInjection(GameRelease.SkyrimSE),
            new IniPathLookupInjection(MyDocumentsPath));
        var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { path.Path, new MockFileData($@"[Archive]
sResourceArchiveList={SomeExplicitListingBsa}, {UnusedExplicitListingBsa}") }
        });
        fs.Directory.CreateDirectory(BaseFolder);
        return fs;
    }

    private GetApplicableArchivePaths GetClass(IFileSystem fs)
    {
        var gameReleaseInjection = new GameReleaseInjection(GameRelease.SkyrimSE);
        var ext = new ArchiveExtensionProvider(gameReleaseInjection);
        return new GetApplicableArchivePaths(
            fs, 
            new CheckArchiveApplicability(
                ext),
            new DataDirectoryInjection(BaseFolder),
            ext,
            new CachedArchiveListingDetailsProvider(
                new LoadOrderListingsInjection(Array.Empty<ModKey>()),
                new GetArchiveIniListings(
                    fs,
                    new IniPathProvider(
                        gameReleaseInjection,
                        new IniPathLookupInjection(MyDocumentsPath))),
                new ArchiveNameFromModKeyProvider(
                    gameReleaseInjection)));
    }

    #region No ModKey
    
    [Fact]
    public void NoModKey()
    {
        var fs = GetFileSystem();
        fs.File.WriteAllText(Path.Combine(BaseFolder, MyModBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(BaseFolder, SkyrimBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(BaseFolder, SomeExplicitListingBsa), string.Empty);
        var get = GetClass(fs);
        var applicable = get.Get()
            .ToArray();
        applicable.Should().StartWith(new FilePath(
            Path.Combine(BaseFolder, SomeExplicitListingBsa)));
        applicable.Should().BeEquivalentTo(new FilePath[]
        {
            Path.Combine(BaseFolder, SomeExplicitListingBsa),
            Path.Combine(BaseFolder, SkyrimBsa),
            Path.Combine(BaseFolder, MyModBsa)
        });
    }

    #endregion

    #region GetApplicableArchivePaths
    [Fact]
    public void Empty()
    {
        var fs = GetFileSystem();
        var get = GetClass(fs);
        get.Get(TestConstants.Skyrim)
            .Should().BeEmpty();
    }

    [Fact]
    public void NullModKey()
    {
        var fs = GetFileSystem();
        var get = GetClass(fs);
        fs.File.WriteAllText(Path.Combine(BaseFolder, SkyrimBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(BaseFolder, SomeExplicitListingBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(BaseFolder, MyModBsa), string.Empty);
        var applicable = get.Get(ModKey.Null)
            .Should().BeEmpty();
    }

    [Fact]
    public void BaseMod()
    {
        var fs = GetFileSystem();
        var get = GetClass(fs);
        fs.File.WriteAllText(Path.Combine(BaseFolder, SkyrimBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(BaseFolder, SomeExplicitListingBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(BaseFolder, MyModBsa), string.Empty);
        var applicable = get.Get(TestConstants.Skyrim)
            .ToArray();
        applicable.Should().Equal(new FilePath[]
        {
            Path.Combine(BaseFolder, SomeExplicitListingBsa),
            Path.Combine(BaseFolder, SkyrimBsa),
        });
    }

    [Fact]
    public void Typical()
    {
        var fs = GetFileSystem();
        var get = GetClass(fs);
        fs.File.WriteAllText(Path.Combine(BaseFolder, $"{TestConstants.MasterModKey2.Name}.bsa"), string.Empty);
        fs.File.WriteAllText(Path.Combine(BaseFolder, SomeExplicitListingBsa), string.Empty);
        fs.File.WriteAllText(Path.Combine(BaseFolder, MyModBsa), string.Empty);
        var applicable = get.Get(TestConstants.MasterModKey2)
            .ToArray();
        applicable.Should().Equal(new FilePath[]
        {
            Path.Combine(BaseFolder, SomeExplicitListingBsa),
            Path.Combine(BaseFolder, $"{TestConstants.MasterModKey2.Name}.bsa")
        });
    }
    #endregion
}