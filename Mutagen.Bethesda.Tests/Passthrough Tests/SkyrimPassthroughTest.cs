using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Noggog;

namespace Mutagen.Bethesda.Tests;

public class SkyrimPassthroughTest : PassthroughTest
{
    public override GameRelease GameRelease { get; }

    public SkyrimPassthroughTest(PassthroughTestParams param, GameRelease mode)
        : base(param)
    {
        GameRelease = mode;
    }

    public override AlignmentRules GetAlignmentRules()
    {
        var ret = new AlignmentRules();
        ret.AddAlignments(
            RecordTypes.CELL,
            RecordTypes.EDID,
            RecordTypes.FULL,
            RecordTypes.DATA,
            RecordTypes.XCLC,
            RecordTypes.XCLL,
            RecordTypes.TVDT,
            RecordTypes.MHDT,
            RecordTypes.LTMP,
            RecordTypes.LNAM,
            RecordTypes.XCLW,
            RecordTypes.XNAM,
            RecordTypes.XCLR,
            RecordTypes.XLCN,
            RecordTypes.XWCN,
            RecordTypes.XWCS,
            RecordTypes.XWCU,
            RecordTypes.XCWT,
            RecordTypes.XOWN,
            RecordTypes.XRNK,
            RecordTypes.XILL,
            RecordTypes.XWEM,
            RecordTypes.XCCM,
            RecordTypes.XCAS,
            RecordTypes.XEZN,
            RecordTypes.XCMO,
            RecordTypes.XCIM
        );
        ret.AddAlignments(
            RecordTypes.REFR,
            RecordTypes.EDID,
            RecordTypes.VMAD,
            RecordTypes.NAME,
            RecordTypes.XMBO,
            RecordTypes.XPRM,
            RecordTypes.XORD,
            RecordTypes.XOCP,
            RecordTypes.XPOD,
            RecordTypes.XPTL,
            RecordTypes.XRMR,
            RecordTypes.LNAM,
            RecordTypes.INAM,
            AlignmentRepeatedRule.Basic(RecordTypes.XLRM),
            RecordTypes.XMBP,
            RecordTypes.XRGD,
            RecordTypes.XRGB,
            RecordTypes.XRDS,
            AlignmentRepeatedRule.Basic(RecordTypes.XPWR),
            AlignmentRepeatedRule.Basic(RecordTypes.XLTW),
            RecordTypes.XEMI,
            RecordTypes.XLIG,
            RecordTypes.XALP,
            RecordTypes.XTEL,
            RecordTypes.XTNM,
            RecordTypes.XMBR,
            RecordTypes.XWCN,
            RecordTypes.XWCS,
            RecordTypes.XWCU,
            RecordTypes.XCVL,
            RecordTypes.XCZR,
            RecordTypes.XCZA,
            RecordTypes.XCZC,
            RecordTypes.XSCL,
            RecordTypes.XSPC,
            RecordTypes.XAPD,
            AlignmentRepeatedRule.Basic(RecordTypes.XAPR),
            RecordTypes.XLIB,
            RecordTypes.XLCM,
            RecordTypes.XLCN,
            RecordTypes.XTRI,
            RecordTypes.XLOC,
            RecordTypes.XEZN,
            RecordTypes.XNDP,
            AlignmentRepeatedRule.Basic(RecordTypes.XLRT),
            RecordTypes.XIS2,
            RecordTypes.XOWN,
            RecordTypes.XRNK,
            RecordTypes.XCNT,
            RecordTypes.XCHG,
            RecordTypes.XLRL,
            RecordTypes.XESP,
            AlignmentRepeatedRule.Basic(RecordTypes.XLKR),
            new AlignmentSubRule(
                RecordTypes.XPRD,
                RecordTypes.XPPA,
                RecordTypes.INAM,
                RecordTypes.SCHR,
                RecordTypes.SCTX),
            AlignmentRepeatedRule.Basic(RecordTypes.PDTO),
            RecordTypes.XACT,
            RecordTypes.XHTW,
            RecordTypes.XFVC,
            RecordTypes.ONAM,
            RecordTypes.XMRK,
            RecordTypes.FNAM,
            RecordTypes.FULL,
            RecordTypes.TNAM,
            RecordTypes.XATR,
            RecordTypes.XLOD,
            RecordTypes.DATA
        );
        ret.AddAlignments(
            RecordTypes.ACHR,
            RecordTypes.EDID,
            RecordTypes.VMAD,
            RecordTypes.NAME,
            RecordTypes.XEZN,
            RecordTypes.XRGD,
            RecordTypes.XRGB,
            new AlignmentSubRule(
                RecordTypes.XPRD,
                RecordTypes.XPPA,
                RecordTypes.INAM,
                RecordTypes.SCHR,
                RecordTypes.SCTX),
            AlignmentRepeatedRule.Basic(RecordTypes.PDTO),
            RecordTypes.XLCM,
            RecordTypes.XMRC,
            RecordTypes.XCNT,
            RecordTypes.XRDS,
            RecordTypes.XHLP,
            AlignmentRepeatedRule.Basic(RecordTypes.XLKR),
            RecordTypes.XAPD,
            AlignmentRepeatedRule.Basic(RecordTypes.XAPR),
            RecordTypes.XCLP,
            RecordTypes.XLCN,
            RecordTypes.XLRL,
            RecordTypes.XIS2,
            AlignmentRepeatedRule.Basic(RecordTypes.XLRT),
            RecordTypes.XHTW,
            RecordTypes.XHOR,
            RecordTypes.XFVC,
            RecordTypes.XESP,
            RecordTypes.XOWN,
            RecordTypes.XRNK,
            RecordTypes.XEMI,
            RecordTypes.XMBR,
            RecordTypes.XIBS,
            RecordTypes.XSCL,
            RecordTypes.DATA);
        AlignmentRule[] trapRules = new AlignmentRule[]
        {
            RecordTypes.EDID,
            RecordTypes.VMAD,
            RecordTypes.NAME,
            RecordTypes.XEZN,
            RecordTypes.XOWN,
            RecordTypes.XRNK,
            RecordTypes.XHTW,
            RecordTypes.XFVC,
            RecordTypes.XPWR,
            RecordTypes.XLKR,
            RecordTypes.XAPD,
            AlignmentRepeatedRule.Basic(RecordTypes.XAPR),
            RecordTypes.XESP,
            RecordTypes.XEMI,
            RecordTypes.XMBR,
            RecordTypes.XIS2,
            RecordTypes.XLRT,
            RecordTypes.XLRL,
            RecordTypes.XLOD,
            RecordTypes.XSCL,
            RecordTypes.DATA,
        };
        ret.AddAlignments(
            PlacedArrow_Registration.TriggeringRecordType,
            trapRules);
        ret.AddAlignments(
            PlacedBarrier_Registration.TriggeringRecordType,
            trapRules);
        ret.AddAlignments(
            PlacedBeam_Registration.TriggeringRecordType,
            trapRules);
        ret.AddAlignments(
            PlacedCone_Registration.TriggeringRecordType,
            trapRules);
        ret.AddAlignments(
            PlacedFlame_Registration.TriggeringRecordType,
            trapRules);
        ret.AddAlignments(
            PlacedHazard_Registration.TriggeringRecordType,
            trapRules);
        ret.AddAlignments(
            PlacedMissile_Registration.TriggeringRecordType,
            trapRules);
        ret.AddAlignments(
            PlacedTrap_Registration.TriggeringRecordType,
            trapRules);
        ret.SetGroupAlignment(
            (int)GroupTypeEnum.CellTemporaryChildren,
            RecordTypes.LAND,
            RecordTypes.NAVM);
        return ret;
    }

    protected override async Task<IModDisposeGetter> ImportBinaryOverlay(FilePath path)
    {
        return SkyrimModBinaryOverlay.SkyrimModFactory(
            new ModPath(ModKey, FilePath.Path),
            GameRelease.ToSkyrimRelease());
    }

    protected override async Task<IMod> ImportBinary(FilePath path)
    {
        return SkyrimMod.CreateFromBinary(
            new ModPath(ModKey, path.Path),
            GameRelease.ToSkyrimRelease(),
            parallel: Settings.ParallelProccessingSteps);
    }

    protected override async Task<IMod> ImportCopyIn(FilePath file)
    {
        var wrapper = SkyrimMod.CreateFromBinaryOverlay(file.Path, GameRelease.ToSkyrimRelease());
        var ret = new SkyrimMod(ModKey, GameRelease.ToSkyrimRelease());
        ret.DeepCopyIn(wrapper);
        return ret;
    }

    protected override Processor ProcessorFactory() => new SkyrimProcessor(GameRelease, Settings.ParallelProccessingSteps);
}