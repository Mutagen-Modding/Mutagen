using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda.Plugins.Assets;

public interface IAssetLinkCache : IDisposable
{
    ILinkCache FormLinkCache { get; }
    TComponent GetComponent<TComponent>()
        where TComponent : class, IAssetCacheComponent, new();
}