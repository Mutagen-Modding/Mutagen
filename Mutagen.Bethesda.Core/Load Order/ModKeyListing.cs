using System;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda
{
    public interface IModListing : IModKeyed
    {
        /// <summary>
        /// Whether the listing is enabled in the load order
        /// </summary>
        bool Enabled { get; }
    }

    /// <summary>
    /// Class representing a ModKey that may or may not exist.
    /// </summary>
    [DebuggerDisplay("ModKeyListing {ToString()}")]
    public class ModKeyListing : IModListing, IEquatable<ModKeyListing>
    {
        /// <inheritdoc />
        public ModKey ModKey { get; }

        /// <inheritdoc />
        public bool Enabled { get; }

        public ModKeyListing(ModKey key, bool enabled)
        {
            this.ModKey = key;
            this.Enabled = enabled;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{ModKey} : {(Enabled ? "On" : "Off")}";
        }

        public bool Equals(ModKeyListing other)
        {
            if (Enabled != other.Enabled) return false;
            if (ModKey != other.ModKey) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ModKeyListing listing)) return false;
            return Equals(listing);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(ModKey);
            hash.Add(Enabled);
            return hash.ToHashCode();
        }
    }
}
