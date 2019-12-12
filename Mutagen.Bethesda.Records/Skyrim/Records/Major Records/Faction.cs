using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
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
            static partial void FillBinaryConditionsCustom(MutagenFrame frame, IFactionInternal item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                var countMeta = frame.MetaData.ReadSubRecordFrame(frame);
                if (countMeta.Header.RecordType != Faction_Registration.CITC_HEADER
                    || countMeta.ContentSpan.Length != 4)
                {
                    throw new ArgumentException();
                }
                var count = BinaryPrimitives.ReadInt32LittleEndian(countMeta.ContentSpan);
                List<Condition> conds = new List<Condition>(count);
                for (int i = 0; i < count; i++)
                {
                    conds.Add(Condition.CreateFromBinary(frame, masterReferences, default(RecordTypeConverter), errorMask));
                }
                item.Conditions.SetTo(conds);
            }
        }

        public partial class FactionBinaryWriteTranslation
        {
            static partial void WriteBinaryConditionsCustom(MutagenWriter writer, IFactionGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (!item.Conditions.HasBeenSet) return;
                using (HeaderExport.ExportSubRecordHeader(writer, Faction_Registration.CITC_HEADER))
                {
                    writer.Write(item.Conditions.Count);
                }
                foreach (var cond in item.Conditions)
                {
                    cond.WriteToBinary(writer, masterReferences);
                }
            }
        }

        public partial class FactionBinaryWrapper
        {
            public IReadOnlySetList<IConditionGetter> Conditions => throw new NotImplementedException();

            partial void ConditionsCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
            }
        }
    }
}
