using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda.Strings;

/// <summary>
/// Interface for looking up TranslatedStrings contents from a single source
/// </summary>
public interface IStringsLookup : IEnumerable<KeyValuePair<uint, string>>
{
    /// <summary>
    /// Attempts to retrieve a string given its index key
    /// </summary>
    /// <param name="key">Index key to look up</param>
    /// <param name="str">String if retrieved</param>
    /// <returns>True if string was located</returns>
    bool TryLookup(uint key, [MaybeNullWhen(false)] out string str);

    /// <summary>
    /// Attempts to retrieve a string given its index key
    /// </summary>
    /// <param name="key">Index key to look up</param>
    /// <returns>String if located, otherwise null</returns>
    string? Lookup(uint key)
    {
        if (TryLookup(key, out var str))
        {
            return str;
        }
        return null;
    }
}