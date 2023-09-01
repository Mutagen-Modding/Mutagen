using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield.Internals;

public partial class RecordTypes
{
    public static readonly RecordType CHGL = new(0x4C474843);
    public static readonly RecordType CNAM = new(0x4D414E43);
    public static readonly RecordType DATA = new(0x41544144);
    public static readonly RecordType DELE = new(0x454C4544);
    public static readonly RecordType EDID = new(0x44494445);
    public static readonly RecordType GMST = new(0x54534D47);
    public static readonly RecordType GRUP = new(0x50555247);
    public static readonly RecordType HEDR = new(0x52444548);
    public static readonly RecordType INCC = new(0x43434E49);
    public static readonly RecordType INTV = new(0x56544E49);
    public static readonly RecordType MAST = new(0x5453414D);
    public static readonly RecordType MODF = new(0x46444F4D);
    public static readonly RecordType MODL = new(0x4C444F4D);
    public static readonly RecordType NPC_ = new(0x5F43504E);
    public static readonly RecordType OFST = new(0x5453464F);
    public static readonly RecordType ONAM = new(0x4D414E4F);
    public static readonly RecordType RACE = new(0x45434152);
    public static readonly RecordType RNAM = new(0x4D414E52);
    public static readonly RecordType SCRN = new(0x4E524353);
    public static readonly RecordType SNAM = new(0x4D414E53);
    public static readonly RecordType TES4 = new(0x34534554);
    public static readonly RecordType TNAM = new(0x4D414E54);
    public static readonly RecordType WEAP = new(0x50414557);
    public static readonly RecordType XALG = new(0x474C4158);
    public static readonly RecordType XXXX = new(0x58585858);
}
