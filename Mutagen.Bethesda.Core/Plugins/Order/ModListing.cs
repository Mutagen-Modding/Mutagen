using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    /// <summary>
    /// Class associating a ModKey with a Mod object that may or may not exist.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public record ModListing : IModListingGetter
    {
        /// <inheritdoc />
        public ModKey ModKey { get; init; }

        /// <inheritdoc />
        public bool Enabled { get; init; }

        /// <inheritdoc />
        public bool Ghosted => GhostSuffix != null;

        /// <inheritdoc />
        public string? GhostSuffix { get; init; }

        public ModListing()
        {
        }

        public ModListing(ModKey modKey, bool enabled, string? ghostSuffix = null)
        {
            ModKey = modKey;
            Enabled = enabled;
            GhostSuffix = ghostSuffix;
        }

        public static ModListing CreateEnabled(ModKey modKey)
        {
            return new ModListing(modKey, enabled: true, ghostSuffix: null);
        }

        public static ModListing CreateDisabled(ModKey modKey)
        {
            return new ModListing(modKey, enabled: false, ghostSuffix: null);
        }

        public static ModListing CreateGhosted(ModKey modKey, string ghostSuffix)
        {
            return new ModListing(modKey, enabled: false, ghostSuffix: ghostSuffix);
        }

        public override string ToString()
        {
            return $"[{(Enabled ? "X" : "_")}] {ModKey}{(Ghosted ? " (ghosted)" : null)}";
        }

        public static implicit operator ModListing(ModKey modKey)
        {
            return new ModListing(modKey, enabled: true);
        }

        public static bool TryFromString(ReadOnlySpan<char> str, bool enabledMarkerProcessing, [MaybeNullWhen(false)] out ModListing listing)
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
                listing = CreateGhosted(key, ghostTerm.ToString());
                return true;
            }

            listing = default;
            return false;
        }

        public static bool TryFromFileName(FileName fileName, bool enabledMarkerProcessing, [MaybeNullWhen(false)] out ModListing listing)
        {
            return TryFromString(fileName.String, enabledMarkerProcessing, out listing);
        }

        public static ModListing FromString(ReadOnlySpan<char> str, bool enabledMarkerProcessing)
        {
            if (!TryFromString(str, enabledMarkerProcessing, out var listing))
            {
                throw new ArgumentException($"Load order file had malformed line: {str.ToString()}");
            }
            return listing;
        }

        public static ModListing FromFileName(FileName name, bool enabledMarkerProcessing)
        {
            if (!TryFromFileName(name, enabledMarkerProcessing, out var listing))
            {
                throw new ArgumentException($"Load order file had malformed line: {name}");
            }
            return listing;
        }

        public static Comparer<ModListing> GetComparer(Comparer<ModKey> comparer) =>
            Comparer<ModListing>.Create((x, y) => comparer.Compare(x.ModKey, y.ModKey));

        public static Comparer<TListing> GetComparer<TListing>(Comparer<ModKey> comparer)
            where TListing : IModListingGetter
        {
            return Comparer<TListing>.Create((x, y) => comparer.Compare(x.ModKey, y.ModKey));
        }
    }

    /// <summary>
    /// Class associating a ModKey with a Mod object that may or may not exist.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public record ModListing<TMod> : ModListing, IModListingGetter<TMod>
        where TMod : class, IModGetter
    {
        /// <inheritdoc />
        public TMod? Mod { get; init; }

        private ModListing(ModKey key, TMod? mod, bool enabled, string? ghostSuffix = null)
        {
            this.ModKey = key;
            this.Mod = mod;
            this.Enabled = enabled;
            this.GhostSuffix = ghostSuffix;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ModListing(TMod mod, bool enabled = true, string? ghostSuffix = null)
        {
            this.ModKey = mod.ModKey;
            this.Mod = mod;
            this.Enabled = enabled;
            this.GhostSuffix = ghostSuffix;
        }

        /// <summary>
        /// Factory to create a ModListing which does not have a mod object
        /// </summary>
        /// <param name="key">ModKey to associate with listing</param>
        /// <param name="enabled">Whether the listing is enabled in the load order</param>
        /// <param name="ghostSuffix">
        /// What file suffix is used if ghosted.  This is done by modifying the file type to be anything abnormal.<br/>
        /// This is the same as disabling a mod as far as the game is concerned, but also is a hint to modmanagers to treat 
        /// the mods differently depending on the context
        /// </param>
        /// <returns>ModListing with no mod object</returns>
        public static ModListing<TMod> CreateUnloaded(ModKey key, bool enabled, string? ghostSuffix = null)
        {
            return new ModListing<TMod>(key, default, enabled: enabled, ghostSuffix: ghostSuffix);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[{(Enabled ? "X" : "_")}] {ModKey}{(Mod == null ? " (missing)" : null)}{(Ghosted ? " (ghosted)" : null)}";
        }

        public void Dispose()
        {
            if (Mod is IDisposable disp)
            {
                disp.Dispose();
            }
        }
    }

    public interface IModListingGetter<out TMod> : IModListingGetter, IDisposable
        where TMod : class, IModGetter
    {
        /// <summary>
        /// Mod object
        /// </summary>
        TMod? Mod { get; }
    }

    public interface IModListingGetter : IModKeyed
    {
        /// <summary>
        /// Whether the listing is enabled in the load order
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Whether the listing is "ghosted".  This is done by modifying the file type to be anything abnormal.<br/>
        /// This is the same as disabling a mod as far as the game is concerned, but also is a hint to modmanagers to treat 
        /// the mods differently depending on the context
        /// </summary>
        bool Ghosted { get; }

        /// <summary>
        /// What file suffix is used if ghosted.  This is done by modifying the file type to be anything abnormal.<br/>
        /// This is the same as disabling a mod as far as the game is concerned, but also is a hint to modmanagers to treat 
        /// the mods differently depending on the context
        /// </summary>
        string? GhostSuffix { get; }
    }
}
