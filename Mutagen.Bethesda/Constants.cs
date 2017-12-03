using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class Constants
    {
        public static readonly ContentLength HEADER_LENGTH = new ContentLength(4);
        public static readonly ContentLength RECORD_LENGTHLENGTH = new ContentLength(4);
        public static readonly ContentLength RECORD_LENGTH = HEADER_LENGTH + RECORD_LENGTHLENGTH;
        public static readonly ContentLength SUBRECORD_LENGTHLENGTH = new ContentLength(2);
        public static readonly ContentLength SUBRECORD_LENGTH = HEADER_LENGTH + SUBRECORD_LENGTHLENGTH;
        public static readonly ContentLength SUBRECORD_HEADER_OFFSET = new ContentLength(0);
        public static readonly ContentLength RECORD_HEADER_LENGTH = new ContentLength(16);
        public static readonly ContentLength RECORD_HEADER_SKIP = RECORD_HEADER_LENGTH - RECORD_LENGTHLENGTH;
        public static readonly ContentLength RECORD_HEADER_OFFSET = new ContentLength(12);
        public static readonly ContentLength GRUP_LENGTHLENGTH = RECORD_LENGTHLENGTH;
        public static readonly ContentLength GRUP_LENGTH = HEADER_LENGTH + GRUP_LENGTHLENGTH + HEADER_LENGTH;
        public static readonly ContentLength GRUP_HEADER_OFFSET = new ContentLength(-8);
        public const string TRIGGERING_RECORDTYPE_MEMBER = "TRIGGERING_RECORD_TYPE";

        public const string OBLIVION_ESM = "Oblivion.esm";
        public const string KNIGHTS_ESP = "Knights.esp";
    }
}
