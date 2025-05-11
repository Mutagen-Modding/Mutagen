using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Tests;

public class OblivionPassthroughTest : PassthroughTest
{
    protected override Processor ProcessorFactory() => new OblivionProcessor(WorkDropoff, MasterFlagsLookup);

    public OblivionPassthroughTest(PassthroughTestParams param, GameRelease release)
        : base(param, release)
    {
    }

    protected override async Task<IModDisposeGetter> ImportBinaryOverlay(FilePath path, StringsReadParameters stringsParams)
    {
        return OblivionMod.Create(OblivionRelease.Oblivion)
            .FromPath(
                new ModPath(ModKey, path.Path))
            .Parallel(parallel: Settings.ParallelModTranslations)
            .ThrowIfUnknownSubrecord()
            .WithStringsParameters(stringsParams)
            .Construct();
    }

    protected override async Task<IMod> ImportBinary(FilePath path, StringsReadParameters stringsParams)
    {
        return OblivionMod.Create(OblivionRelease.Oblivion)
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
        var wrapper = OblivionMod.CreateFromBinaryOverlay(file.Path,
            OblivionRelease.Oblivion);
        var ret = new OblivionMod(ModKey,
            OblivionRelease.Oblivion);
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
            new RecordType("FULL"),
            new RecordType("MODL"),
            new RecordType("MODB"),
            new RecordType("MODT"),
            AlignmentRepeatedRule.Basic(new RecordType("CNTO")),
            AlignmentRepeatedRule.Basic(new RecordType("SPLO")),
            AlignmentRepeatedRule.Basic(new RecordType("NIFZ")),
            new RecordType("NIFT"),
            new RecordType("ACBS"),
            AlignmentRepeatedRule.Basic(new RecordType("SNAM")),
            new RecordType("INAM"),
            new RecordType("SCRI"),
            new RecordType("AIDT"),
            AlignmentRepeatedRule.Basic(new RecordType("PKID")),
            AlignmentRepeatedRule.Basic(new RecordType("KFFZ")),
            new RecordType("DATA"),
            new RecordType("RNAM"),
            new RecordType("ZNAM"),
            new RecordType("TNAM"),
            new RecordType("BNAM"),
            new RecordType("WNAM"),
            new RecordType("NAM0"),
            new RecordType("NAM1"));
        ret.StopMarkers[RecordTypes.CREA] = new List<RecordType>()
        {
            new("DATA"),
        };
        ret.SetGroupAlignment(
            (int)GroupTypeEnum.CellTemporaryChildren,
            new RecordType("LAND"),
            new RecordType("PGRD"));
        return ret;
    }
}
