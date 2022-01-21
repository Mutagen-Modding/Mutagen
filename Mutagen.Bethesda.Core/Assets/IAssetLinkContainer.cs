using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda.Assets;

/// <summary>
/// An interface for classes that contain assets and can enumerate them.
/// </summary>
public interface IAssetLinkContainer : IAssetLinkContainerGetter
{
    /// <summary>
    /// Swaps out all links to point to new assets
    /// </summary>
    void RemapLinks(IReadOnlyDictionary<string, string> mapping);
    
    /// <summary>
    /// Enumerable of all contained assets
    /// </summary>
    IEnumerable<IAssetLink> EnumerateAssetLinks();
}

/// <summary>
/// An interface for classes that contain assets and can enumerate them.
/// </summary>
public interface IAssetLinkContainerGetter
{
    /// <summary>
    /// Enumerable of all contained assets
    /// </summary>
    IEnumerable<IAssetLinkGetter> EnumerateAssetLinks(ILinkCache? linkCache = null, bool includeImplicit = true);
}