using System.IO.Abstractions;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Strings;

public class StringsFolderLookupOverlayTests
{
    [Theory, MutagenAutoData]
    internal void Typical(IFileSystem fileSystem,
        ModKey modKey,
        DirectoryPath existingPath,
        MutagenEncodingProvider encodingProvider,
        string normalStr,
        string ilStr,
        string dlStr)
    {
        uint normalId, ilId, dlId;
        using (var writer = new StringsWriter(GameRelease.SkyrimSE, modKey, existingPath, encodingProvider, fileSystem))
        {
            normalId = writer.Register(normalStr, Language.English, StringsSource.Normal);
            ilId = writer.Register(ilStr, Language.English, StringsSource.IL);
            dlId = writer.Register(dlStr, Language.English, StringsSource.DL);
        }
        var overlay = StringsFolderLookupOverlay.TypicalFactory(GameRelease.SkyrimSE, modKey, existingPath, new StringsReadParameters()
        {
            BsaOrdering = Enumerable.Empty<FileName>(),
            StringsFolderOverride = existingPath
        }, fileSystem: fileSystem);
        
        overlay.TryLookup(StringsSource.Normal, Language.English, normalId, out var normalStrOut)
            .Should().BeTrue();
        normalStrOut.Should().Be(normalStr);
        overlay.TryLookup(StringsSource.IL, Language.English, ilId, out var ilStrOut)
            .Should().BeTrue();
        ilStrOut.Should().Be(ilStr);
        overlay.TryLookup(StringsSource.DL, Language.English, dlId, out var dlStrOut)
            .Should().BeTrue();
        dlStrOut.Should().Be(dlStr);
    }
    
    [Theory, MutagenAutoData]
    internal void SuffixCollision(IFileSystem fileSystem,
        DirectoryPath existingPath,
        MutagenEncodingProvider encodingProvider,
        string str1,
        string str2)
    {
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
        
        var overlay = StringsFolderLookupOverlay.TypicalFactory(GameRelease.SkyrimSE, modKey1, existingPath, new StringsReadParameters()
        {
            BsaOrdering = Enumerable.Empty<FileName>(),
            StringsFolderOverride = existingPath
        }, fileSystem: fileSystem);
        
        overlay.TryLookup(StringsSource.Normal, Language.English, id1, out var strOut)
            .Should().BeTrue();
        strOut.Should().Be(str1);
    }
}