using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class Constants
    {
        public static readonly int HEADER_LENGTH = 4;
        public static readonly int RECORD_LENGTHLENGTH = 4;
        public static readonly int RECORD_LENGTH = HEADER_LENGTH + RECORD_LENGTHLENGTH;
        public static readonly int SUBRECORD_LENGTHLENGTH = 2;
        public static readonly int SUBRECORD_LENGTH = HEADER_LENGTH + SUBRECORD_LENGTHLENGTH;
        public static readonly int SUBRECORD_HEADER_OFFSET = 0;
        public static readonly int RECORD_META_LENGTH = 16;
        public static readonly int RECORD_META_SKIP = RECORD_META_LENGTH - RECORD_LENGTHLENGTH;
        public static readonly int RECORD_HEADER_LENGTH = RECORD_META_LENGTH + HEADER_LENGTH;
        public static readonly int RECORD_META_OFFSET = 12;
        public static readonly int GRUP_LENGTHLENGTH = RECORD_LENGTHLENGTH;
        public static readonly int GRUP_LENGTH = HEADER_LENGTH + RECORD_META_LENGTH;
        public static readonly int GRUP_HEADER_OFFSET = -8;
        public const string TRIGGERING_RECORDTYPE_MEMBER = "TRIGGERING_RECORD_TYPE";
        public const string GRUP_RECORDTYPE_MEMBER = "GRUP_RECORD_TYPE";
    }
}
