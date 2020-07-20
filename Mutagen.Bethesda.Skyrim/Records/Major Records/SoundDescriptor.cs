using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class SoundDescriptor
    {
        public enum LoopType
        {
            None = 0,
            Loop = 0x08,
            EnvelopeFast = 0x10,
            EnvelopeSlow = 0x20,
        }
    }

    namespace Internals
    {
        public partial class SoundDescriptorBinaryCreateTranslation
        {
            static partial void FillBinaryConditionsCustom(MutagenFrame frame, ISoundDescriptorInternal item)
            {
                ConditionBinaryCreateTranslation.FillConditionsList(item.Conditions, frame);
            }
        }

        public partial class SoundDescriptorBinaryWriteTranslation
        {
            static partial void WriteBinaryConditionsCustom(MutagenWriter writer, ISoundDescriptorGetter item)
            {
                ConditionBinaryWriteTranslation.WriteConditionsList(item.Conditions, writer);
            }
        }

        public partial class SoundDescriptorBinaryOverlay
        {
            public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

            partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
            }
        }
    }
}
