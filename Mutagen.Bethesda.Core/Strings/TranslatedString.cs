using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Strings;

/// <summary>
/// A string that can be represented in multiple different languages.<br/>
/// Threadsafe.
/// </summary>
[DebuggerDisplay("{String}")]
public sealed class TranslatedString : ITranslatedString, IEquatable<TranslatedString>, IOptionalStringsKeyGetter
{
    /// <summary>
    /// Whether to only consider the DefaultLanguage in equality/hash comparisons.
    /// This is not good to change during a program's execution.  It should only be 
    /// modified early during initialization.
    /// </summary>
    public static bool DefaultLanguageComparisonOnly = true;

    private static Language _defaultLanguage;

    /// <summary>
    /// The default language to use as the main target language
    /// </summary>
    public static Language DefaultLanguage
    {
        get => _defaultLanguage;
        set
        {
            _defaultLanguage = value;
            _empty = new(value, string.Empty);
        }
    }

    /// <summary>
    /// Language the string is targeting, and will be set/return when accessed normally
    /// </summary>
    public Language TargetLanguage { get; }

    private string? _directString;
    private readonly object _lock = new();
    internal Dictionary<Language, string?>? _localization;

    // Alternate way of populating a Translated String
    // Will cause it to act in a lazy lookup fashion
    public uint? StringsKey { get; internal set; }
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

    public int NumLanguages => _localization?.Count ?? 1;

    private static TranslatedString _empty = new(Language.English, string.Empty);
    public static ITranslatedStringGetter Empty => _empty;

    /// <summary>
    /// Creates a translated string with empty string set for the default language
    /// </summary>
    /// <param name="language">Optional target language override</param>
    public TranslatedString(Language language)
    {
        TargetLanguage = language;
    }

    /// <summary>
    /// Creates a translated string with a value for the default language
    /// </summary>
    /// <param name="directString">String to register for the default language</param>
    public TranslatedString(Language targetLanguage, string? directString)
    {
        _directString = directString;
        TargetLanguage = targetLanguage;
    }

    /// <summary>
    /// Creates a translated string with a number of strings for languages.
    /// If no string is provided for the default language, string.Empty will be assigned.
    /// </summary>
    /// <param name="strs">Language string pairs to register</param>
    public TranslatedString(Language targetLanguage, IEnumerable<KeyValuePair<Language, string>> strs)
    {
        _localization = new Dictionary<Language, string?>();
        foreach (var str in strs)
        {
            _localization[str.Key] = str.Value;
        }
        TargetLanguage = targetLanguage;
    }

    /// <summary>
    /// Creates a translated string with a number of strings for languages.
    /// If no string is provided for the default language, string.Empty will be assigned.
    /// </summary>
    /// <param name="targetLanguage">Target language override</param>
    /// <param name="strs">Language string pairs to register</param>
    public TranslatedString(Language targetLanguage, params KeyValuePair<Language, string>[] strs)
        : this(targetLanguage, (IEnumerable<KeyValuePair<Language, string>>)strs)
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
                if (StringsLookup.TryLookup(StringsSource, language, StringsKey!.Value, out str))
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
                if (language == TargetLanguage
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
        _directString = null;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
                if (StringsLookup.TryLookup(StringsSource, lang, StringsKey!.Value, out var str))
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
        return new TranslatedString(DefaultLanguage, str);
    }

    public static implicit operator string?(TranslatedString? str)
    {
        return str?.String;
    }

    public override string ToString()
    {
        return String ?? string.Empty;
    }

    public TranslatedString DeepCopy()
    {
        if (_directString == null)
        {
            return new TranslatedString(
                targetLanguage: TargetLanguage,
                strs: this.ToArray());
        }
        else
        {
            return new TranslatedString(
                targetLanguage: TargetLanguage,
                directString: _directString);
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

internal sealed class TranslatedStringOnlyDefaultComparer : IEqualityComparer<ITranslatedStringGetter>
{
    public bool Equals(ITranslatedStringGetter? x, ITranslatedStringGetter? y)
    {
        if (ReferenceEquals(x, y)) return true;
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

internal sealed class TranslatedStringComparer : IEqualityComparer<ITranslatedStringGetter>
{
    public bool Equals(ITranslatedStringGetter? x, ITranslatedStringGetter? y)
    {
        if (ReferenceEquals(x, y)) return true;
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