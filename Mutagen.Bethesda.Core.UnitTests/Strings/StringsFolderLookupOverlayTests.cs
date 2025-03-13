using System.IO.Abstractions;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Strings;

public class StringsFolderLookupOverlayTests
{
    [Theory, MutagenModAutoData]
    internal void Typical(IFileSystem fileSystem,
        ModKey modKey,
        DirectoryPath existingPath,
        MutagenEncodingProvider encodingProvider,
        string normalStr,
        string ilStr,
        string dlStr,
        IGameReleaseContext gameRelease,
        IDataDirectoryProvider dataDirectoryProvider,
        IGetApplicableArchivePaths getApplicableArchivePaths)
    {
        var factory = new StringsFolderLookupFactory(gameRelease, dataDirectoryProvider, getApplicableArchivePaths, fileSystem, new StringsReadParameters()
        {
            StringsFolderOverride = existingPath
        });
        uint normalId, ilId, dlId;
        using (var writer = new StringsWriter(GameRelease.SkyrimSE, modKey, existingPath, encodingProvider, fileSystem))
        {
            normalId = writer.Register(normalStr, Language.English, StringsSource.Normal);
            ilId = writer.Register(ilStr, Language.English, StringsSource.IL);
            dlId = writer.Register(dlStr, Language.English, StringsSource.DL);
        }
        var overlay = factory.Factory(modKey);
        
        overlay.TryLookup(StringsSource.Normal, Language.English, normalId, out var normalStrOut)
            .ShouldBeTrue();
        normalStrOut.ShouldBe(normalStr);
        overlay.TryLookup(StringsSource.IL, Language.English, ilId, out var ilStrOut)
            .ShouldBeTrue();
        ilStrOut.ShouldBe(ilStr);
        overlay.TryLookup(StringsSource.DL, Language.English, dlId, out var dlStrOut)
            .ShouldBeTrue();
        dlStrOut.ShouldBe(dlStr);
    }
    
    [Theory, MutagenModAutoData]
    internal void SuffixCollision(IFileSystem fileSystem,
        DirectoryPath existingPath,
        MutagenEncodingProvider encodingProvider,
        string str1,
        string str2,
        IGameReleaseContext gameRelease,
        IDataDirectoryProvider dataDirectoryProvider,
        IGetApplicableArchivePaths getApplicableArchivePaths)
    {
        var factory = new StringsFolderLookupFactory(gameRelease, dataDirectoryProvider, getApplicableArchivePaths, fileSystem, new StringsReadParameters()
        {
            StringsFolderOverride = existingPath
        });
        
        var modKey1 = ModKey.FromFileName("FileName.esm");
        uint id1;
        using (var writer = new StringsWriter(GameRelease.SkyrimSE, modKey1, existingPath, encodingProvider, fileSystem))
        {
            id1 = writer.Register(str1, Language.English, StringsSource.Normal);
        }
        
        var modKey2 = ModKey.FromFileName("FileNameWithSuffix.esm");
        uint id2;
        using (var writer = new StringsWriter(GameRelease.SkyrimSE, modKey2, existingPath, encodingProvider, fileSystem))
        {
            id2 = writer.Register(str2, Language.English, StringsSource.Normal);
        }
        
        var overlay = factory.Factory(modKey1);
        
        overlay.TryLookup(StringsSource.Normal, Language.English, id1, out var strOut)
            .ShouldBeTrue();
        strOut.ShouldBe(str1);
    }
}