using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Fallout3.Assets;

namespace Mutagen.Bethesda.Fallout3;

internal partial class Fallout3GameModule
{
    public IReadOnlyList<IAssetType> AssetTypes { get; } =
    [
        Fallout3TextureAssetType.Instance,
    ];
}
