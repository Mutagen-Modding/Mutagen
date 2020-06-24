using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class AStoryManagerNode
    {
        [Flags]
        public enum Flag
        {
            Random = 0x01,
            WarnIfNoChildQuestStarted = 0x02,
        }
    }

    namespace Internals
    {
        public partial class AStoryManagerNodeBinaryCreateTranslation
        {
            static partial void FillBinaryConditionsCustom(MutagenFrame frame, IAStoryManagerNodeInternal item)
            {
                if (!frame.TryReadSubrecordFrame(RecordTypes.CITC, out var countMeta)
                    || countMeta.Content.Length != 4)
                {
                    throw new ArgumentException();
                }
                var count = BinaryPrimitives.ReadInt32LittleEndian(countMeta.Content);
                ConditionBinaryCreateTranslation.FillConditionsList(item.Conditions, frame, count);
            }
        }

        public partial class AStoryManagerNodeBinaryWriteTranslation
        {
            static partial void WriteBinaryConditionsCustom(MutagenWriter writer, IAStoryManagerNodeGetter item)
            {
                var conditions = item.Conditions;
                using (HeaderExport.Subrecord(writer, RecordTypes.CITC))
                {
                    writer.Write(conditions.Count);
                }
                ConditionBinaryWriteTranslation.WriteConditionsList(conditions, writer);
            }
        }

        public partial class AStoryManagerNodeBinaryOverlay
        {
            public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

            partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                Conditions = ConditionBinaryOverlay.ConstructBinayOverlayCountedList(stream, _package);
            }
        }
    }
}
