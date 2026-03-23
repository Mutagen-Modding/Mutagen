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

    protected override Processor ProcessorFactory() => new Fallout3Processor(WorkDropoff, MasterFlagsLookup);

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
            new RecordType("XCLR"),
            new RecordType("XCMT"),
            new RecordType("XCLW"),
            new RecordType("XCCM"),
            new RecordType("XCWT"),
            new RecordType("XOWN"),
            new RecordType("XRNK"),
            new RecordType("XGLB"),
            new RecordType("XTLI"),
            new RecordType("XLRL"));
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
            new RecordType("NAME"),
            new RecordType("XPCI"),
            new RecordType("FULL"),
            new RecordType("XTEL"),
            new RecordType("XLOC"),
            new RecordType("XOWN"),
            new RecordType("XRNK"),
            new RecordType("XGLB"),
            new RecordType("XESP"),
            new RecordType("XTRG"),
            new RecordType("XSED"),
            new RecordType("XLOD"),
            new RecordType("XCHG"),
            new RecordType("XHLT"),
            new RecordType("XLCM"),
            new RecordType("XRTM"),
            new RecordType("XACT"),
            new RecordType("XCNT"),
            new AlignmentSubRule(
                new RecordType("XMRK"),
                new RecordType("FNAM"),
                new RecordType("FULL"),
                new RecordType("TNAM")),
            new RecordType("ONAM"),
            new RecordType("XRGD"),
            new RecordType("XSCL"),
            new RecordType("XSOL"),
            new RecordType("DATA"),
            new RecordType("XAAG"),
            new RecordType("XACN"));
        ret.AddAlignments(
            RecordTypes.ACRE,
            new RecordType("EDID"),
            new RecordType("NAME"),
            new RecordType("XOWN"),
            new RecordType("XRNK"),
            new RecordType("XGLB"),
            new RecordType("XESP"),
            new RecordType("XRGD"),
            new RecordType("XSCL"),
            new RecordType("DATA"));
        ret.AddAlignments(
            RecordTypes.ACHR,
            new RecordType("EDID"),
            new RecordType("NAME"),
            new RecordType("XPCI"),
            new RecordType("FULL"),
            new RecordType("XLOD"),
            new RecordType("XESP"),
            new RecordType("XMRC"),
            new RecordType("XHRS"),
            new RecordType("XRGD"),
            new RecordType("XSCL"),
            new RecordType("DATA"));
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
            new RecordType("KEYM"));
        return ret;
    }
}
