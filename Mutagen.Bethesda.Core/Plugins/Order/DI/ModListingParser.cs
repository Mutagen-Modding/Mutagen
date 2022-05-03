using System.Diagnostics.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

/// <summary>
/// Parses a single line or filename into a ModListing object
/// </summary>
public interface ILoadOrderListingParser
{
    /// <summary>
    /// Attempts to convert from a string to a ModListing
    /// </summary>
    /// <param name="str">string to parse</param>
    /// <param name="listing">ModListing from the string, if successful</param>
    /// <returns>True if conversion successful</returns>
    bool TryFromString(ReadOnlySpan<char> str, [MaybeNullWhen(false)] out LoadOrderListing listing);
        
    /// <summary>
    /// Attempts to convert from a FileName to a ModListing
    /// </summary>
    /// <param name="fileName">FileName to parse</param>
    /// <param name="listing">ModListing from the FileName, if successful</param>
    /// <returns>True if conversion successful</returns>
    bool TryFromFileName(FileName fileName, [MaybeNullWhen(false)] out LoadOrderListing listing);
        
    /// <summary>
    /// Converts from a string to a ModListing
    /// </summary>
    /// <param name="str">string to parse</param>
    /// <returns>ModListing from the string</returns>
    /// <exception cref="InvalidDataException">If string malformed</exception>
    LoadOrderListing FromString(ReadOnlySpan<char> str);
        
    /// <summary>
    /// Converts from a FileName to a ModListing
    /// </summary>
    /// <param name="fileName">FileName to parse</param>
    /// <returns>ModListing from the FileName</returns>
    /// <exception cref="InvalidDataException">If FileName malformed</exception>
    LoadOrderListing FromFileName(FileName fileName);
}

public class LoadOrderListingParser : ILoadOrderListingParser
{
    private readonly IHasEnabledMarkersProvider _hasEnabledMarkers;

    public LoadOrderListingParser(IHasEnabledMarkersProvider hasEnabledMarkers)
    {
        _hasEnabledMarkers = hasEnabledMarkers;
    }
        
    /// <inheritdoc />
    public bool TryFromString(ReadOnlySpan<char> str, [MaybeNullWhen(false)] out LoadOrderListing listing)
    {
        return LoadOrderListing.TryFromString(str, _hasEnabledMarkers.HasEnabledMarkers, out listing);
    }

    /// <inheritdoc />
    public bool TryFromFileName(FileName fileName, [MaybeNullWhen(false)] out LoadOrderListing listing)
    {
        return LoadOrderListing.TryFromFileName(fileName, _hasEnabledMarkers.HasEnabledMarkers, out listing);
    }

    /// <inheritdoc />
    public LoadOrderListing FromString(ReadOnlySpan<char> str)
    {
        return LoadOrderListing.FromString(str, _hasEnabledMarkers.HasEnabledMarkers);
    }

    /// <inheritdoc />
    public LoadOrderListing FromFileName(FileName name)
    {
        return LoadOrderListing.FromFileName(name, _hasEnabledMarkers.HasEnabledMarkers);
    }
}