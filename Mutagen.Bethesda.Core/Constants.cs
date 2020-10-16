using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Internals
{
    public class Constants
    {
        public static readonly sbyte HeaderLength = 4;
        public const string TriggeringRecordTypeMember = "TriggeringRecordType";
        public const string GrupRecordTypeMember = "GrupRecordType";
        public const string EdidLinked = "edidLinked";
        public static readonly RecordType EditorID = new RecordType("EDID");
        public static readonly RecordType Group = new RecordType("GRUP");
        public const int LightMasterLimit = 2048;
        public const int CompressedFlag = 0x0004_0000;
        public const int DeletedFlag = 0x0000_0020;
    }
}
