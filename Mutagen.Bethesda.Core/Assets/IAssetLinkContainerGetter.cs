using System.Collections.Generic;

namespace Mutagen.Bethesda.Assets; 

/// <summary>
/// An interface for classes that contain assets and can enumerate them.
/// </summary>
public interface IAssetLinkContainer : IAssetLinkContainerGetter {
    /// <summary>
    /// Swaps out all links to point to new assets
    /// </summary>
    void RemapLinks(IReadOnlyDictionary<string, string> mapping);
}

/// <summary>
/// An interface for classes that contain assets and can enumerate them.
/// </summary>
public interface IAssetLinkContainerGetter {
    /// <summary>
    /// Enumerable of all contained assets
    /// </summary>
    IEnumerable<IAssetLinkGetter> ContainedAssetLinks { get; }
}