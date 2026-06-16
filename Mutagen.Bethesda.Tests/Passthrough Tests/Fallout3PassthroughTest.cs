using Mutagen.Bethesda.Fallout3;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Tests;

public class Fallout3PassthroughTest : PassthroughTest
{
    private readonly Fallout3Release _release;

    protected override Processor ProcessorFactory() => new Fallout3Processor(WorkDropoff, MasterFlagsLookup, GameRelease);

    public Fallout3PassthroughTest(PassthroughTestParams param, GameRelease release)
        : base(param, release)
    {
        _release = release.ToFallout3Release();
    }

    protected override async Task<IModDisposeGetter> ImportBinaryOverlay(FilePath path, StringsReadParameters stringsParams)
    {
        return Fallout3Mod.Create(_release)
            .FromPath(
                new ModPath(ModKey, path.Path))
            .Parallel(parallel: Settings.ParallelModTranslations)
            .ThrowIfUnknownSubrecord()
            .WithStringsParameters(stringsParams)
            .Construct();
    }

    protected override async Task<IMod> ImportBinary(FilePath path, StringsReadParameters stringsParams)
    {
        return Fallout3Mod.Create(_release)
            .FromPath(
                new ModPath(ModKey, path.Path))
            .Parallel(parallel: Settings.ParallelModTranslations)
            .WithStringsParameters(stringsParams)
            .Mutable()
            .ThrowIfUnknownSubrecord()
            .Construct();
    }

    protected override async Task<IMod> ImportCopyIn(FilePath file)
    {
        var wrapper = Fallout3Mod.CreateFromBinaryOverlay(file.Path,
            _release);
        var ret = new Fallout3Mod(ModKey,
            _release);
        ret.DeepCopyIn(wrapper);
        return ret;
    }

    public override AlignmentRules GetAlignmentRules()
    {
        var ret = new AlignmentRules();
        ret.AddAlignments(
            RecordTypes.CELL,
            new RecordType("EDID"),
            new RecordType("FULL"),
            new RecordType("DATA"),
            new RecordType("XCLC"),
            new RecordType("XCLL"),
            new RecordType("IMPF"),
            new RecordType("LTMP"),
            new RecordType("LNAM"),
            new RecordType("XCLW"),
            new RecordType("XNAM"),
            new RecordType("XCLR"),
            new RecordType("XCIM"),
            new RecordType("XCET"),
            new RecordType("XEZN"),
            new RecordType("XCCM"),
            new RecordType("XCWT"),
            new RecordType("XOWN"),
            new RecordType("XRNK"),
            new RecordType("XCAS"),
            new RecordType("XCMT"),
            new RecordType("XCMO"));
        ret.AddAlignments(
            RecordTypes.WRLD,
            new RecordType("EDID"),
            new RecordType("FULL"),
            new RecordType("WNAM"),
            new RecordType("CNAM"),
            new RecordType("NAM2"),
            new RecordType("ICON"),
            new RecordType("MNAM"),
            new RecordType("DATA"),
            new RecordType("NAM0"),
            new RecordType("NAM9"),
            new RecordType("SNAM"),
            new RecordType("XXXX"));
        ret.StopMarkers[RecordTypes.WRLD] = new List<RecordType>()
        {
            new("OFST"),
        };
        ret.AddAlignments(
            RecordTypes.REFR,
            new RecordType("EDID"),
            new RecordType("RCLR"),
            new RecordType("NAME"),
            new RecordType("XEZN"),
            new RecordType("XRGD"),
            new RecordType("XRGB"),
            new RecordType("XPRM"),
            new RecordType("XTRI"),
            new RecordType("XMBP"),
            new RecordType("XMBO"),
            new RecordType("XTEL"),
            new AlignmentSubRule(
                new RecordType("XMRK"),
                new RecordType("FNAM"),
                new RecordType("FULL"),
                new RecordType("TNAM"),
                new RecordType("WMI1"))
            {
                TriggerTypes = new List<RecordType> { new("XMRK") }
            },
            new AlignmentSubRule(
                new RecordType("MMRK"),
                new RecordType("FULL"),
                new RecordType("CNAM"),
                new RecordType("BNAM"),
                new RecordType("MNAM"),
                new RecordType("NNAM"))
            {
                TriggerTypes = new List<RecordType> { new("MMRK") }
            },
            new RecordType("XSRF"),
            new RecordType("XSRD"),
            new RecordType("XTRG"),
            new RecordType("XLCM"),
            new RecordType("XPRD"),
            new RecordType("XPPA"),
            new RecordType("INAM"),
            new RecordType("SCHR"),
            new RecordType("SCDA"),
            new RecordType("SCTX"),
            new RecordType("SLSD"),
            new RecordType("SCVR"),
            AlignmentRepeatedRule.Basic(new RecordType("SCRO"), new RecordType("SCRV")),
            new RecordType("TNAM"),
            new RecordType("XRDO"),
            new RecordType("XOWN"),
            new RecordType("XRNK"),
            new RecordType("XLOC"),
            new RecordType("XCNT"),
            new RecordType("XRDS"),
            new RecordType("XHLP"),
            new RecordType("XRAD"),
            new RecordType("XCHG"),
            new RecordType("XAMT"),
            new RecordType("XAMC"),
            AlignmentRepeatedRule.Basic(new RecordType("XPWR")),
            AlignmentRepeatedRule.Basic(new RecordType("XLTW")),
            AlignmentRepeatedRule.Basic(new RecordType("XDCR")),
            new RecordType("XLKR"),
            new RecordType("XCLP"),
            new RecordType("XAPD"),
            AlignmentRepeatedRule.Basic(new RecordType("XAPR")),
            new RecordType("XATO"),
            new RecordType("XESP"),
            new RecordType("XEMI"),
            new RecordType("XMBR"),
            new RecordType("XACT"),
            new RecordType("ONAM"),
            new RecordType("XIBS"),
            new RecordType("XNDP"),
            new RecordType("XPOD"),
            new RecordType("XPTL"),
            new RecordType("XSED"),
            new RecordType("XRMR"),
            AlignmentRepeatedRule.Basic(new RecordType("XLRM")),
            new RecordType("XOCP"),
            new RecordType("XORD"),
            new RecordType("XLOD"),
            new RecordType("XSCL"),
            new RecordType("DATA"));
        ret.AddAlignments(
            RecordTypes.ACRE,
            new RecordType("EDID"),
            new RecordType("NAME"),
            new RecordType("XEZN"),
            new RecordType("XRGD"),
            new RecordType("XRGB"),
            new RecordType("XPRD"),
            new RecordType("XPPA"),
            new RecordType("INAM"),
            new RecordType("SCHR"),
            new RecordType("SCDA"),
            new RecordType("SCTX"),
            new RecordType("SLSD"),
            new RecordType("SCVR"),
            AlignmentRepeatedRule.Basic(new RecordType("SCRO"), new RecordType("SCRV")),
            new RecordType("TNAM"),
            new RecordType("XLCM"),
            new RecordType("XOWN"),
            new RecordType("XRNK"),
            new RecordType("XMRC"),
            new RecordType("XCNT"),
            new RecordType("XRDS"),
            new RecordType("XHLP"),
            AlignmentRepeatedRule.Basic(new RecordType("XDCR")),
            new RecordType("XLKR"),
            new RecordType("XCLP"),
            new RecordType("XAPD"),
            AlignmentRepeatedRule.Basic(new RecordType("XAPR")),
            new RecordType("XESP"),
            new RecordType("XEMI"),
            new RecordType("XMBR"),
            new RecordType("XATO"),
            new RecordType("XIBS"),
            new RecordType("XSCL"),
            new RecordType("DATA"));
        ret.AddAlignments(
            RecordTypes.ACHR,
            new RecordType("EDID"),
            new RecordType("NAME"),
            new RecordType("XEZN"),
            new RecordType("XRGD"),
            new RecordType("XRGB"),
            new RecordType("XPRD"),
            new RecordType("XPPA"),
            new RecordType("INAM"),
            new RecordType("SCHR"),
            new RecordType("SCDA"),
            new RecordType("SCTX"),
            new RecordType("SLSD"),
            new RecordType("SCVR"),
            AlignmentRepeatedRule.Basic(new RecordType("SCRO"), new RecordType("SCRV")),
            new RecordType("TNAM"),
            new RecordType("XLCM"),
            new RecordType("XMRC"),
            new RecordType("XCNT"),
            new RecordType("XRDS"),
            new RecordType("XHLP"),
            AlignmentRepeatedRule.Basic(new RecordType("XDCR")),
            new RecordType("XLKR"),
            new RecordType("XCLP"),
            new RecordType("XAPD"),
            AlignmentRepeatedRule.Basic(new RecordType("XAPR")),
            new RecordType("XATO"),
            new RecordType("XESP"),
            new RecordType("XEMI"),
            new RecordType("XMBR"),
            new RecordType("XIBS"),
            new RecordType("XSCL"),
            new RecordType("DATA"));
        AlignmentRule[] projectileRules = new AlignmentRule[]
        {
            new RecordType("EDID"),
            new RecordType("NAME"),
            new RecordType("XEZN"),
            new RecordType("XRGD"),
            new RecordType("XRGB"),
            new RecordType("XPRD"),
            new RecordType("XPPA"),
            new RecordType("INAM"),
            new RecordType("SCHR"),
            new RecordType("SCDA"),
            new RecordType("SCTX"),
            new RecordType("SCRO"),
            new RecordType("TNAM"),
            new RecordType("XLCM"),
            new RecordType("XOWN"),
            new RecordType("XRNK"),
            new RecordType("XCNT"),
            new RecordType("XRDS"),
            new RecordType("XHLP"),
            AlignmentRepeatedRule.Basic(new RecordType("XDCR")),
            new RecordType("XLKR"),
            new RecordType("XCLP"),
            new RecordType("XAPD"),
            AlignmentRepeatedRule.Basic(new RecordType("XAPR")),
            new RecordType("XESP"),
            new RecordType("XEMI"),
            new RecordType("XMBR"),
            AlignmentRepeatedRule.Basic(new RecordType("XPWR")),
            new RecordType("XIBS"),
            new RecordType("XSCL"),
            new RecordType("DATA"),
        };
        ret.AddAlignments(
            PlacedGrenade_Registration.TriggeringRecordType,
            projectileRules);
        ret.AddAlignments(
            PlacedMissile_Registration.TriggeringRecordType,
            projectileRules);
        ret.AddAlignments(
            PlacedBeam_Registration.TriggeringRecordType,
            projectileRules);
        ret.AddAlignments(
            RecordTypes.CREA,
            new RecordType("EDID"),
            new RecordType("OBND"),
            new RecordType("FULL"),
            new RecordType("MODL"),
            new RecordType("MODB"),
            new RecordType("MODT"),
            new RecordType("MODS"),
            new RecordType("MODD"),
            AlignmentRepeatedRule.Basic(new RecordType("SPLO")),
            new RecordType("EITM"),
            new RecordType("EAMT"),
            new RecordType("NIFZ"),
            new RecordType("NIFT"),
            new RecordType("ACBS"),
            AlignmentRepeatedRule.Basic(new RecordType("SNAM")),
            new RecordType("INAM"),
            new RecordType("VTCK"),
            new RecordType("TPLT"),
            new RecordType("SCRI"),
            AlignmentRepeatedRule.Basic(new RecordType("CNTO")),
            new RecordType("AIDT"),
            AlignmentRepeatedRule.Basic(new RecordType("PKID")),
            new RecordType("KFFZ"),
            new RecordType("DATA"),
            new RecordType("RNAM"),
            new RecordType("ZNAM"),
            new RecordType("PNAM"),
            new RecordType("TNAM"),
            new RecordType("BNAM"),
            new RecordType("WNAM"),
            new RecordType("NAM4"),
            new RecordType("NAM5"),
            new RecordType("CSCR"),
            AlignmentRepeatedRule.Basic(new RecordType("CSDT")),
            AlignmentRepeatedRule.Basic(new RecordType("CSDI")),
            AlignmentRepeatedRule.Basic(new RecordType("CSDC")),
            new RecordType("CNAM"),
            new RecordType("LNAM"));
        ret.StopMarkers[RecordTypes.CREA] = new List<RecordType>()
        {
            new("DATA"),
        };
        ret.AddAlignments(
            RecordTypes.WEAP,
            new AlignmentSubRule(
                new RecordType("MWD1"),
                new RecordType("MWD2"),
                new RecordType("MWD3"),
                new RecordType("MWD4"),
                new RecordType("MWD5"),
                new RecordType("MWD6"),
                new RecordType("MWD7")),
            new AlignmentSubRule(
                new RecordType("WNM1"),
                new RecordType("WNM2"),
                new RecordType("WNM3"),
                new RecordType("WNM4"),
                new RecordType("WNM5"),
                new RecordType("WNM6"),
                new RecordType("WNM7")));
        ret.SetGroupAlignment(
            (int)GroupTypeEnum.CellTemporaryChildren,
            new RecordType("LAND"),
            new RecordType("NAVM"),
            new RecordType("PGRD"));
        ret.SetTopLevelGroupOrder(
            new RecordType("GMST"),
            new RecordType("TXST"),
            new RecordType("MICN"),
            new RecordType("GLOB"),
            new RecordType("CLAS"),
            new RecordType("FACT"),
            new RecordType("HDPT"),
            new RecordType("HAIR"),
            new RecordType("EYES"),
            new RecordType("RACE"),
            new RecordType("SOUN"),
            new RecordType("ASPC"),
            new RecordType("MGEF"),
            new RecordType("SCPT"),
            new RecordType("LTEX"),
            new RecordType("ENCH"),
            new RecordType("SPEL"),
            new RecordType("ACTI"),
            new RecordType("TACT"),
            new RecordType("TERM"),
            new RecordType("ARMO"),
            new RecordType("BOOK"),
            new RecordType("CONT"),
            new RecordType("DOOR"),
            new RecordType("INGR"),
            new RecordType("LIGH"),
            new RecordType("MISC"),
            new RecordType("STAT"),
            new RecordType("SCOL"),
            new RecordType("MSTT"),
            new RecordType("PWAT"),
            new RecordType("GRAS"),
            new RecordType("TREE"),
            new RecordType("FURN"),
            new RecordType("WEAP"),
            new RecordType("AMMO"),
            new RecordType("NPC_"),
            new RecordType("CREA"),
            new RecordType("LVLC"),
            new RecordType("LVLN"),
            new RecordType("KEYM"),
            new RecordType("ALCH"),
            new RecordType("IDLM"),
            new RecordType("NOTE"),
            new RecordType("PROJ"),
            new RecordType("LVLI"),
            new RecordType("WTHR"),
            new RecordType("CLMT"),
            new RecordType("REGN"),
            new RecordType("NAVI"),
            new RecordType("CELL"),
            new RecordType("WRLD"),
            new RecordType("DIAL"),
            new RecordType("QUST"),
            new RecordType("IDLE"),
            new RecordType("PACK"),
            new RecordType("CSTY"),
            new RecordType("LSCR"),
            new RecordType("ANIO"),
            new RecordType("WATR"),
            new RecordType("EFSH"),
            new RecordType("EXPL"),
            new RecordType("DEBR"),
            new RecordType("IMGS"),
            new RecordType("IMAD"),
            new RecordType("MESG"),
            new RecordType("PERK"),
            new RecordType("BPTD"),
            new RecordType("ADDN"),
            new RecordType("AVIF"),
            new RecordType("RADS"),
            new RecordType("CAMS"),
            new RecordType("CPTH"),
            new RecordType("VTYP"),
            new RecordType("IPCT"),
            new RecordType("IPDS"),
            new RecordType("ARMA"),
            new RecordType("ECZN"),
            new RecordType("RGDL"),
            new RecordType("DOBJ"),
            new RecordType("LGTM"),
            new RecordType("MUSC"),
            new RecordType("FLST"),
            new RecordType("IMOD"),
            new RecordType("REPU"),
            new RecordType("RCPE"),
            new RecordType("RCCT"),
            new RecordType("CHIP"),
            new RecordType("CSNO"),
            new RecordType("LSCT"),
            new RecordType("MSET"),
            new RecordType("ALOC"),
            new RecordType("CHAL"),
            new RecordType("AMEF"));
        return ret;
    }
}
