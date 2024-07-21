using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

internal struct BinaryOverlayFactoryPackage
{
    public ParsingMeta MetaData;
    public IFormVersionGetter? FormVersion;

    public BinaryOverlayFactoryPackage(ParsingMeta metaData)
    {
        MetaData = metaData;
        FormVersion = null;
    }

    public static implicit operator ParsingMeta(BinaryOverlayFactoryPackage package)
    {
        return package.MetaData;
    }

    public static implicit operator GameConstants(BinaryOverlayFactoryPackage package)
    {
        return package.MetaData.Constants;
    }
}