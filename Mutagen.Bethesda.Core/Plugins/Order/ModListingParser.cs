using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    /// <summary>
    /// Parses a single line or filename into a ModListing object
    /// </summary>
    public interface IModListingParser
    {
        /// <summary>
        /// Attempts to convert from a string to a ModListing
        /// </summary>
        /// <param name="str">string to parse</param>
        /// <param name="listing">ModListing from the string, if successful</param>
        /// <returns>True if conversion successful</returns>
        bool TryFromString(ReadOnlySpan<char> str, [MaybeNullWhen(false)] out ModListing listing);
        
        /// <summary>
        /// Attempts to convert from a FileName to a ModListing
        /// </summary>
        /// <param name="fileName">FileName to parse</param>
        /// <param name="listing">ModListing from the FileName, if successful</param>
        /// <returns>True if conversion successful</returns>
        bool TryFromFileName(FileName fileName, [MaybeNullWhen(false)] out ModListing listing);
        
        /// <summary>
        /// Converts from a string to a ModListing
        /// </summary>
        /// <param name="str">string to parse</param>
        /// <returns>ModListing from the string</returns>
        /// <exception cref="InvalidDataException">If string malformed</exception>
        ModListing FromString(ReadOnlySpan<char> str);
        
        /// <summary>
        /// Converts from a FileName to a ModListing
        /// </summary>
        /// <param name="fileName">FileName to parse</param>
        /// <returns>ModListing from the FileName</returns>
        /// <exception cref="InvalidDataException">If FileName malformed</exception>
        ModListing FromFileName(FileName fileName);
    }

    public class ModListingParser : IModListingParser
    {
        private readonly IHasEnabledMarkersProvider _hasEnabledMarkers;

        public ModListingParser(IHasEnabledMarkersProvider hasEnabledMarkers)
        {
            _hasEnabledMarkers = hasEnabledMarkers;
        }
        
        /// <inheritdoc />
        public bool TryFromString(ReadOnlySpan<char> str, [MaybeNullWhen(false)] out ModListing listing)
        {
            return ModListing.TryFromString(str, _hasEnabledMarkers.HasEnabledMarkers, out listing);
        }

        /// <inheritdoc />
        public bool TryFromFileName(FileName fileName, [MaybeNullWhen(false)] out ModListing listing)
        {
            return ModListing.TryFromFileName(fileName, _hasEnabledMarkers.HasEnabledMarkers, out listing);
        }

        /// <inheritdoc />
        public ModListing FromString(ReadOnlySpan<char> str)
        {
            return ModListing.FromString(str, _hasEnabledMarkers.HasEnabledMarkers);
        }

        /// <inheritdoc />
        public ModListing FromFileName(FileName name)
        {
            return ModListing.FromFileName(name, _hasEnabledMarkers.HasEnabledMarkers);
        }
    }
}