using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Starfield;

public partial class StarfieldMod : AMod
{
    public const uint DefaultInitialNextFormID = 0x800;
    private uint GetDefaultInitialNextFormID() => DefaultInitialNextFormID;

    partial void CustomCtor()
    {
        this.ModHeader.FormVersion = GameConstants.Get(GameRelease).DefaultFormVersion!.Value;
    }
}

partial class StarfieldModSetterCommon
{
    private static partial void RemapInferredAssetLinks(
        IStarfieldMod obj,
        IReadOnlyDictionary<IAssetLinkGetter, string> mapping,
        AssetLinkQuery queryCategories)
    {
        // Nothing to do here, we can't change the name of the mod
    }
}

partial class StarfieldModCommon
{
    public static partial IEnumerable<IAssetLinkGetter> GetInferredAssetLinks(IStarfieldModGetter obj, Type? assetType)
    {
        yield break;
    }
}