using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim.Internals
{
    public partial class EffectBinaryCreateTranslation
    {
        static partial void FillBinaryConditionsCustom(MutagenFrame frame, IEffect item)
        {
            ConditionBinaryCreateTranslation.FillConditionsList(item.Conditions, frame);
        }
    }

    public partial class EffectBinaryWriteTranslation
    {
        static partial void WriteBinaryConditionsCustom(MutagenWriter writer, IEffectGetter item)
        {
            ConditionBinaryWriteTranslation.WriteConditionsList(item.Conditions, writer);
        }
    }

    public partial class EffectBinaryOverlay
    {
        public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();

        partial void ConditionsCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
        {
            Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
        }
    }
}
