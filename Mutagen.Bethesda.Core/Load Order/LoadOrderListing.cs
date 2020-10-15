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
    }
}
