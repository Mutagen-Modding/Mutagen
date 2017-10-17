using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    public static class Constants
    {
        public const byte HEADER_LENGTH = 4;
        public const byte RECORD_LENGTHLENGTH = 4;
        public const byte RECORD_LENGTH = HEADER_LENGTH + RECORD_LENGTHLENGTH;
        public const byte SUBRECORD_LENGTHLENGTH = 2;
        public const byte SUBRECORD_LENGTH = HEADER_LENGTH + SUBRECORD_LENGTHLENGTH;
        public const byte SUBRECORD_HEADER_OFFSET = 0;
        public const byte RECORD_HEADER_LENGTH = 16;
        public const byte RECORD_HEADER_SKIP = RECORD_HEADER_LENGTH - RECORD_LENGTHLENGTH;
        public const byte RECORD_HEADER_OFFSET = 12;
        public const byte GRUP_LENGTHLENGTH = RECORD_LENGTHLENGTH;
        public const byte GRUP_LENGTH = HEADER_LENGTH + GRUP_LENGTHLENGTH + HEADER_LENGTH;
        public const sbyte GRUP_HEADER_OFFSET = -8;
        public const string TRIGGERING_RECORDTYPE_MEMBER = "TRIGGERING_RECORD_TYPE";

        public const string OBLIVION_ESM = "Oblivion.esm";
        public const string KNIGHTS_ESP = "Knights.esp";
    }
}
