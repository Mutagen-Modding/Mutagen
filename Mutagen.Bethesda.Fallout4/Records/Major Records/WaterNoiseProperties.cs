using Mutagen.Bethesda.Fallout4.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Fallout4;

partial class WaterNoisePropertiesBinaryOverlay
{
    private readonly SubrecordFrame? _textureRec;

    public float WindDirection => BinaryPrimitives.ReadSingleLittleEndian(_structData);

    public float WindSpeed => BinaryPrimitives.ReadSingleLittleEndian(_structData.Slice(12));

    public float AmplitudeScale => BinaryPrimitives.ReadSingleLittleEndian(_structData.Slice(24));

    public float UvScale => BinaryPrimitives.ReadSingleLittleEndian(_structData.Slice(36));

    public float NoiseFalloff => BinaryPrimitives.ReadSingleLittleEndian(_structData.Slice(48));

    public AssetLinkGetter<Fallout4TextureAssetType>? Texture =>
        _textureRec is { } rec ? new AssetLinkGetter<Fallout4TextureAssetType>(rec.AsString(_package.MetaData.Encodings.NonTranslated)) : null;

    public WaterNoisePropertiesBinaryOverlay(ReadOnlyMemorySlice<byte> data, SubrecordFrame? textureRec, BinaryOverlayFactoryPackage package)
        : base(MemoryPair.StructMemory(data), package)
    {
        _textureRec = textureRec;
    }
}
