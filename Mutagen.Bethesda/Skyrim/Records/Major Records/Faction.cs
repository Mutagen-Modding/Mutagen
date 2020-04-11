using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Faction
    {
        [Flags]
        public enum FactionFlag
        {
            HiddenFromPC = 0x00001,
            SpecialCombat = 0x00002,
            TrackCrime = 0x00040,
            IgnoreMurder = 0x00080,
            IgnoreAssault = 0x00100,
            IgnoreStealing = 0x00200,
            IgnoreTrespass = 0x00400,
            DoNotReportCrimesAgainstMembers = 0x00800,
            CrimeGoldUseDefaults = 0x01000,
            IgnorePickpocket = 0x02000,
            Vendor = 0x04000,
            CanBeOwner = 0x08000,
            IgnoreWerewolf = 0x10000,
        }
    }

    namespace Internals
    {
        public partial class FactionBinaryCreateTranslation
        {
            static partial void FillBinaryConditionsCustom(MutagenFrame frame, IFactionInternal item)
            {
                if (!frame.TryReadSubrecordFrame(Faction_Registration.CITC_HEADER, out var countMeta)
                    || countMeta.Content.Length != 4)
                {
                    throw new ArgumentException();
                }
                var count = BinaryPrimitives.ReadInt32LittleEndian(countMeta.Content);
                item.Conditions = new ExtendedList<Condition>();
                ConditionBinaryCreateTranslation.FillConditionsList(item.Conditions, frame, count);
            }
        }

        public partial class FactionBinaryWriteTranslation
        {
            static partial void WriteBinaryConditionsCustom(MutagenWriter writer, IFactionGetter item)
            {
                var conditions = item.Conditions;
                if (conditions == null) return;
                using (HeaderExport.ExportSubrecordHeader(writer, Faction_Registration.CITC_HEADER))
                {
                    writer.Write(conditions.Count);
                }
                ConditionBinaryWriteTranslation.WriteConditionsList(conditions, writer);
            }
        }

        public partial class FactionBinaryOverlay
        {
            public IReadOnlyList<IConditionGetter>? Conditions { get; private set; }

            partial void ConditionsCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                Conditions = ConditionBinaryOverlay.ConstructBinayOverlayCountedList(stream, _package);
            }
        }
    }
}
