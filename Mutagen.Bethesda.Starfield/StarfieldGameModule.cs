using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Starfield.Assets;

namespace Mutagen.Bethesda.Starfield;

internal partial class StarfieldGameModule
{
    public IReadOnlyList<IAssetType> AssetTypes { get; } =
    [
        StarfieldAnimationTextAssetType.Instance,
        StarfieldMaterialAssetType.Instance,
        StarfieldModelAssetType.Instance,
        StarfieldRigAssetType.Instance,
        StarfieldTextureAssetType.Instance,
    ];
}
