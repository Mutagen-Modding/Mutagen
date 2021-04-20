using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Records.Binary.Overlay;
using Mutagen.Bethesda.Records.Binary.Streams;
using Noggog;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PackageEventBinaryCreateTranslation
        {
            static partial void FillBinaryTopicsCustom(MutagenFrame frame, IPackageEvent item)
            {
                item.Topics.SetTo(ATopicReferenceBinaryCreateTranslation.Factory(frame));
            }
        }

        public partial class PackageEventBinaryWriteTranslation
        {
            static partial void WriteBinaryTopicsCustom(MutagenWriter writer, IPackageEventGetter item)
            {
                ATopicReferenceBinaryWriteTranslation.Write(writer, item.Topics);
            }
        }

        public partial class PackageEventBinaryOverlay
        {
            public IReadOnlyList<IATopicReferenceGetter> Topics { get; private set; } = ListExt.Empty<IATopicReferenceGetter>();

            partial void TopicsCustomParse(
                OverlayStream stream,
                long finalPos,
                int offset,
                RecordType type,
                int? lastParsed)
            {
                Topics = new List<IATopicReferenceGetter>(
                    ATopicReferenceBinaryCreateTranslation.Factory(
                        new MutagenFrame(stream)));
            }
        }
    }
}
