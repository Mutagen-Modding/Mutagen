using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order;

/// <summary>
/// A Mod Listing on a load order.  Can be enabled or disabled.  Can also be "ghosted" which means
/// the listing does not end with a typical ModKey suffix
/// </summary>
public interface ILoadOrderListingGetter : IModKeyed
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
    string GhostSuffix { get; }
        
    public static string ToString(ILoadOrderListingGetter getter)
    {
        return $"[{(getter.Enabled ? "X" : "_")}] {getter.ModKey}{(getter.Ghosted ? " (ghosted)" : null)}";
    }
}

/// <inheritdoc cref="ILoadOrderListingGetter" />
[DebuggerDisplay("{ToString()}")]
public record LoadOrderListing : ILoadOrderListingGetter
{
    /// <inheritdoc />
    public ModKey ModKey { get; init; }

    /// <inheritdoc />
    public bool Enabled { get; init; }

    /// <inheritdoc />
    public bool Ghosted => !string.IsNullOrWhiteSpace(GhostSuffix);

    /// <inheritdoc />
    public string GhostSuffix { get; init; } = string.Empty;

    public LoadOrderListing()
    {
    }

    public LoadOrderListing(ModKey modKey, bool enabled, string ghostSuffix = "")
    {
        ModKey = modKey;
        Enabled = enabled;
        GhostSuffix = ghostSuffix;
    }

    public static LoadOrderListing CreateEnabled(ModKey modKey)
    {
        return new LoadOrderListing(modKey, enabled: true, ghostSuffix: "");
    }

    public static LoadOrderListing CreateDisabled(ModKey modKey)
    {
        return new LoadOrderListing(modKey, enabled: false, ghostSuffix: "");
    }

    public static LoadOrderListing CreateGhosted(ModKey modKey, string ghostSuffix)
    {
        return new LoadOrderListing(modKey, enabled: false, ghostSuffix: ghostSuffix);
    }

    public override string ToString()
    {
        return ILoadOrderListingGetter.ToString(this);
    }

    public static implicit operator LoadOrderListing(ModKey modKey)
    {
        return new LoadOrderListing(modKey, enabled: true);
    }

    /// <inheritdoc cref="ILoadOrderListingParser" />
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
        if (periodIndex == -1)
        {
            listing = default;
            return false;
        }
        var ghostTerm = str.Slice(periodIndex + 1);
        str = str.Slice(0, periodIndex);

        if (ModKey.TryFromNameAndExtension(str, out key))
        {
            listing = LoadOrderListing.CreateGhosted(key, ghostTerm.ToString());
            return true;
        }

        listing = default;
        return false;
    }

    /// <inheritdoc cref="ILoadOrderListingParser" />
    public static bool TryFromFileName(FileName fileName, bool enabledMarkerProcessing, [MaybeNullWhen(false)] out LoadOrderListing listing)
    {
        return TryFromString(fileName.String, enabledMarkerProcessing, out listing);
    }

    /// <inheritdoc cref="ILoadOrderListingParser" />
    public static LoadOrderListing FromString(ReadOnlySpan<char> str, bool enabledMarkerProcessing)
    {
        if (!TryFromString(str, enabledMarkerProcessing, out var listing))
        {
            throw new InvalidDataException($"Load order file had malformed line: {str.ToString()}");
        }
        return listing;
    }

    /// <inheritdoc cref="ILoadOrderListingParser" />
    public static LoadOrderListing FromFileName(FileName name, bool enabledMarkerProcessing)
    {
        if (!TryFromFileName(name, enabledMarkerProcessing, out var listing))
        {
            throw new InvalidDataException($"Load order file had malformed line: {name}");
        }
        return listing;
    }

    public static Comparer<LoadOrderListing> GetComparer(Comparer<ModKey> comparer) =>
        Comparer<LoadOrderListing>.Create((x, y) => comparer.Compare(x.ModKey, y.ModKey));

    public static Comparer<TListing> GetComparer<TListing>(Comparer<ModKey> comparer)
        where TListing : ILoadOrderListingGetter
    {
        return Comparer<TListing>.Create((x, y) => comparer.Compare(x.ModKey, y.ModKey));
    }
}
