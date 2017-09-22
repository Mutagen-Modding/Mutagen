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
        public const byte RECORD_HEADER_LENGTH = 16;
        public const byte RECORD_HEADER_SKIP = RECORD_HEADER_LENGTH - RECORD_LENGTHLENGTH;
        public const string TRIGGERING_RECORDTYPE_MEMBER = "TRIGGERING_RECORD_TYPE";
    }
}
