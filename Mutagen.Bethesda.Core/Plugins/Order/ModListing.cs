using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order;

/// <inheritdoc cref="IModListingGetter" />
[DebuggerDisplay("{ToString()}")]
public record ModListing : IModListingGetter
{
    /// <inheritdoc />
    public ModKey ModKey { get; init; }

    /// <inheritdoc />
    public bool Enabled { get; init; }

    /// <inheritdoc />
    public bool ExistsOnDisk { get; init; } = true;

    /// <inheritdoc />
    public bool Ghosted => !string.IsNullOrWhiteSpace(GhostSuffix);

    /// <inheritdoc />
    public string GhostSuffix { get; init; } = string.Empty;

    public ModListing()
    {
    }

    public ModListing(ModKey modKey, bool enabled, bool existsOnDisk, string ghostSuffix = "")
    {
        ModKey = modKey;
        ExistsOnDisk = existsOnDisk;
        Enabled = enabled;
        GhostSuffix = ghostSuffix;
    }

    public override string ToString()
    {
        return IModListingGetter.ToString(this);
    }

    public static Comparer<ModListing> GetComparer(Comparer<ModKey> comparer) =>
        Comparer<ModListing>.Create((x, y) => comparer.Compare(x.ModKey, y.ModKey));

    public static Comparer<TListing> GetComparer<TListing>(Comparer<ModKey> comparer)
        where TListing : ILoadOrderListingGetter
    {
        return Comparer<TListing>.Create((x, y) => comparer.Compare(x.ModKey, y.ModKey));
    }
}

/// <inheritdoc cref="IModListingGetter{TMod}" />
[DebuggerDisplay("{ToString()}")]
public record ModListing<TMod> : ModListing, IModListing<TMod>
    where TMod : class, IModGetter
{
    /// <inheritdoc cref="IModListing{TMod}.Mod" />
    public TMod? Mod { get; set; }

    public bool Exists => Mod != null;

    private ModListing(ModKey key, TMod? mod, bool enabled, string ghostSuffix = "")
    {
        ModKey = key;
        Mod = mod;
        Enabled = enabled;
        GhostSuffix = ghostSuffix;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public ModListing(TMod mod, bool enabled = true, string ghostSuffix = "")
    {
        ModKey = mod.ModKey;
        Mod = mod;
        Enabled = enabled;
        GhostSuffix = ghostSuffix;
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
    public static ModListing<TMod> CreateUnloaded(ModKey key, bool enabled, string ghostSuffix = "")
    {
        return new ModListing<TMod>(key, default, enabled: enabled, ghostSuffix: ghostSuffix);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return IModListingGetter<TMod>.ToString(this);
    }

    public void Dispose()
    {
        if (Mod is IDisposable disp)
        {
            disp.Dispose();
        }
    }
}

/// <summary>
/// A Mod Listing on a load order.  Can be enabled or disabled.  Can also be "ghosted" which means
/// the listing does not end with a typical ModKey suffix.<br/>
/// <br/>
/// The generic variant also includes an optional Mod object that may or may not exist.
/// </summary>
public interface IModListingGetter<out TMod> : IModListingGetter, IDisposable
    where TMod : class, IModGetter
{
    /// <summary>
    /// Mod object
    /// </summary>
    TMod? Mod { get; }
}

/// <inheritdoc />
public interface IModListing<TMod> : IModListingGetter<TMod>
    where TMod : class, IModGetter
{
    /// <summary>
    /// Mod object
    /// </summary>
    new TMod? Mod { get; set; }
}

/// <summary>
/// A Mod Listing on a load order with a mod object attached.  Can be enabled or disabled.  Can also be "ghosted" which means
/// the listing does not end with a typical ModKey suffix
/// </summary>
public interface IModListingGetter : ILoadOrderListingGetter
{
    public bool ExistsOnDisk { get; }
        
    public static string ToString(IModListingGetter getter)
    {
        return $"[{(getter.Enabled ? "X" : "_")}] {getter.ModKey}{(getter.ExistsOnDisk ? null : " (missing)")}{(getter.Ghosted ? " (ghosted)" : null)}";
    }
}

/// <inheritdoc cref="IModListingGetter" />
public interface IModListing : IModListingGetter
{
    /// <summary>
    /// Whether the listing is enabled in the load order
    /// </summary>
    new bool Enabled { get; set; }

    /// <summary>
    /// What file suffix is used if ghosted.  This is done by modifying the file type to be anything abnormal.<br/>
    /// This is the same as disabling a mod as far as the game is concerned, but also is a hint to modmanagers to treat 
    /// the mods differently depending on the context
    /// </summary>
    new string GhostSuffix { get; set; }
}