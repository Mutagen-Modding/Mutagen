using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Archives.Exceptions;
using Mutagen.Bethesda.Plugins;
using Noggog;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings.DI;

namespace Mutagen.Bethesda.Strings;

public sealed class StringsFolderLookupOverlay : IStringsFolderLookup
{
    private readonly Lazy<DictionaryBundle> _dictionaries;

    public DirectoryPath DataPath { get; }
    public ModKey ModKey { get; }

    internal record LookupItem(IStringsLookup StringsLookup, string SourcePath);
    
    class DictionaryBundle
    {
        private readonly Dictionary<Language, Lazy<LookupItem>> _strings = new();
        private readonly Dictionary<Language, Lazy<LookupItem>> _dlStrings = new();
        private readonly Dictionary<Language, Lazy<LookupItem>> _ilStrings = new();

        public bool Empty =>
            _strings.Count == 0
            && _dlStrings.Count == 0
            && _ilStrings.Count == 0;

        public Dictionary<Language, Lazy<LookupItem>> Get(StringsSource source)
        {
            return source switch
            {
                StringsSource.Normal => _strings,
                StringsSource.IL => _ilStrings,
                StringsSource.DL => _dlStrings,
                _ => throw new NotImplementedException(),
            };
        }
    }

    public bool Empty => _dictionaries.Value.Empty;

    private StringsFolderLookupOverlay(Lazy<DictionaryBundle> instantiator, DirectoryPath dataPath, ModKey modKey)
    {
        _dictionaries = instantiator;
        DataPath = dataPath;
        ModKey = modKey;
    }

    // todo integrate IAssetProvider
    public static StringsFolderLookupOverlay TypicalFactory(
        GameRelease release, 
        ModKey modKey, 
        DirectoryPath dataPath,
        StringsReadParameters? instructions,
        IFileSystem? fileSystem = null)
    {
        fileSystem = fileSystem.GetOrDefault();
        var languageFormat = GameConstants.Get(release).StringsLanguageFormat ?? throw new ArgumentException($"Tried to get language format for an unsupported game: {release}", nameof(release));
        var encodings = instructions?.EncodingProvider ?? new MutagenEncodingProvider();
        var stringsFolderPath = instructions?.StringsFolderOverride;
        if (stringsFolderPath == null)
        {
            stringsFolderPath = Path.Combine(dataPath.Path, "Strings");
        }

        if (instructions?.BsaFolderOverride != null)
        {
            dataPath = instructions.BsaFolderOverride.Value;
        }
        return new StringsFolderLookupOverlay(new Lazy<DictionaryBundle>(
                isThreadSafe: true,
                valueFactory: () =>
                {
                    var bundle = new DictionaryBundle();
                    if (fileSystem.Directory.Exists(stringsFolderPath.Value))
                    {
                        var bsaEnumer = stringsFolderPath.Value.EnumerateFiles(searchPattern: $"{modKey.Name}*{StringsUtility.StringsFileExtension}", fileSystem: fileSystem);
                        foreach (var file in bsaEnumer)
                        {
                            if (!StringsUtility.TryRetrieveInfoFromString(
                                    languageFormat,
                                    file.Name.String, 
                                    out var type,
                                    out var lang, 
                                    out var modName)
                                || !modKey.Name.AsSpan().Equals(modName, StringComparison.InvariantCulture))
                            {
                                continue;
                            }
                            var dict = bundle.Get(type);
                            dict[lang] = new Lazy<LookupItem>(() =>
                                {
                                    return new LookupItem(
                                        new StringsLookupOverlay(file.Path, type, encodings.GetEncoding(release, lang), fileSystem: fileSystem),
                                        file.Path);
                                },
                                LazyThreadSafetyMode.ExecutionAndPublication);
                        }
                    }
                    foreach (var bsaFile in Archive.GetApplicableArchivePaths(
                        release, dataPath, modKey, instructions?.BsaOrdering, fileSystem: fileSystem))
                    {
                        try
                        {
                            var bsaReader = Archive.CreateReader(release, bsaFile, fileSystem);
                            if (!bsaReader.TryGetFolder("strings", out var stringsFolder)) continue;
                            try
                            {
                                foreach (var item in stringsFolder.Files)
                                {
                                    if (!StringsUtility.TryRetrieveInfoFromString(
                                            languageFormat, 
                                            Path.GetFileName(item.Path), 
                                            out var type, 
                                            out var lang,
                                            out var modName))
                                    {
                                        continue;
                                    }
                                    if (!MemoryExtensions.Equals(modKey.Name, modName, StringComparison.OrdinalIgnoreCase)) continue;
                                    var dict = bundle.Get(type);
                                    if (dict.ContainsKey(lang)) continue;
                                    dict[lang] = new Lazy<LookupItem>(() =>
                                    {
                                        try
                                        {
                                            return new LookupItem(
                                                new StringsLookupOverlay(item.GetMemorySlice(), type, encodings.GetEncoding(release, lang)),
                                                Path.Combine(bsaFile, item.Path));
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ArchiveException.EnrichWithFileAccessed("String file from BSA failed to parse", ex, item.Path);
                                        }
                                    }, LazyThreadSafetyMode.ExecutionAndPublication);
                                }
                            }
                            catch (Exception ex)
                                when (stringsFolder.Path != null)
                            {
                                throw ArchiveException.EnrichWithFolderAccessed("BSA folder failed to parse for string file", ex, stringsFolder.Path);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ArchiveException.EnrichWithArchivePath("BSA failed to parse for string file", ex, bsaFile);
                        }
                    }
                    return bundle;
                }),
            dataPath: dataPath,
            modKey: modKey);
    }

    /// <inheritdoc />
    public bool TryLookup(StringsSource source, Language language, uint key, [MaybeNullWhen(false)] out string str)
    {
        var dict = Get(source);
        if (!dict.TryGetValue(language, out var lookup))
        {
            str = default;
            return false;
        }
        return lookup.Value.StringsLookup.TryLookup(key, out str);
    }

    public bool TryLookup(
        StringsSource source,
        Language language,
        uint key,
        [MaybeNullWhen(false)] out string str,
        [MaybeNullWhen(false)] out string sourcePath)
    {
        var dict = Get(source);
        if (!dict.TryGetValue(language, out var lookup))
        {
            str = default;
            sourcePath = default;
            return false;
        }
        if (!lookup.Value.StringsLookup.TryLookup(key, out str))
        {
            sourcePath = default;
            return false;
        }

        sourcePath = lookup.Value.SourcePath;
        return true;
    }

    internal Dictionary<Language, Lazy<LookupItem>> Get(StringsSource source) => _dictionaries.Value.Get(source);

    public TranslatedString CreateString(StringsSource source, uint key, Language defaultLanguage)
    {
        return new TranslatedString(defaultLanguage)
        {
            StringsLookup = this,
            StringsKey = key,
            StringsSource = source,
        };
    }

    public IReadOnlyCollection<Language> AvailableLanguages(StringsSource source)
    {
        return Get(source).Keys;
    }
}