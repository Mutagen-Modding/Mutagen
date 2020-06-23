using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class QuestAlias
    {
        public enum TypeEnum
        {
            Reference,
            Location,
        }

        [Flags]
        public enum Flag
        {
            ReservesLocationOrReference = 0x0000_0001,
            Optional = 0x0000_0002,
            QuestObject = 0x0000_0004,
            AllowReuseInQuest = 0x0000_0008,
            AllowDead = 0x0000_0010,
            MatchingRefInLoadedArea = 0x0000_0020,
            Essential = 0x0000_0040,
            AllowDisabled = 0x0000_0080,
            StoresText = 0x0000_0100,
            AllowReserved = 0x0000_0200,
            Protected = 0x0000_0400,
            ForcedByAliases = 0x0000_0800,
            AllowDestroyed = 0x0000_1000,
            MatchingRefClosest = 0x0000_2000,
            UsesStoredText = 0x0000_4000,
            InitiallyDisabled = 0x0000_8000,
            AllowCleared = 0x0001_0000,
            ClearNamesWhenRemoved = 0x0002_0000,
        }
    }

    namespace Internals
    {
        public partial class QuestAliasBinaryCreateTranslation
        {
            static partial void FillBinaryIDParseCustom(MutagenFrame frame, IQuestAlias item)
            {
                var subMeta = frame.ReadSubrecord();
                item.Type = subMeta.RecordTypeInt switch
                {
                    // ALST
                    0x54534C41 => QuestAlias.TypeEnum.Reference,
                    // ALLS
                    0x534C4C41 => QuestAlias.TypeEnum.Location,
                    _ => throw new NotImplementedException(),
                };
                item.ID = frame.ReadUInt32();
            }

            static partial void FillBinaryConditionsCustom(MutagenFrame frame, IQuestAlias item)
            {
                ConditionBinaryCreateTranslation.FillConditionsList(item.Conditions, frame);
            }

            static partial void FillBinaryEndCustom(MutagenFrame frame, IQuestAlias item)
            {
                // Skip
                frame.ReadSubrecordFrame();
            }
        }

        public partial class QuestAliasBinaryWriteTranslation
        {
            static partial void WriteBinaryIDParseCustom(MutagenWriter writer, IQuestAliasGetter item)
            {
                RecordType type = item.Type switch
                {
                    QuestAlias.TypeEnum.Reference => RecordTypes.ALST,
                    QuestAlias.TypeEnum.Location => RecordTypes.ALLS,
                    _ => throw new NotImplementedException(),
                };
                using (HeaderExport.Subrecord(writer, type))
                {
                    writer.Write(item.ID);
                }
            }

            static partial void WriteBinaryConditionsCustom(MutagenWriter writer, IQuestAliasGetter item)
            {
                ConditionBinaryWriteTranslation.WriteConditionsList(item.Conditions, writer);
            }

            static partial void WriteBinaryEndCustom(MutagenWriter writer, IQuestAliasGetter item)
            {
                using (HeaderExport.Subrecord(writer, RecordTypes.ALED)) { }
            }
        }

        public partial class QuestAliasBinaryOverlay
        {
            public IReadOnlyList<IConditionGetter> Conditions { get; private set; } = ListExt.Empty<IConditionGetter>();
            public uint ID { get; private set; }
            public QuestAlias.TypeEnum Type { get; private set; }

            partial void ConditionsCustomParse(OverlayStream stream, long finalPos, int offset, RecordType type, int? lastParsed)
            {
                Conditions = ConditionBinaryOverlay.ConstructBinayOverlayList(stream, _package);
            }

            partial void IDParseCustomParse(OverlayStream stream, int offset)
            {
                var subMeta = stream.ReadSubrecord();
                this.Type = subMeta.RecordTypeInt switch
                {
                    // ALST
                    0x54534C41 => QuestAlias.TypeEnum.Reference,
                    // ALLS
                    0x534C4C41 => QuestAlias.TypeEnum.Location,
                    _ => throw new NotImplementedException(),
                };
                this.ID = stream.ReadUInt32();
            }

            partial void EndCustomParse(OverlayStream stream, int offset)
            {
                stream.ReadSubrecordFrame();
            }
        }
    }    
}
