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
        public const string TRIGGERING_RECORDTYPE_MEMBER = "TRIGGERING_RECORD_TYPE";
        public const string GRUP_RECORDTYPE_MEMBER = "GRUP_RECORD_TYPE";
        public const string EdidLinked = "edidLinked";
        public static readonly RecordType EditorID = new RecordType("EDID");
        public static readonly RecordType GRUP = new RecordType("GRUP");
    }
}
