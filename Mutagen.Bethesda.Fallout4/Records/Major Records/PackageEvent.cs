using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Fallout4;

partial class PackageEventBinaryCreateTranslation
{
    public static partial void FillBinaryTopicsCustom(MutagenFrame frame, IPackageEvent item, PreviousParse lastParsed)
    {
        item.Topics.SetTo(ATopicReferenceBinaryCreateTranslation.Factory(frame));
    }
}

partial class PackageEventBinaryWriteTranslation
{
    public static partial void WriteBinaryTopicsCustom(MutagenWriter writer, IPackageEventGetter item)
    {
        ATopicReferenceBinaryWriteTranslation.Write(writer, item.Topics);
    }
}

partial class PackageEventBinaryOverlay
{
    public IReadOnlyList<IATopicReferenceGetter> Topics { get; private set; } = Array.Empty<IATopicReferenceGetter>();

    partial void TopicsCustomParse(
        OverlayStream stream,
        int finalPos,
        int offset,
        RecordType type,
        PreviousParse lastParsed)
    {
        Topics = new List<IATopicReferenceGetter>(
            ATopicReferenceBinaryCreateTranslation.Factory(
                new MutagenFrame(stream)));
    }
}