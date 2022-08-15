using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda.Assets;

public class AssetLinkCache : IAssetLinkCache
{
    private readonly Dictionary<Type, object> _componentCache = new();

    public ILinkCache LinkCache { get; }
    
    public AssetLinkCache(ILinkCache linkCache)
    {
        LinkCache = linkCache;
    }
    
    public TComponent GetComponent<TComponent>() where TComponent : IAssetCacheComponent
    {
        lock (_componentCache)
        {
            if (_componentCache.TryGetValue(typeof(TComponent), out var component)) return (TComponent)component;
            
            // ToDo
            // Use reflection to instantiate component
            throw new NotImplementedException();
        }
    }
}