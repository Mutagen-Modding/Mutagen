using Mutagen.Bethesda.Assets;

namespace Mutagen.Bethesda.Skyrim.Assets;

public class VoiceTypeAssetLookup : IAssetCacheComponent
{
    private readonly IAssetLinkCache _assetLinkCache;

    public VoiceTypeAssetLookup(IAssetLinkCache assetLinkCache)
    {
        _assetLinkCache = assetLinkCache;
        // Do setup
    }
    
    // Expose whatever
}