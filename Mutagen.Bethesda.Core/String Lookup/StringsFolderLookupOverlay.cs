using Compression.BSA;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Wabbajack.Common;

namespace Mutagen.Bethesda
{
    public class StringsFolderLookupOverlay : IStringsFolderLookup
    {
        private readonly Lazy<DictionaryBundle> _dictionaries;

        public string DataPath { get; }
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

        private StringsFolderLookupOverlay(Lazy<DictionaryBundle> instantiator, string dataPath, ModKey modKey)
        {
            _dictionaries = instantiator;
            DataPath = dataPath;
            ModKey = modKey;
        }

        public static StringsFolderLookupOverlay TypicalFactory(GameRelease release, string dataPath, StringsReadParameters? instructions, ModKey modKey)
        {
            var stringsFolderPath = instructions?.StringsFolderOverride;
            if (stringsFolderPath == null)
            {
                stringsFolderPath = Path.Combine(dataPath, "Strings");
            }
            return new StringsFolderLookupOverlay(new Lazy<DictionaryBundle>(
                isThreadSafe: true,
                valueFactory: () =>
                {
                    var bundle = new DictionaryBundle();
                    if (stringsFolderPath.Value.Exists)
                    {
                        foreach (var file in stringsFolderPath.Value.Info.EnumerateFiles($"{modKey.Name}*{StringsUtility.StringsFileExtension}"))
                        {
                            if (!StringsUtility.TryRetrieveInfoFromString(file.Name, out var type, out var lang, out _)) continue;
                            var dict = bundle.Get(type);
                            dict[lang] = new Lazy<IStringsLookup>(() => new StringsLookupOverlay(file.FullName, type), LazyThreadSafetyMode.ExecutionAndPublication);
                        }
                    }
                    foreach (var bsaFile in Directory.EnumerateFiles(dataPath, "*.bsa"))
                    {
                        try
                        {
                            var bsaReader = Archive.CreateReader(release, bsaFile);
                            if (!bsaReader.TryGetFolder("strings", out var stringsFolder)) continue;
                            try
                            {
                                foreach (var item in stringsFolder.Files)
                                {
                                    if (!StringsUtility.TryRetrieveInfoFromString(Path.GetFileName(item.Path.ToString()), out var type, out var lang, out var modName)) continue;
                                    if (!MemoryExtensions.Equals(modKey.Name, modName, StringComparison.OrdinalIgnoreCase)) continue;
                                    var dict = bundle.Get(type);
                                    if (dict.ContainsKey(lang)) continue;
                                    dict[lang] = new Lazy<IStringsLookup>(() =>
                                    {
                                        try
                                        {
                                            byte[] bytes = new byte[item.Size];
                                            using var stream = new MemoryStream(bytes);
                                            item.CopyDataTo(stream);
                                            return new StringsLookupOverlay(bytes, type);
                                        }
                                        catch (Exception ex)
                                        {
                                            throw BsaException.FolderError("String file from BSA failed to parse", ex, bsaFile, item.Path.ToString());
                                        }
                                    }, LazyThreadSafetyMode.ExecutionAndPublication);
                                }
                            }
                            catch (Exception ex)
                            {
                                throw BsaException.FolderError("BSA folder failed to parse for string file", ex, bsaFile, stringsFolder.Path ?? string.Empty);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw BsaException.OverallError("BSA failed to parse for string file", ex, bsaFile);
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

        public IEnumerable<Language> AvailableLanguages(StringsSource source)
        {
            return Get(source).Keys;
        }
    }
}
