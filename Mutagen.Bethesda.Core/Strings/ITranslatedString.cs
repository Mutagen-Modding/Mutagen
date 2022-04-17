using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda.Strings;

/// <summary>
/// A readonly string that can be represented in different languages
/// </summary>
public interface ITranslatedStringGetter : IEnumerable<KeyValuePair<Language, string>>
{
    /// <summary>
    /// Language the string is targeting, which will be set/return when accessed normally
    /// </summary>
    Language TargetLanguage { get; }

    /// <summary>
    /// String for the language stored in TranslatedString.DefaultLanguage
    /// </summary>
    string? String { get; set; }

    /// <summary>
    /// Attempts to retrieve a string for a specific language
    /// Default language will always at least succeed and return string.Empty, never null.
    /// </summary>
    /// <param name="language">Language to query</param>
    /// <param name="str">String if located</param>
    /// <returns>True if string was located for given language</returns>
    bool TryLookup(Language language, [MaybeNullWhen(false)] out string str);

    TranslatedString DeepCopy();

    /// <summary>
    /// Number of languages registered
    /// </summary>
    int NumLanguages { get; }
}

/// <summary>
/// A string that can be represented in different languages
/// </summary>
public interface ITranslatedString : ITranslatedStringGetter
{
    /// <summary>
    /// Sets a string for a specific language
    /// </summary>
    /// <param name="language">Language to register string under</param>
    /// <param name="str">String to register</param>
    void Set(Language language, string str);

    /// <summary>
    /// Removes a non-default language.
    /// If the default language is provided, no change will be applied.
    /// </summary>
    /// <param name="language">Language to remove</param>
    void RemoveNonDefault(Language language);

    /// <summary>
    /// Clears all non-default language string registrations
    /// </summary>
    void ClearNonDefault();

    /// <summary>
    /// Clears all language registrations, and sets the default string to empty.
    /// </summary>
    void Clear();

    /// <summary>
    /// String for the language stored in TranslatedString.DefaultLanguage
    /// </summary>
    new string? String { get; set; }
}

public static class TranslatedStringExt
{
    /// <summary>
    /// Attempts to retrieve a string for a specific language.
    /// Default language will always at least return string.Empty, never null.
    /// </summary>
    /// <param name="getter">Translated string to look up on</param>
    /// <param name="language">Language to query</param>
    /// <returns>String if located, otherwise null</returns>
    public static string? Lookup(this ITranslatedStringGetter getter, Language language)
    {
        if (getter.TryLookup(language, out var str))
        {
            return str;
        }
        if (language == getter.TargetLanguage) return string.Empty;
        return null;
    }
}