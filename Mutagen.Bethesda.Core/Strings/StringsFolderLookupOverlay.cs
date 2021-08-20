using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Archives.Exceptions;
using Mutagen.Bethesda.Plugins;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

namespace Mutagen.Bethesda.Strings
{
    public class StringsFolderLookupOverlay : IStringsFolderLookup
    {
        private readonly Lazy<DictionaryBundle> _dictionaries;

        public DirectoryPath DataPath { get; }
        public ModKey ModKey { get; }

        class DictionaryBundle
        {
            public readonly Dictionary<Language, Lazy<IStringsLookup>> Strings = new Dictionary<Language, Lazy<IStringsLookup>>();
            public readonly Dictionary<Language, Lazy<IStringsLookup>> DlStrings = new Dictionary<Language, Lazy<IStringsLookup>>();
            public readonly Dictionary<Language, Lazy<IStringsLookup>> IlStrings = new Dictionary<Language, Lazy<IStringsLookup>>();

            public bool Empty =>
                Strings.Count == 0
                && DlStrings.Count == 0
                && IlStrings.Count == 0;

            public Dictionary<Language, Lazy<IStringsLookup>> Get(StringsSource source)
            {
                return source switch
                {
                    StringsSource.Normal => Strings,
                    StringsSource.IL => IlStrings,
                    StringsSource.DL => DlStrings,
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

        public static StringsFolderLookupOverlay TypicalFactory(GameRelease release, ModKey modKey, DirectoryPath dataPath, StringsReadParameters? instructions)
        {
            var stringsFolderPath = instructions?.StringsFolderOverride;
            if (stringsFolderPath == null)
            {
                stringsFolderPath = Path.Combine(dataPath.Path, "Strings");
            }
            return new StringsFolderLookupOverlay(new Lazy<DictionaryBundle>(
                isThreadSafe: true,
                valueFactory: () =>
                {
                    var bundle = new DictionaryBundle();
                    if (stringsFolderPath.Value.Exists)
                    {
                        var bsaEnumer = stringsFolderPath.Value.EnumerateFiles(searchPattern: $"{modKey.Name}*{StringsUtility.StringsFileExtension}");
                        foreach (var file in bsaEnumer)
                        {
                            if (!StringsUtility.TryRetrieveInfoFromString(
                                release.GetLanguageFormat(),
                                file.Name.String, 
                                out var type,
                                out var lang, 
                                out _))
                            {
                                continue;
                            }
                            var dict = bundle.Get(type);
                            dict[lang] = new Lazy<IStringsLookup>(() => new StringsLookupOverlay(file.Path, type), LazyThreadSafetyMode.ExecutionAndPublication);
                        }
                    }
                    foreach (var bsaFile in Archive.GetApplicableArchivePaths(release, dataPath, modKey, instructions?.BsaOrdering))
                    {
                        try
                        {
                            var bsaReader = Archive.CreateReader(release, bsaFile);
                            if (!bsaReader.TryGetFolder("strings", out var stringsFolder)) continue;
                            try
                            {
                                foreach (var item in stringsFolder.Files)
                                {
                                    if (!StringsUtility.TryRetrieveInfoFromString(
                                        release.GetLanguageFormat(), 
                                        Path.GetFileName(item.Path.ToString()), 
                                        out var type, 
                                        out var lang,
                                        out var modName))
                                    {
                                        continue;
                                    }
                                    if (!MemoryExtensions.Equals(modKey.Name, modName, StringComparison.OrdinalIgnoreCase)) continue;
                                    var dict = bundle.Get(type);
                                    if (dict.ContainsKey(lang)) continue;
                                    dict[lang] = new Lazy<IStringsLookup>(() =>
                                    {
                                        try
                                        {
                                            return new StringsLookupOverlay(item.GetMemorySlice(), type);
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
            return lookup.Value.TryLookup(key, out str);
        }

        public Dictionary<Language, Lazy<IStringsLookup>> Get(StringsSource source) => _dictionaries.Value.Get(source);

        public TranslatedString CreateString(StringsSource source, uint key)
        {
            return new TranslatedString()
            {
                StringsLookup = this,
                Key = key,
                StringsSource = source,
            };
        }

        public IReadOnlyCollection<Language> AvailableLanguages(StringsSource source)
        {
            return Get(source).Keys;
        }
    }
}
