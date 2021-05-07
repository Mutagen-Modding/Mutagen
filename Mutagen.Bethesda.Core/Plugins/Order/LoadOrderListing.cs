using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda.Plugins.Order
{
    [DebuggerDisplay("LoadOrderListing {ToString()}")]
    public record LoadOrderListing
    {
        /// <summary>
        /// The ModKey of the listing
        /// </summary>
        public ModKey ModKey { get; init; }

        /// <summary>
        /// Whether the mod is marked to be loaded
        /// </summary>
        public bool Enabled { get; init; }

        /// <summary>
        /// Whether the listing is "ghosted".  This is done by modifying the file type to be anything abnormal.<br/>
        /// This is the same as disabling a mod as far as the game is concerned, but also is a hint to modmanagers to treat 
        /// the mods differently depending on the context
        /// </summary>
        public bool Ghosted { get; }

        public LoadOrderListing()
        {
        }

        public LoadOrderListing(ModKey modKey, bool enabled)
        {
            ModKey = modKey;
            Enabled = enabled;
        }

        private LoadOrderListing(ModKey modKey, bool enabled, bool ghosted)
        {
            ModKey = modKey;
            Enabled = enabled;
            Ghosted = ghosted;
        }

        public static LoadOrderListing CreateEnabled(ModKey modKey)
        {
            return new LoadOrderListing(modKey, enabled: false, ghosted: true);
        }

        public static LoadOrderListing CreateDisabled(ModKey modKey)
        {
            return new LoadOrderListing(modKey, enabled: false, ghosted: true);
        }

        public static LoadOrderListing CreateGhosted(ModKey modKey)
        {
            return new LoadOrderListing(modKey, enabled: false, ghosted: true);
        }

        public override string ToString()
        {
            return $"[{(Enabled ? "X" : "_")}] {ModKey}{(Ghosted ? " (ghosted)" : null)}";
        }

        public static implicit operator LoadOrderListing(ModKey modKey)
        {
            return new LoadOrderListing(modKey, enabled: true);
        }

        public static bool TryFromString(ReadOnlySpan<char> str, bool enabledMarkerProcessing, [MaybeNullWhen(false)] out LoadOrderListing listing)
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
                listing = new LoadOrderListing(key, enabled);
                return true;
            }

            var periodIndex = str.LastIndexOf('.');
            str = str.Slice(0, periodIndex);

            if (ModKey.TryFromNameAndExtension(str, out key))
            {
                listing = CreateGhosted(key);
                return true;
            }

            listing = default;
            return false;
        }

        public static LoadOrderListing FromString(ReadOnlySpan<char> str, bool enabledMarkerProcessing)
        {
            if (!TryFromString(str, enabledMarkerProcessing, out var listing))
            {
                throw new ArgumentException($"Load order file had malformed line: {str.ToString()}");
            }
            return listing;
        }

        public static Comparer<LoadOrderListing> GetComparer(Comparer<ModKey> comparer) =>
            Comparer<LoadOrderListing>.Create((x, y) => comparer.Compare(x.ModKey, y.ModKey));
    }
}
