using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MusicTrack
    {
        public enum TypeEnum : uint
        {
            Palette = 0x23F678C3,
            SingleTrack = 0x6ED7E048,
            SilentTrack = 0xA1A9C4D5,
        }
    }

    namespace Internals
    {
        public partial class MusicTrackBinaryCreateTranslation
        {
            static partial void FillBinaryConditionsCustom(MutagenFrame frame, IMusicTrackInternal item)
            {
                if (!frame.TryReadSubrecordFrame(RecordTypes.CITC, out var countMeta)
                    || countMeta.Content.Length != 4)
                {
                    throw new ArgumentException();
                }
                var count = BinaryPrimitives.ReadInt32LittleEndian(countMeta.Content);
                item.Conditions = new ExtendedList<Condition>();
                ConditionBinaryCreateTranslation.FillConditionsList(item.Conditions, frame, count);
            }
        }

        public partial class MusicTrackBinaryWriteTranslation
        {
            static partial void WriteBinaryConditionsCustom(MutagenWriter writer, IMusicTrackGetter item)
            {
                var conditions = item.Conditions;
                if (conditions == null) return;
                using (HeaderExport.Subrecord(writer, RecordTypes.CITC))
                {
                    writer.Write(conditions.Count);
                }
                ConditionBinaryWriteTranslation.WriteConditionsList(conditions, writer);
            }
        }

        public partial class MusicTrackBinaryOverlay
        {
            public IReadOnlyList<IConditionGetter>? Conditions { get; private set; }

            partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                Conditions = ConditionBinaryOverlay.ConstructBinayOverlayCountedList(stream, _package);
            }
        }
    }
}
