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

    internal class ModListingParser : IModListingParser
    {
        private readonly bool _enabledMarkerProcessing;

        public ModListingParser(bool enabledMarkerProcessing)
        {
            _enabledMarkerProcessing = enabledMarkerProcessing;
        }
        
        /// <inheritdoc />
        public bool TryFromString(ReadOnlySpan<char> str, [MaybeNullWhen(false)] out ModListing listing)
        {
            str = str.Trim();
            bool enabled = true;
            if (_enabledMarkerProcessing)
            {
                if (str[0] == '*')
                {
                    str = str[1..];
                }
                else
                {
                    enabled = false;
                }
            }
            if (ModKey.TryFromNameAndExtension(str, out var key))
            {
                listing = new ModListing(key, enabled);
                return true;
            }

            var periodIndex = str.LastIndexOf('.');
            if (periodIndex == -1)
            {
                listing = default;
                return false;
            }
            var ghostTerm = str.Slice(periodIndex + 1);
            str = str.Slice(0, periodIndex);

            if (ModKey.TryFromNameAndExtension(str, out key))
            {
                listing = ModListing.CreateGhosted(key, ghostTerm.ToString());
                return true;
            }

            listing = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryFromFileName(FileName fileName, [MaybeNullWhen(false)] out ModListing listing)
        {
            return TryFromString(fileName.String,  out listing);
        }

        /// <inheritdoc />
        public ModListing FromString(ReadOnlySpan<char> str)
        {
            if (!TryFromString(str, out var listing))
            {
                throw new InvalidDataException($"Load order file had malformed line: {str.ToString()}");
            }
            return listing;
        }

        /// <inheritdoc />
        public ModListing FromFileName(FileName name)
        {
            if (!TryFromFileName(name, out var listing))
            {
                throw new InvalidDataException($"Load order file had malformed line: {name}");
            }
            return listing;
        }
    }
}