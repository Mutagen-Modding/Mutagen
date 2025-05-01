using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Morrowind.Internals;

public partial class RecordTypes
{
    public static readonly RecordType CELL = new(0x4C4C4543);
    public static readonly RecordType CNAM = new(0x4D414E43);
    public static readonly RecordType DATA = new(0x41544144);
    public static readonly RecordType DELE = new(0x454C4544);
    public static readonly RecordType EDID = new(0x44494445);
    public static readonly RecordType GMST = new(0x54534D47);
    public static readonly RecordType GRUP = new(0x50555247);
    public static readonly RecordType HEDR = new(0x52444548);
    public static readonly RecordType MAST = new(0x5453414D);
    public static readonly RecordType OFST = new(0x5453464F);
    public static readonly RecordType SNAM = new(0x4D414E53);
    public static readonly RecordType TES3 = new(0x33534554);
    public static readonly RecordType XXXX = new(0x58585858);
}
