using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Skyrim.Assets;

namespace Mutagen.Bethesda.Skyrim;

internal partial class SkyrimGameModule
{
    public IReadOnlyList<IAssetType> AssetTypes { get; } =
    [
        SkyrimBehaviorAssetType.Instance,
        SkyrimBodyTextureAssetType.Instance,
        SkyrimDeformedModelAssetType.Instance,
        SkyrimInterfaceAssetType.Instance,
        SkyrimModelAssetType.Instance,
        SkyrimMusicAssetType.Instance,
        SkyrimScriptCompiledAssetType.Instance,
        SkyrimScriptSourceAssetType.Instance,
        SkyrimSeqAssetType.Instance,
        SkyrimSoundAssetType.Instance,
        SkyrimTextureAssetType.Instance,
        SkyrimTranslationAssetType.Instance,
    ];
}
