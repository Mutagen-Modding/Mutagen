using Mutagen.Bethesda.Plugins;
using Noggog;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Strings.DI;

namespace Mutagen.Bethesda.Strings;

public sealed class StringsFolderLookupOverlay : IStringsFolderLookup
{
    private readonly Lazy<DictionaryBundle> _dictionaries;

    public DirectoryPath DataPath { get; }
    public ModKey ModKey { get; }

    internal record LookupItem(IStringsLookup StringsLookup, string SourcePath);
    
    internal class DictionaryBundle
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

    internal StringsFolderLookupOverlay(Lazy<DictionaryBundle> instantiator, DirectoryPath dataPath, ModKey modKey)
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
        var factory = new StringsFolderLookupFactory(
            new DataDirectoryInjection(dataPath),
            fileSystem.GetOrDefault());
        return factory.InternalFactory(release, modKey, instructions);
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