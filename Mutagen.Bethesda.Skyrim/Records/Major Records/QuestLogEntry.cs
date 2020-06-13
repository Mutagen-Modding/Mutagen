using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class QuestLogEntry
    {
        [Flags]
        public enum Flag
        {
            CompleteQuest = 0x01,
            FailQuest = 0x02,
        }
    }

    namespace Internals
    {
        public partial class QuestLogEntryBinaryCreateTranslation
        {
            static partial void FillBinaryConditionsCustom(MutagenFrame frame, IQuestLogEntry item)
            {
                ConditionBinaryCreateTranslation.FillConditionsList(item.Conditions, frame);
            }
        }

        public partial class QuestLogEntryBinaryWriteTranslation
        {
            static partial void WriteBinaryConditionsCustom(MutagenWriter writer, IQuestLogEntryGetter item)
            {
                ConditionBinaryWriteTranslation.WriteConditionsList(item.Conditions, writer);
            }
        }

        public partial class QuestLogEntryBinaryOverlay
        {
            public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

            partial void ConditionsCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
            }
        }
    }
}
