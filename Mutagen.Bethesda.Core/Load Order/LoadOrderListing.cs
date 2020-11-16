using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda
{
    [DebuggerDisplay("LoadOrderListing {ToString()}")]
    public class LoadOrderListing 
    {
        public ModKey ModKey { get; set; }
        public bool Enabled { get; set; }

        public LoadOrderListing(ModKey modKey, bool enabled)
        {
            ModKey = modKey;
            Enabled = enabled;
        }

        public override bool Equals(object? obj)
        {
            return obj is LoadOrderListing listing &&
                   ModKey.Equals(listing.ModKey) &&
                   Enabled == listing.Enabled;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ModKey, Enabled);
        }

        public override string ToString()
        {
            return $"[{(Enabled ? "X" : "_")}] {ModKey}";
        }

        public static implicit operator LoadOrderListing(ModKey modKey)
        {
            return new LoadOrderListing(modKey, enabled: true);
        }

        public static LoadOrderListing FromString(ReadOnlySpan<char> str, bool enabledMarkerProcessing)
        {
            str = str.Trim();
            bool enabled = true;
            if (enabledMarkerProcessing)
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
            if (!ModKey.TryFromNameAndExtension(str, out var key))
            {
                throw new ArgumentException($"Load order file had malformed line: {str.ToString()}");
            }
            return new LoadOrderListing(key, enabled);
        }
    }
}
