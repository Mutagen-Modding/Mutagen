using Mutagen.Bethesda.Plugins;
using Noggog;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Strings.DI;
using StrongInject;

namespace Mutagen.Bethesda.Strings;

[RegisterModule(typeof(MutagenStrongInjectModule))]
internal partial class StringsFolderLookupOverlayFactoryContainer : IContainer<IStringsFolderLookupFactory>
{
    [Instance] private readonly IGameReleaseContext _release;
    [Instance] private readonly IFileSystem _fileSystem;
    [Instance] private readonly IDataDirectoryProvider _dataDirectory;
    [Instance] private readonly StringsReadParameters? _readParameters;

    public StringsFolderLookupOverlayFactoryContainer(
        GameRelease release,
        IFileSystem? fileSystem,
        DirectoryPath dataDirectory,
        StringsReadParameters? readParameters)
    {
        _readParameters = readParameters;
        _release = new GameReleaseInjection(release);
        _fileSystem = fileSystem.GetOrDefault();
        _dataDirectory = new DataDirectoryInjection(dataDirectory);
        if (_readParameters?.BsaFolderOverride != null)
        {
            _dataDirectory = new DataDirectoryInjection(_readParameters.BsaFolderOverride.Value);
        }
    }
}

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
        return new StringsFolderLookupOverlayFactoryContainer(release, fileSystem, dataPath, instructions).Resolve().Value
            .InternalFactory(modKey);
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
        var ret = new TranslatedString(defaultLanguage)
        {
            StringsLookup = this,
            StringsKey = key,
            StringsSource = source,
        };
        ret.ResolveAllStringSources();
        return ret;
    }

    public IReadOnlyCollection<Language> AvailableLanguages(StringsSource source)
    {
        return Get(source).Keys;
    }
}