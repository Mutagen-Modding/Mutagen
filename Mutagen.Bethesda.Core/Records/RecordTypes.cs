using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Records.Internals
{
    public static class RecordTypes
    {
        public static readonly RecordType MAST = new RecordType("MAST");
        public static readonly RecordType DATA = new RecordType("DATA");
        public static readonly RecordType EDID = new RecordType("EDID");
        public static readonly RecordType XXXX = new RecordType("XXXX");
    }
}
