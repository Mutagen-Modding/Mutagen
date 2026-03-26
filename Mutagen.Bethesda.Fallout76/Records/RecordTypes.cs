using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout76.Internals;

public partial class RecordTypes
{
    public static readonly RecordType CS2K = new(0x4B325343);
    public static readonly RecordType DEMO = new(0x4F4D4544);
    public static readonly RecordType DEVA = new(0x41564544);
    public static readonly RecordType DTID = new(0x44495444);
    public static readonly RecordType IDLB = new(0x424C4449);

    public static readonly RecordType RDGS = new(0x53474452);
    public static readonly RecordType SCDA = new(0x41444353);
    public static readonly RecordType SCRO = new(0x4F524353);
    public static readonly RecordType SCTX = new(0x58544353);

    public static readonly RecordType XLOD = new(0x444F4C58);
}
