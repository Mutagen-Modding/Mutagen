using Mutagen.Bethesda.Plugins.Cache;

namespace Mutagen.Bethesda.Plugins.Assets;

public class AssetLinkCache : IAssetLinkCache
{
    private readonly Dictionary<Type, object> _componentCache = new();

    public ILinkCache FormLinkCache { get; }
    
    public AssetLinkCache(ILinkCache linkCache)
    {
        FormLinkCache = linkCache;
    }
    
    public TComponent GetComponent<TComponent>() 
        where TComponent : class, IAssetCacheComponent, new()
    {
        lock (_componentCache)
        {
            if (_componentCache.TryGetValue(typeof(TComponent), out var component)) return (TComponent)component;
            var newComponent = new TComponent();
            newComponent.Prep(this);
            _componentCache[typeof(TComponent)] = newComponent;
            return newComponent;
        }
    }

    public void Dispose()
    {
    }
}