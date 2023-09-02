using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield.Internals;

public partial class RecordTypes
{
    public static readonly RecordType AACT = new(0x54434141);
    public static readonly RecordType AORU = new(0x55524F41);
    public static readonly RecordType BFCB = new(0x42434642);
    public static readonly RecordType BFCE = new(0x45434642);
    public static readonly RecordType BNAM = new(0x4D414E42);
    public static readonly RecordType CHGL = new(0x4C474843);
    public static readonly RecordType CNAM = new(0x4D414E43);
    public static readonly RecordType DATA = new(0x41544144);
    public static readonly RecordType DELE = new(0x454C4544);
    public static readonly RecordType DNAM = new(0x4D414E44);
    public static readonly RecordType EDID = new(0x44494445);
    public static readonly RecordType ENAM = new(0x4D414E45);
    public static readonly RecordType FFKW = new(0x574B4646);
    public static readonly RecordType FLTR = new(0x52544C46);
    public static readonly RecordType FNAM = new(0x4D414E46);
    public static readonly RecordType FULL = new(0x4C4C5546);
    public static readonly RecordType GMST = new(0x54534D47);
    public static readonly RecordType GRUP = new(0x50555247);
    public static readonly RecordType HEDR = new(0x52444548);
    public static readonly RecordType INCC = new(0x43434E49);
    public static readonly RecordType INTV = new(0x56544E49);
    public static readonly RecordType KYWD = new(0x4457594B);
    public static readonly RecordType LCRT = new(0x5452434C);
    public static readonly RecordType MAST = new(0x5453414D);
    public static readonly RecordType MODF = new(0x46444F4D);
    public static readonly RecordType MODL = new(0x4C444F4D);
    public static readonly RecordType NPC_ = new(0x5F43504E);
    public static readonly RecordType OFST = new(0x5453464F);
    public static readonly RecordType ONAM = new(0x4D414E4F);
    public static readonly RecordType RACE = new(0x45434152);
    public static readonly RecordType REFL = new(0x4C464552);
    public static readonly RecordType RNAM = new(0x4D414E52);
    public static readonly RecordType SCRN = new(0x4E524353);
    public static readonly RecordType SNAM = new(0x4D414E53);
    public static readonly RecordType TES4 = new(0x34534554);
    public static readonly RecordType TNAM = new(0x4D414E54);
    public static readonly RecordType TRNS = new(0x534E5254);
    public static readonly RecordType WEAP = new(0x50414557);
    public static readonly RecordType XALG = new(0x474C4158);
    public static readonly RecordType XXXX = new(0x58585858);
}
