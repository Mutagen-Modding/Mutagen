using System;

namespace Mutagen.Bethesda.Records.Constants
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
        public const int MasterFlag = 0x0000_0001;
        public const int CompressedFlag = 0x0004_0000;
        public const int DeletedFlag = 0x0000_0020;
        public const int InitiallyDisabled = 0x0000_0800;
        public const int Ignored = 0x0000_1000;
    }
}
