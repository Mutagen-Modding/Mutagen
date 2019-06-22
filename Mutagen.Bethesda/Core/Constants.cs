using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public class Constants
    {
        public static readonly sbyte HEADER_LENGTH = 4;
        public static readonly sbyte RECORD_LENGTHLENGTH = 4;
        public static readonly sbyte RECORD_LENGTH = (sbyte)(HEADER_LENGTH + RECORD_LENGTHLENGTH);
        public static readonly sbyte SUBRECORD_LENGTHLENGTH = 2;
        public static readonly sbyte SUBRECORD_LENGTH = (sbyte)(HEADER_LENGTH + SUBRECORD_LENGTHLENGTH);
        public static readonly sbyte SUBRECORD_HEADER_OFFSET = 0;
        public static readonly sbyte RECORD_META_LENGTH = 16;
        public static readonly sbyte RECORD_META_SKIP = (sbyte)(RECORD_META_LENGTH - RECORD_LENGTHLENGTH);
        public static readonly sbyte RECORD_HEADER_LENGTH = (sbyte)(RECORD_META_LENGTH + HEADER_LENGTH);
        public static readonly sbyte RECORD_META_OFFSET = 12;
        public static readonly sbyte GRUP_LENGTHLENGTH = RECORD_LENGTHLENGTH;
        public static readonly sbyte GRUP_LENGTH = (sbyte)(HEADER_LENGTH + RECORD_META_LENGTH);
        public static readonly sbyte GRUP_HEADER_OFFSET = -8;
        public const string TRIGGERING_RECORDTYPE_MEMBER = "TRIGGERING_RECORD_TYPE";
        public const string GRUP_RECORDTYPE_MEMBER = "GRUP_RECORD_TYPE";
        public static readonly RecordType EditorID = new RecordType("EDID");
        public static readonly RecordType GRUP = new RecordType("GRUP");

        public sbyte ModHeaderFluffLength { get; private set; }
        public sbyte GrupMetaLengthAfterType { get; private set; }
        public sbyte RecordMetaLengthAfterRecordLength { get; private set; }

        public static readonly Constants Oblivion = new Constants()
        {
            ModHeaderFluffLength = 12,
            GrupMetaLengthAfterType = 4,
            RecordMetaLengthAfterRecordLength = 12,
        };

        public static readonly Constants Skyrim = new Constants()
        {
            ModHeaderFluffLength = 16,
            GrupMetaLengthAfterType = 8,
            RecordMetaLengthAfterRecordLength = 16,
        };

        public static Constants Get(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Oblivion:
                    return Oblivion;
                case GameMode.Skyrim:
                    return Skyrim;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
