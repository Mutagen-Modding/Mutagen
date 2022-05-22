using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Fallout4;

partial class WaterNoisePropertiesBinaryOverlay
{
    private readonly SubrecordFrame? _textureRec;

    public float WindDirection => BinaryPrimitives.ReadSingleLittleEndian(_data);

    public float WindSpeed => BinaryPrimitives.ReadSingleLittleEndian(_data.Slice(12));

    public float AmplitudeScale => BinaryPrimitives.ReadSingleLittleEndian(_data.Slice(24));

    public float UvScale => BinaryPrimitives.ReadSingleLittleEndian(_data.Slice(36));

    public float NoiseFalloff => BinaryPrimitives.ReadSingleLittleEndian(_data.Slice(48));

    public string? Texture => _textureRec?.AsString(_package.MetaData.Encodings.NonTranslated);

    public WaterNoisePropertiesBinaryOverlay(ReadOnlyMemorySlice<byte> data, SubrecordFrame? textureRec, BinaryOverlayFactoryPackage package)
        : base(data, package)
    {
        _textureRec = textureRec;
    }
}
