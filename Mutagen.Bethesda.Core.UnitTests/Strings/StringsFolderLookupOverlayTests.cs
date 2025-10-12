using System.IO.Abstractions;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using NSubstitute;
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
        var overlay = factory.InternalFactory(modKey);
        
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

    [Theory, MutagenModAutoData]
    internal void TryLookupWithSourcePath(IFileSystem fileSystem,
        ModKey modKey,
        DirectoryPath existingPath,
        MutagenEncodingProvider encodingProvider,
        string testString,
        IGameReleaseContext gameRelease,
        IDataDirectoryProvider dataDirectoryProvider,
        IGetApplicableArchivePaths getApplicableArchivePaths)
    {
        var factory = new StringsFolderLookupFactory(gameRelease, dataDirectoryProvider, getApplicableArchivePaths, fileSystem, new StringsReadParameters()
        {
            StringsFolderOverride = existingPath
        });
        uint stringId;
        using (var writer = new StringsWriter(GameRelease.SkyrimSE, modKey, existingPath, encodingProvider, fileSystem))
        {
            stringId = writer.Register(testString, Language.English, StringsSource.Normal);
        }
        var overlay = factory.InternalFactory(modKey);

        overlay.TryLookup(StringsSource.Normal, Language.English, stringId, out var str, out var sourcePath)
            .ShouldBeTrue();
        str.ShouldBe(testString);
        sourcePath.ShouldNotBeNullOrEmpty();
    }

    [Theory, MutagenModAutoData]
    internal void TryLookupNonExistentLanguage(IFileSystem fileSystem,
        ModKey modKey,
        DirectoryPath existingPath,
        MutagenEncodingProvider encodingProvider,
        string testString,
        IGameReleaseContext gameRelease,
        IDataDirectoryProvider dataDirectoryProvider,
        IGetApplicableArchivePaths getApplicableArchivePaths)
    {
        var factory = new StringsFolderLookupFactory(gameRelease, dataDirectoryProvider, getApplicableArchivePaths, fileSystem, new StringsReadParameters()
        {
            StringsFolderOverride = existingPath
        });
        uint stringId;
        using (var writer = new StringsWriter(GameRelease.SkyrimSE, modKey, existingPath, encodingProvider, fileSystem))
        {
            stringId = writer.Register(testString, Language.English, StringsSource.Normal);
        }
        var overlay = factory.InternalFactory(modKey);

        overlay.TryLookup(StringsSource.Normal, Language.French, stringId, out var str)
            .ShouldBeFalse();
        str.ShouldBeNull();

        overlay.TryLookup(StringsSource.Normal, Language.French, stringId, out str, out var sourcePath)
            .ShouldBeFalse();
        str.ShouldBeNull();
        sourcePath.ShouldBeNull();
    }

    [Theory, MutagenModAutoData]
    internal void TryLookupNonExistentKey(IFileSystem fileSystem,
        ModKey modKey,
        DirectoryPath existingPath,
        MutagenEncodingProvider encodingProvider,
        string testString,
        IGameReleaseContext gameRelease,
        IDataDirectoryProvider dataDirectoryProvider,
        IGetApplicableArchivePaths getApplicableArchivePaths)
    {
        var factory = new StringsFolderLookupFactory(gameRelease, dataDirectoryProvider, getApplicableArchivePaths, fileSystem, new StringsReadParameters()
        {
            StringsFolderOverride = existingPath
        });
        using (var writer = new StringsWriter(GameRelease.SkyrimSE, modKey, existingPath, encodingProvider, fileSystem))
        {
            writer.Register(testString, Language.English, StringsSource.Normal);
        }
        var overlay = factory.InternalFactory(modKey);

        overlay.TryLookup(StringsSource.Normal, Language.English, 999999u, out var str)
            .ShouldBeFalse();
        str.ShouldBeNull();

        overlay.TryLookup(StringsSource.Normal, Language.English, 999999u, out str, out var sourcePath)
            .ShouldBeFalse();
        str.ShouldBeNull();
        sourcePath.ShouldBeNull();
    }

    [Theory, MutagenModAutoData]
    internal void EmptyOverlay(IFileSystem fileSystem,
        ModKey modKey,
        DirectoryPath existingPath,
        IGameReleaseContext gameRelease,
        IDataDirectoryProvider dataDirectoryProvider,
        IGetApplicableArchivePaths getApplicableArchivePaths)
    {
        var factory = new StringsFolderLookupFactory(gameRelease, dataDirectoryProvider, getApplicableArchivePaths, fileSystem, new StringsReadParameters()
        {
            StringsFolderOverride = existingPath
        });
        var overlay = factory.InternalFactory(modKey);

        overlay.Empty.ShouldBeTrue();
        overlay.AvailableLanguages(StringsSource.Normal).ShouldBeEmpty();
        overlay.AvailableLanguages(StringsSource.IL).ShouldBeEmpty();
        overlay.AvailableLanguages(StringsSource.DL).ShouldBeEmpty();
    }

    [Theory, MutagenModAutoData]
    internal void AvailableLanguages(IFileSystem fileSystem,
        ModKey modKey,
        DirectoryPath existingPath,
        MutagenEncodingProvider encodingProvider,
        string englishStr,
        string frenchStr,
        string germanStr,
        IGameReleaseContext gameRelease,
        IDataDirectoryProvider dataDirectoryProvider,
        IGetApplicableArchivePaths getApplicableArchivePaths)
    {
        var factory = new StringsFolderLookupFactory(gameRelease, dataDirectoryProvider, getApplicableArchivePaths, fileSystem, new StringsReadParameters()
        {
            StringsFolderOverride = existingPath
        });

        using (var writer = new StringsWriter(GameRelease.SkyrimSE, modKey, existingPath, encodingProvider, fileSystem))
        {
            writer.Register(englishStr, Language.English, StringsSource.Normal);
            writer.Register(frenchStr, Language.French, StringsSource.Normal);
            writer.Register(germanStr, Language.German, StringsSource.IL);
        }

        var languageFormat = GameConstants.Get(GameRelease.SkyrimSE).StringsLanguageFormat!.Value;
        fileSystem.File.Delete(Path.Combine(existingPath.Path, StringsUtility.GetFileName(languageFormat, modKey, Language.English, StringsSource.IL)));
        fileSystem.File.Delete(Path.Combine(existingPath.Path, StringsUtility.GetFileName(languageFormat, modKey, Language.French, StringsSource.IL)));
        fileSystem.File.Delete(Path.Combine(existingPath.Path, StringsUtility.GetFileName(languageFormat, modKey, Language.German, StringsSource.Normal)));
        fileSystem.File.Delete(Path.Combine(existingPath.Path, StringsUtility.GetFileName(languageFormat, modKey, Language.English, StringsSource.DL)));
        fileSystem.File.Delete(Path.Combine(existingPath.Path, StringsUtility.GetFileName(languageFormat, modKey, Language.French, StringsSource.DL)));
        fileSystem.File.Delete(Path.Combine(existingPath.Path, StringsUtility.GetFileName(languageFormat, modKey, Language.German, StringsSource.DL)));

        var overlay = factory.InternalFactory(modKey);

        overlay.Empty.ShouldBeFalse();

        var normalLanguages = overlay.AvailableLanguages(StringsSource.Normal);
        normalLanguages.ShouldContain(Language.English);
        normalLanguages.ShouldContain(Language.French);
        
        var ilLanguages = overlay.AvailableLanguages(StringsSource.IL);
        ilLanguages.ShouldContain(Language.German);

        overlay.AvailableLanguages(StringsSource.DL).ShouldBeEmpty();
    }

    [Theory, MutagenModAutoData]
    internal void CreateString(IFileSystem fileSystem,
        ModKey modKey,
        DirectoryPath existingPath,
        MutagenEncodingProvider encodingProvider,
        string englishStr,
        string frenchStr,
        IGameReleaseContext gameRelease,
        IDataDirectoryProvider dataDirectoryProvider,
        IGetApplicableArchivePaths getApplicableArchivePaths)
    {
        var factory = new StringsFolderLookupFactory(gameRelease, dataDirectoryProvider, getApplicableArchivePaths, fileSystem, new StringsReadParameters()
        {
            StringsFolderOverride = existingPath
        });
        uint stringId;
        using (var writer = new StringsWriter(GameRelease.SkyrimSE, modKey, existingPath, encodingProvider, fileSystem))
        {
            stringId = writer.Register(StringsSource.Normal,
                new KeyValuePair<Language, string>(Language.English, englishStr),
                new KeyValuePair<Language, string>(Language.French, frenchStr));
        }
        var overlay = factory.InternalFactory(modKey);
    
        var translatedString = overlay.CreateString(StringsSource.Normal, stringId, Language.English);
    
        translatedString.ShouldNotBeNull();
        translatedString.TargetLanguage.ShouldBe(Language.English);
        translatedString.StringsKey.ShouldBe(stringId);
        translatedString.String.ShouldBe(englishStr);
    }
    
    [Theory, MutagenModAutoData]
    internal void MultipleLanguagesAndSources(IFileSystem fileSystem,
        ModKey modKey,
        DirectoryPath existingPath,
        MutagenEncodingProvider encodingProvider,
        string englishNormal,
        string frenchNormal,
        string englishIL,
        string englishDL,
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
            normalId = writer.Register(englishNormal, Language.English, StringsSource.Normal);
            writer.Register(frenchNormal, Language.French, StringsSource.Normal);
            ilId = writer.Register(englishIL, Language.English, StringsSource.IL);
            dlId = writer.Register(englishDL, Language.English, StringsSource.DL);
        }

        fileSystem.File.Delete(Path.Combine(existingPath.Path, StringsUtility.GetFileName(GameConstants.Get(GameRelease.SkyrimSE).StringsLanguageFormat!.Value, modKey, Language.French, StringsSource.IL)));
        fileSystem.File.Delete(Path.Combine(existingPath.Path, StringsUtility.GetFileName(GameConstants.Get(GameRelease.SkyrimSE).StringsLanguageFormat!.Value, modKey, Language.French, StringsSource.DL)));

        var overlay = factory.InternalFactory(modKey);
    
        overlay.TryLookup(StringsSource.Normal, Language.English, normalId, out var str)
            .ShouldBeTrue();
        str.ShouldBe(englishNormal);
    
    
        overlay.TryLookup(StringsSource.IL, Language.English, ilId, out str)
            .ShouldBeTrue();
        str.ShouldBe(englishIL);
    
        overlay.TryLookup(StringsSource.DL, Language.English, dlId, out str)
            .ShouldBeTrue();
        str.ShouldBe(englishDL);
    
        var normalLanguages = overlay.AvailableLanguages(StringsSource.Normal);
        normalLanguages.ShouldContain(Language.English);
    
        var ilLanguages = overlay.AvailableLanguages(StringsSource.IL);
        ilLanguages.Count.ShouldBe(1);
        ilLanguages.ShouldContain(Language.English);
    
        var dlLanguages = overlay.AvailableLanguages(StringsSource.DL);
        dlLanguages.Count.ShouldBe(1);
        dlLanguages.ShouldContain(Language.English);
    }
    
    [Theory, MutagenModAutoData]
    internal void LooseStringsPriorityOverBSA_CaseSensitivityFix(IFileSystem fileSystem,
        DirectoryPath existingPath,
        MutagenEncodingProvider encodingProvider,
        string looseStringContent,
        IGameReleaseContext gameRelease,
        IDataDirectoryProvider dataDirectoryProvider)
    {
        var modKey = ModKey.FromFileName("SKYRIM.esm");
    
        uint looseStringId;
        using (var writer = new StringsWriter(GameRelease.SkyrimSE, modKey, existingPath, encodingProvider, fileSystem))
        {
            looseStringId = writer.Register(looseStringContent, Language.English, StringsSource.Normal);
        }
    
        var mockGetApplicableArchivePaths = Substitute.For<IGetApplicableArchivePaths>();
        mockGetApplicableArchivePaths.Get(modKey).Returns([]);
    
        var factory = new StringsFolderLookupFactory(gameRelease, dataDirectoryProvider, mockGetApplicableArchivePaths, fileSystem, new StringsReadParameters()
        {
            StringsFolderOverride = existingPath
        });
    
        var overlay = factory.InternalFactory(modKey);
    
        overlay.TryLookup(StringsSource.Normal, Language.English, looseStringId, out var retrievedString, out var sourcePath)
            .ShouldBeTrue();
    
        retrievedString.ShouldBe(looseStringContent);
        sourcePath.ShouldNotBeNull();
        sourcePath.ShouldContain(existingPath.Path);
        sourcePath.ShouldContain("SKYRIM_english.strings");
    }
    
    [Theory, MutagenModAutoData]
    internal void LooseStringsOnly_ShouldWork(IFileSystem fileSystem,
        DirectoryPath existingPath,
        MutagenEncodingProvider encodingProvider,
        string looseStringContent,
        IGameReleaseContext gameRelease,
        IDataDirectoryProvider dataDirectoryProvider)
    {
        var modKey = ModKey.FromFileName("Skyrim.esm");
    
        var mockGetApplicableArchivePaths = Substitute.For<IGetApplicableArchivePaths>();
        mockGetApplicableArchivePaths.Get(modKey).Returns([]);
    
        var factory = new StringsFolderLookupFactory(gameRelease, dataDirectoryProvider, mockGetApplicableArchivePaths, fileSystem, new StringsReadParameters()
        {
            StringsFolderOverride = existingPath
        });
    
        uint looseStringId;
    
        using (var writer = new StringsWriter(GameRelease.SkyrimSE, modKey, existingPath, encodingProvider, fileSystem))
        {
            looseStringId = writer.Register(looseStringContent, Language.English, StringsSource.Normal);
        }
    
        var overlay = factory.InternalFactory(modKey);
    
        overlay.TryLookup(StringsSource.Normal, Language.English, looseStringId, out var retrievedString, out var sourcePath)
            .ShouldBeTrue();
    
        retrievedString.ShouldBe(looseStringContent);
        sourcePath.ShouldContain(existingPath.Path);
        sourcePath.ShouldContain("skyrim_english.strings");
    }
}