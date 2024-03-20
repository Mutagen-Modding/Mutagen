using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public class AssetLinkBinaryTranslation
{
    public static readonly AssetLinkBinaryTranslation Instance = new();

    public AssetLink<TAssetType> Parse<TAssetType>(
        MutagenFrame reader)
        where TAssetType : class, IAssetType
    {
        return new AssetLink<TAssetType>(
            StringBinaryTranslation.Instance.Parse(reader, StringBinaryType.NullTerminate, parseWhole: true));
    }

    public bool Parse<TAssetType>(
        MutagenFrame reader,
        [MaybeNullWhen(false)] out IAssetLink<TAssetType> item)
        where TAssetType : class, IAssetType
    {
        if (StringBinaryTranslation.Instance.Parse(reader, parseWhole: true, out var str, StringBinaryType.NullTerminate))
        {
            item = new AssetLink<TAssetType>(str);
            return true;
        }

        item = default;
        return false;
    }
}