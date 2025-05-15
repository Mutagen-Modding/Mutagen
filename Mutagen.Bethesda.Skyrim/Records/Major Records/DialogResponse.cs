using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Skyrim.Records.Assets.VoiceType;
namespace Mutagen.Bethesda.Skyrim;

public partial class DialogResponse
{
    [Flags]
    public enum Flag
    {
        UseEmotionAnimation = 0x01
    }
}

partial class DialogResponsesSetterCommon
{
    private static partial void RemapResolvedAssetLinks(
        IDialogResponses obj,
        IReadOnlyDictionary<IAssetLinkGetter, string> mapping,
        IAssetLinkCache? linkCache,
        AssetLinkQuery queryCategories)
    {
        // Nothing to do here, we can't change the form key of the dialogue or any other parameters, like quest editor id
    }
}

partial class DialogResponsesCommon
{
    public static partial IEnumerable<IAssetLinkGetter> GetResolvedAssetLinks(IDialogResponsesGetter obj, IAssetLinkCache linkCache, Type? assetType)
    {
        if (assetType != null && assetType != typeof(SkyrimSoundAssetType)) yield break;

        var voiceTypeLookup = linkCache.GetComponent<VoiceTypeAssetLookup>();
        foreach (var voiceTypePath in voiceTypeLookup.GetVoiceTypePaths(obj))
        {
            yield return new AssetLink<SkyrimSoundAssetType>(voiceTypePath);
        }
    }
}