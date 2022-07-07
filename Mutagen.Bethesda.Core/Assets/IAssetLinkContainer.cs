using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda.Assets;

/// <summary>
/// An interface for classes that contain AssetLinks and can enumerate them.
/// </summary>
public interface IAssetLinkContainer : IAssetLinkContainerGetter
{
    /// <summary>
    /// Swaps out all links to point to new assets
    /// </summary>
    void RemapListedAssetLinks(IReadOnlyDictionary<IAssetLinkGetter, string> mapping);

    /// <summary>
    /// Enumerates only AssetLinks that are explicitly listed in the record and can be modified directly.
    /// </summary>
    IEnumerable<IAssetLink> EnumerateListedAssetLinks();
}

/// <summary>
/// An interface for classes that contain AssetLinks and can enumerate them.
/// </summary>
public interface IAssetLinkContainerGetter
{
    /// <summary>
    /// Enumerates AssetLinks that are explicitly or implicitly defined
    /// </summary>
    /// <param name="linkCache">Link cache to provide meta assets related to the interaction of multiple major records</param>
    /// <param name="includeImplicit">Whether to include assets with paths that are derivative from other fields on a record</param>
    IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(ILinkCache? linkCache = null, bool includeImplicit = true);
}