using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.UnitTests.Plugins;

public static class TestRecordTypes
{
    public static readonly RecordType EDID = new("EDID");
    public static readonly RecordType DATA = new("DATA");
    public static readonly RecordType FNAM = new("FNAM");
    public static readonly RecordType MAST = new("MAST");
    public static readonly RecordType TX00 = new("TX00");
    public static readonly RecordType TX01 = new("TX01");
    public static readonly RecordType TX02 = new("TX02");
    public static readonly RecordType TX03 = new("TX03");
    public static readonly RecordType XNAM = new("XNAM");
}