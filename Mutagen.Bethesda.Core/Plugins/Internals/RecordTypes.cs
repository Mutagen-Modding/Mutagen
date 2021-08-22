using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Plugins.Internals
{
    public static class RecordTypes
    {
        public static readonly RecordType MAST = new("MAST");
        public static readonly RecordType DATA = new("DATA");
        public static readonly RecordType EDID = new("EDID");
        public static readonly RecordType XXXX = new("XXXX");
    }
}
