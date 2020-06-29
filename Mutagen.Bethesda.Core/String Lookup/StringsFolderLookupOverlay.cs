using Wabbajack.Compression.BSA;
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
        private readonly Dictionary<Language, Lazy<IStringsLookup>> _strings = new Dictionary<Language, Lazy<IStringsLookup>>();
        private readonly Dictionary<Language, Lazy<IStringsLookup>> _dlstrings = new Dictionary<Language, Lazy<IStringsLookup>>();
        private readonly Dictionary<Language, Lazy<IStringsLookup>> _ilstrings = new Dictionary<Language, Lazy<IStringsLookup>>();

        public bool Empty =>
            _strings.Count == 0 
            && _dlstrings.Count == 0
            && _ilstrings.Count == 0;

        private StringsFolderLookupOverlay()
        {
        }

        public static StringsFolderLookupOverlay? TypicalFactory(string referenceModPath, StringsReadParameters? instructions, ModKey modKey)
        {
            var ret = new StringsFolderLookupOverlay();
            var stringsFolderPath = instructions?.StringsFolderOverride;
            var dir = Path.GetDirectoryName(referenceModPath);
            if (stringsFolderPath == null)
            {
                stringsFolderPath = Path.Combine(dir, "Strings");
            }
            if (stringsFolderPath.Value.Exists)
            {
                foreach (var file in stringsFolderPath.Value.Info.EnumerateFiles($"{modKey.Name}*{StringsUtility.StringsFileExtension}"))
                {
                    if (!StringsUtility.TryRetrieveInfoFromString(file.Name, out var type, out var lang, out _)) continue;
                    var dict = ret.Get(type);
                    dict[lang] = new Lazy<IStringsLookup>(() => new StringsLookupOverlay(file.FullName, type), LazyThreadSafetyMode.ExecutionAndPublication);
                }
            }
            foreach (var bsaFile in Directory.EnumerateFiles(dir, "*.bsa"))
            {
                var bsaReader = BSAReader.Load(new AbsolutePath(bsaFile, skipValidation: true));
                foreach (var item in bsaReader.Files)
                {
                    if (!StringsUtility.TryRetrieveInfoFromString(Path.GetFileName(item.Path.ToString()), out var type, out var lang, out var modName)) continue;
                    if (!MemoryExtensions.Equals(modKey.Name, modName, StringComparison.OrdinalIgnoreCase)) continue;
                    var dict = ret.Get(type);
                    if (dict.ContainsKey(lang)) continue;
                    dict[lang] = new Lazy<IStringsLookup>(() =>
                    {
                        byte[] bytes = new byte[item.Size];
                        using var stream = new MemoryStream(bytes);
                        item.CopyDataTo(stream).AsTask().Wait();
                        return new StringsLookupOverlay(bytes, type);
                    }, LazyThreadSafetyMode.ExecutionAndPublication);
                }
            }
            return ret;
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

        private Dictionary<Language, Lazy<IStringsLookup>> Get(StringsSource source)
        {
            return source switch
            {
                StringsSource.Normal => _strings,
                StringsSource.IL => _ilstrings,
                StringsSource.DL => _dlstrings,
                _ => throw new NotImplementedException(),
            };
        }

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
