using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda.Assets;

public interface IAssetLinkCache
{
    ILinkCache LinkCache { get; }
    TComponent GetComponent<TComponent>()
        where TComponent : IAssetCacheComponent;
}