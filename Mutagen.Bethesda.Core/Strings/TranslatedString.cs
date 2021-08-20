using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Strings
{
    /// <summary>
    /// A string that can be represented in multiple different languages.<br/>
    /// Threadsafe.
    /// </summary>
    public class TranslatedString : ITranslatedString, IEquatable<TranslatedString>
    {
        /// <summary>
        /// Whether to only consider the DefaultLanguage in equality/hash comparisons.
        /// This is not good to change during a program's execution.  It should only be 
        /// modified early during initialization.
        /// </summary>
        public static bool DefaultLanguageComparisonOnly = true;

        /// <summary>
        /// The default language to use as the main target language
        /// </summary>
        public static Language DefaultLanguage = Language.English;

        /// <summary>
        /// Language the string is targeting, and will be set/return when accessed normally
        /// </summary>
        public Language TargetLanguage { get; }

        private string? _directString;
        private readonly object _lock = new();
        internal Dictionary<Language, string?>? _localization;

        // Alternate way of populating a Translated String
        // Will cause it to act in a lazy lookup fashion
        internal uint Key;
        internal IStringsFolderLookup? StringsLookup;
        internal StringsSource StringsSource;

        internal bool UsingLocalizationDictionary => _localization != null || StringsLookup != null;

        /// <inheritdoc />
        public string? String
        {
            get
            {
                lock (_lock)
                {
                    if (!UsingLocalizationDictionary) return _directString;
                    if (TryLookup(TargetLanguage, out var str))
                    {
                        return str;
                    }
                    return null;
                }
            }
            set => Set(TargetLanguage, value);
        }

        public int NumLanguages
        {
            get
            {
                return StringsLookup?.AvailableLanguages(StringsSource).Count ?? 1;
            }
        }

        private readonly static TranslatedString _empty = new TranslatedString(string.Empty);
        public static ITranslatedStringGetter Empty => _empty;

        /// <summary>
        /// Creates a translated string with empty string set for the default language
        /// </summary>
        /// <param name="language">Optional target language override</param>
        public TranslatedString(Language? language = null)
        {
            TargetLanguage = language ?? DefaultLanguage;
        }

        /// <summary>
        /// Creates a translated string with a value for the default language
        /// </summary>
        /// <param name="directString">String to register for the default language</param>
        /// <param name="language">Optional target language override</param>
        public TranslatedString(string? directString, Language? language = null)
        {
            _directString = directString;
            TargetLanguage = language ?? DefaultLanguage;
        }

        /// <summary>
        /// Creates a translated string with a number of strings for languages.
        /// If no string is provided for the default language, string.Empty will be assigned.
        /// </summary>
        /// <param name="strs">Language string pairs to register</param>
        /// <param name="language">Optional target language override</param>
        public TranslatedString(IEnumerable<KeyValuePair<Language, string>> strs, Language? language = null)
        {
            _localization = new Dictionary<Language, string?>();
            foreach (var str in strs)
            {
                _localization[str.Key] = str.Value;
            }
            TargetLanguage = language ?? DefaultLanguage;
        }

        /// <summary>
        /// Creates a translated string with a number of strings for languages.
        /// If no string is provided for the default language, string.Empty will be assigned.
        /// </summary>
        /// <param name="language">Target language override</param>
        /// <param name="strs">Language string pairs to register</param>
        public TranslatedString(Language language, params KeyValuePair<Language, string>[] strs)
            : this((IEnumerable<KeyValuePair<Language, string>>)strs, language)
        {
        }

        /// <summary>
        /// Creates a translated string with a number of strings for languages.
        /// If no string is provided for the default language, string.Empty will be assigned.
        /// </summary>
        /// <param name="strs">Language string pairs to register</param>
        public TranslatedString(params KeyValuePair<Language, string>[] strs)
            : this((IEnumerable<KeyValuePair<Language, string>>)strs, language: null)
        {
        }

        /// <inheritdoc />
        public bool TryLookup(Language language, [MaybeNullWhen(false)] out string str)
        {
            lock (_lock)
            {
                if (TargetLanguage == language
                    && !UsingLocalizationDictionary)
                {
                    str = _directString;
                    return str != null;
                }

                if (_localization != null
                    && _localization.TryGetValue(language, out str))
                {
                    return str != null;
                }
                else if (StringsLookup != null)
                {
                    if (_localization == null)
                    {
                        _localization = new Dictionary<Language, string?>();
                    }
                    if (StringsLookup.TryLookup(this.StringsSource, language, this.Key, out str))
                    {
                        _localization[language] = str;
                        return true;
                    }
                    else
                    {
                        _localization[language] = null;
                        return false;
                    }
                }
            }

            str = default;
            return false;
        }

        /// <summary>
        /// Sets the string for a specific language
        /// </summary>
        /// <param name="language">Language to register the string under</param>
        /// <param name="str">String to register</param>
        public void Set(Language language, string? str)
        {
            lock (_lock)
            {
                if (_localization == null)
                {
                    if (language == DefaultLanguage
                        && !UsingLocalizationDictionary)
                    {
                        _directString = str;
                        return;
                    }

                    _localization = CreateLocalization();
                }
                _localization[language] = str;
            }
        }

        /// <inheritdoc />
        public void RemoveNonDefault(Language language)
        {
            if (language == TargetLanguage) return;
            lock (_lock)
            {
                if (_localization == null)
                {
                    _localization = CreateLocalization();
                }
                _localization[language] = null;
            }
        }

        private Dictionary<Language, string?> CreateLocalization()
        {
            var ret = new Dictionary<Language, string?>();

            // Swap direct string to the internal setup where it's stored in the dictionary
            if (_directString != null)
            {
                ret[TargetLanguage] = _directString;
                _directString = null;
            }
            return ret;
        }

        /// <inheritdoc />
        public void ClearNonDefault()
        {
            lock (_lock)
            {
                if (_localization == null) return;
                if (!_localization.TryGetValue(TargetLanguage, out _directString))
                {
                    _directString = null;
                }
                _localization = null;
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            ClearNonDefault();
            this._directString = null;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public IEnumerator<KeyValuePair<Language, string>> GetEnumerator()
        {
            lock (_lock)
            {
                ResolveAllStringSources();
                if (_localization == null)
                {
                    if (_directString != null)
                    {
                        yield return new KeyValuePair<Language, string>(TargetLanguage, _directString);
                    }
                    yield break;
                }
                else
                {
                    foreach (var item in _localization)
                    {
                        if (item.Value == null) continue;
                        yield return new KeyValuePair<Language, string>(item.Key, item.Value);
                    }
                }
            }
        }

        internal void ResolveAllStringSources()
        {
            lock (_lock)
            {
                if (StringsLookup == null) return;
                if (_localization == null)
                {
                    _localization = new Dictionary<Language, string?>();
                }
                foreach (var lang in StringsLookup.AvailableLanguages(StringsSource))
                {
                    if (_localization.ContainsKey(lang)) continue;
                    if (StringsLookup.TryLookup(StringsSource, lang, Key, out var str))
                    {
                        _localization[lang] = str;
                    }
                    else
                    {
                        _localization[lang] = null;
                    }
                }
                StringsLookup = null;
            }
        }

        public static implicit operator TranslatedString(string? str)
        {
            return new TranslatedString(str);
        }

        public static implicit operator string?(TranslatedString? str)
        {
            return str?.String;
        }

        public override string ToString()
        {
            return this.String ?? string.Empty;
        }

        public TranslatedString DeepCopy()
        {
            if (_directString == null)
            {
                return new TranslatedString(
                    language: this.TargetLanguage,
                    strs: this.ToArray());
            }
            else
            {
                return new TranslatedString(
                    language: this.TargetLanguage,
                    directString: this._directString);
            }
        }

        public static readonly IEqualityComparer<ITranslatedStringGetter> OnlyDefaultComparer = new TranslatedStringOnlyDefaultComparer();

        public static readonly IEqualityComparer<ITranslatedStringGetter> AllLanguageComparer = new TranslatedStringComparer();

        public override bool Equals(object? obj)
        {
            if (obj is not TranslatedString str) return false;
            return Equals(str);
        }

        public override int GetHashCode()
        {
            return (DefaultLanguageComparisonOnly ? OnlyDefaultComparer : AllLanguageComparer).GetHashCode(this);
        }

        public bool Equals(TranslatedString? other)
        {
            return (DefaultLanguageComparisonOnly ? OnlyDefaultComparer : AllLanguageComparer).Equals(this, other);
        }
    }

    class TranslatedStringOnlyDefaultComparer : IEqualityComparer<ITranslatedStringGetter>
    {
        public bool Equals(ITranslatedStringGetter? x, ITranslatedStringGetter? y)
        {
            if (object.ReferenceEquals(x, y)) return true;
            if (x?.TargetLanguage != y?.TargetLanguage) return false;
            return string.Equals(x?.String, y?.String);
        }

        public int GetHashCode(ITranslatedStringGetter obj)
        {
            var ret = new HashCode();
            ret.Add(obj.String);
            ret.Add(obj.TargetLanguage);
            return ret.ToHashCode();
        }
    }

    class TranslatedStringComparer : IEqualityComparer<ITranslatedStringGetter>
    {
        public bool Equals(ITranslatedStringGetter? x, ITranslatedStringGetter? y)
        {
            if (object.ReferenceEquals(x, y)) return true;
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.NumLanguages != y.NumLanguages) return false;
            return x.OrderBy(kv => kv.Key).SequenceEqual(y.OrderBy(kv => kv.Key));
        }

        public int GetHashCode(ITranslatedStringGetter obj)
        {
            var code = new HashCode();
            foreach (var kv in obj)
            {
                code.Add(kv);
            }
            return code.ToHashCode();
        }
    }
}
