using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// Class associating a ModKey with a Mod object that may or may not exist.
    /// </summary>
    [DebuggerDisplay("ModListing {ToString()}")]
    public class ModListing<TMod> : IModListing<TMod>
        where TMod : class, IModGetter
    {
        /// <inheritdoc />
        public TMod? Mod { get; }

        /// <inheritdoc />
        public ModKey ModKey { get; }

        /// <inheritdoc />
        public bool Enabled { get; }

        private ModListing(ModKey key, TMod? mod, bool enabled)
        {
            this.ModKey = key;
            this.Mod = mod;
            this.Enabled = enabled;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ModListing(TMod mod, bool enabled)
        {
            this.ModKey = mod.ModKey;
            this.Mod = mod;
            this.Enabled = enabled;
        }

        /// <summary>
        /// Factory to create a ModListing which does not have a mod object
        /// </summary>
        /// <param name="key">ModKey to associate with listing</param>
        /// <param name="enabled">Whether the listing is enabled in the load order</param>
        /// <returns>ModListing with no mod object</returns>
        public static ModListing<TMod> UnloadedModListing(ModKey key, bool enabled)
        {
            return new ModListing<TMod>(key, default, enabled: enabled);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{ModKey} : {(Enabled ? "On" : "Off")} : {(Mod == null ? "Missing" : "Present")}";
        }

        public void Dispose()
        {
            if (Mod is IDisposable disp)
            {
                disp.Dispose();
            }
        }
    }

    public interface IModListing<out TMod> : IModListing, IDisposable
        where TMod : class, IModGetter
    {
        /// <summary>
        /// Mod object
        /// </summary>
        TMod? Mod { get; }
    }

    public interface IModListing : IModKeyed, IDisposable
    {
        /// <summary>
        /// Whether the listing is enabled in the load order
        /// </summary>
        bool Enabled { get; }
    }

    public static class IModListingExt
    {
        public static bool TryGetIfEnabled<TMod>(this LoadOrder<IModListing<TMod>> loadOrder, ModKey modKey, [MaybeNullWhen(false)] out IModListing<TMod> item)
            where TMod : class, IModGetter
        {
            if (loadOrder.TryGetValue(modKey, out var listing)
                && listing.Enabled)
            {
                item = listing;
                return true;
            }

            item = default;
            return false;
        }
    }
}
