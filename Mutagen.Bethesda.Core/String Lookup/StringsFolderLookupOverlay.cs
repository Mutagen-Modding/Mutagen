using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

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
            var targetDir = instructions?.StringsFolderOverride;
            if (targetDir == null)
            {
                targetDir = Path.Combine(Path.GetDirectoryName(referenceModPath), "Strings");
            }
            if (targetDir.Value.Exists)
            {
                foreach (var file in targetDir.Value.Info.EnumerateFiles($"{modKey.Name}*{StringsUtility.StringsFileExtension}"))
                {
                    if (!StringsUtility.TryRetrieveInfoFromString(file.Name, out var type, out var lang)) continue;
                    var dict = ret.Get(type);
                    dict[lang] = new Lazy<IStringsLookup>(() => new StringsLookupOverlay(file.FullName, type), LazyThreadSafetyMode.ExecutionAndPublication);
                }
                return ret;
            }
            else
            {
                return null;
            }
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
            var ret = new TranslatedString();
            var dict = Get(source);
            // Avoid register dictionaries if just one strings language
            if (dict.Count == 1
                && dict.Keys.First() == TranslatedString.DefaultLanguage)
            {
                var first = dict.Values.First();
                if (first.Value.TryLookup(key, out var str))
                {
                    ret.String = str;
                    return ret;
                }
            }
            foreach (var kv in dict)
            {
                if (kv.Value.Value.TryLookup(key, out var str))
                {
                    ret.Set(kv.Key, str);
                }
            }
            return ret;
        }
    }
}
