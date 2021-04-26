using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PatrolBinaryCreateTranslation
        {
            static partial void FillBinaryPatrolScriptMarkerCustom(MutagenFrame frame, IPatrol item)
            {
                if (frame.ReadSubrecordFrame().Content.Length != 0)
                {
                    throw new ArgumentException($"Marker had unexpected length.");
                }
            }

            static partial void FillBinaryTopicsCustom(MutagenFrame frame, IPatrol item)
            {
                item.Topics.SetTo(ATopicReferenceBinaryCreateTranslation.Factory(frame));
            }
        }

        public partial class PatrolBinaryWriteTranslation
        {
            static partial void WriteBinaryPatrolScriptMarkerCustom(MutagenWriter writer, IPatrolGetter item)
            {
                using (HeaderExport.Subrecord(writer, RecordTypes.XPPA)) { }
            }

            static partial void WriteBinaryTopicsCustom(MutagenWriter writer, IPatrolGetter item)
            {
                ATopicReferenceBinaryWriteTranslation.Write(writer, item.Topics);
            }
        }

        public partial class PatrolBinaryOverlay
        {
            public IReadOnlyList<IATopicReferenceGetter> Topics { get; private set; } = ListExt.Empty<IATopicReferenceGetter>();

            partial void PatrolScriptMarkerCustomParse(OverlayStream stream, int offset)
            {
                if (stream.ReadSubrecordFrame().Content.Length != 0)
                {
                    throw new ArgumentException($"Marker had unexpected length.");
                }
            }

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
