using System;
using System.Diagnostics.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface IModListingParser
    {
        bool TryFromString(ReadOnlySpan<char> str, [MaybeNullWhen(false)] out ModListing listing);
        bool TryFromFileName(FileName fileName, [MaybeNullWhen(false)] out ModListing listing);
        ModListing FromString(ReadOnlySpan<char> str);
        ModListing FromFileName(FileName name);
    }

    internal class ModListingParser : IModListingParser
    {
        private readonly bool _enabledMarkerProcessing;

        public ModListingParser(bool enabledMarkerProcessing)
        {
            _enabledMarkerProcessing = enabledMarkerProcessing;
        }
        
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

        public bool TryFromFileName(FileName fileName, [MaybeNullWhen(false)] out ModListing listing)
        {
            return TryFromString(fileName.String,  out listing);
        }

        public ModListing FromString(ReadOnlySpan<char> str)
        {
            if (!TryFromString(str, out var listing))
            {
                throw new ArgumentException($"Load order file had malformed line: {str.ToString()}");
            }
            return listing;
        }

        public ModListing FromFileName(FileName name)
        {
            if (!TryFromFileName(name, out var listing))
            {
                throw new ArgumentException($"Load order file had malformed line: {name}");
            }
            return listing;
        }
    }
}