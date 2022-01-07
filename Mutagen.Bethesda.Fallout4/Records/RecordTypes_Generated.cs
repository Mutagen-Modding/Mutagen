using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout4.Internals
{
    public class RecordTypes
    {
        public static readonly RecordType AACT = new(0x54434141);
        public static readonly RecordType AORU = new(0x55524F41);
        public static readonly RecordType APPR = new(0x52505041);
        public static readonly RecordType ARMO = new(0x4F4D5241);
        public static readonly RecordType AVIF = new(0x46495641);
        public static readonly RecordType BOD2 = new(0x32444F42);
        public static readonly RecordType CELL = new(0x4C4C4543);
        public static readonly RecordType CITC = new(0x43544943);
        public static readonly RecordType CLAS = new(0x53414C43);
        public static readonly RecordType CLFM = new(0x4D464C43);
        public static readonly RecordType CMPO = new(0x4F504D43);
        public static readonly RecordType CNAM = new(0x4D414E43);
        public static readonly RecordType CRGR = new(0x52475243);
        public static readonly RecordType CRVA = new(0x41565243);
        public static readonly RecordType CTDA = new(0x41445443);
        public static readonly RecordType CUSD = new(0x44535543);
        public static readonly RecordType DATA = new(0x41544144);
        public static readonly RecordType DEBR = new(0x52424544);
        public static readonly RecordType DELE = new(0x454C4544);
        public static readonly RecordType DESC = new(0x43534544);
        public static readonly RecordType DMGT = new(0x54474D44);
        public static readonly RecordType DNAM = new(0x4D414E44);
        public static readonly RecordType DODT = new(0x54444F44);
        public static readonly RecordType DOOR = new(0x524F4F44);
        public static readonly RecordType EXPL = new(0x4C505845);
        public static readonly RecordType FACT = new(0x54434146);
        public static readonly RecordType FLST = new(0x54534C46);
        public static readonly RecordType FLTV = new(0x56544C46);
        public static readonly RecordType FNAM = new(0x4D414E46);
        public static readonly RecordType FULL = new(0x4C4C5546);
        public static readonly RecordType GLOB = new(0x424F4C47);
        public static readonly RecordType GMST = new(0x54534D47);
        public static readonly RecordType GNAM = new(0x4D414E47);
        public static readonly RecordType GRUP = new(0x50555247);
        public static readonly RecordType HDPT = new(0x54504448);
        public static readonly RecordType HEDR = new(0x52444548);
        public static readonly RecordType HNAM = new(0x4D414E48);
        public static readonly RecordType ICON = new(0x4E4F4349);
        public static readonly RecordType INAM = new(0x4D414E49);
        public static readonly RecordType INCC = new(0x43434E49);
        public static readonly RecordType INTV = new(0x56544E49);
        public static readonly RecordType IPDS = new(0x53445049);
        public static readonly RecordType JAIL = new(0x4C49414A);
        public static readonly RecordType JOUT = new(0x54554F4A);
        public static readonly RecordType KSIZ = new(0x5A49534B);
        public static readonly RecordType KWDA = new(0x4144574B);
        public static readonly RecordType KYWD = new(0x4457594B);
        public static readonly RecordType LCRT = new(0x5452434C);
        public static readonly RecordType LVSP = new(0x5053564C);
        public static readonly RecordType MAST = new(0x5453414D);
        public static readonly RecordType MISC = new(0x4353494D);
        public static readonly RecordType MNAM = new(0x4D414E4D);
        public static readonly RecordType MODC = new(0x43444F4D);
        public static readonly RecordType MODF = new(0x46444F4D);
        public static readonly RecordType MODL = new(0x4C444F4D);
        public static readonly RecordType MODS = new(0x53444F4D);
        public static readonly RecordType MODT = new(0x54444F4D);
        public static readonly RecordType MSWP = new(0x5057534D);
        public static readonly RecordType NAM0 = new(0x304D414E);
        public static readonly RecordType NAM1 = new(0x314D414E);
        public static readonly RecordType NNAM = new(0x4D414E4E);
        public static readonly RecordType OBND = new(0x444E424F);
        public static readonly RecordType OFST = new(0x5453464F);
        public static readonly RecordType ONAM = new(0x4D414E4F);
        public static readonly RecordType OTFT = new(0x5446544F);
        public static readonly RecordType PLCN = new(0x4E434C50);
        public static readonly RecordType PLVD = new(0x44564C50);
        public static readonly RecordType PNAM = new(0x4D414E50);
        public static readonly RecordType PRPS = new(0x53505250);
        public static readonly RecordType RACE = new(0x45434152);
        public static readonly RecordType REFR = new(0x52464552);
        public static readonly RecordType REPT = new(0x54504552);
        public static readonly RecordType RNAM = new(0x4D414E52);
        public static readonly RecordType SCRN = new(0x4E524353);
        public static readonly RecordType SDSC = new(0x43534453);
        public static readonly RecordType SNAM = new(0x4D414E53);
        public static readonly RecordType SNDR = new(0x52444E53);
        public static readonly RecordType SOUN = new(0x4E554F53);
        public static readonly RecordType SPCT = new(0x54435053);
        public static readonly RecordType SPLO = new(0x4F4C5053);
        public static readonly RecordType STAG = new(0x47415453);
        public static readonly RecordType STCP = new(0x50435453);
        public static readonly RecordType STOL = new(0x4C4F5453);
        public static readonly RecordType TES4 = new(0x34534554);
        public static readonly RecordType TNAM = new(0x4D414E54);
        public static readonly RecordType TRNS = new(0x534E5254);
        public static readonly RecordType TX00 = new(0x30305854);
        public static readonly RecordType TX01 = new(0x31305854);
        public static readonly RecordType TX02 = new(0x32305854);
        public static readonly RecordType TX03 = new(0x33305854);
        public static readonly RecordType TX04 = new(0x34305854);
        public static readonly RecordType TX05 = new(0x35305854);
        public static readonly RecordType TX06 = new(0x36305854);
        public static readonly RecordType TX07 = new(0x37305854);
        public static readonly RecordType TXST = new(0x54535854);
        public static readonly RecordType VENC = new(0x434E4556);
        public static readonly RecordType VEND = new(0x444E4556);
        public static readonly RecordType VENV = new(0x564E4556);
        public static readonly RecordType WAIT = new(0x54494157);
        public static readonly RecordType WNAM = new(0x4D414E57);
        public static readonly RecordType XNAM = new(0x4D414E58);
        public static readonly RecordType XXXX = new(0x58585858);
    }
}
