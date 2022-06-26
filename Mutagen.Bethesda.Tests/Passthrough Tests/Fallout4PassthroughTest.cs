using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Tests;

public class Fallout4PassthroughTest : PassthroughTest
{
    public override GameRelease GameRelease { get; }

    public Fallout4PassthroughTest(PassthroughTestParams param)
        : base(param)
    {
        GameRelease = GameRelease.Fallout4;
    }

    public override AlignmentRules GetAlignmentRules()
    {
        var ret = new AlignmentRules();
        ret.StartMarkers.Add(RecordTypes.RACE, new[]
        {
            RecordTypes.SGNM,
            RecordTypes.SAKD,
            RecordTypes.STKD,
            RecordTypes.SAPT,
            RecordTypes.SRAF,
        });
        ret.StopMarkers.Add(RecordTypes.RACE, new[]
        {
            RecordTypes.BSMP,
        });
        ret.AddAlignments(
            RecordTypes.RACE,
            AlignmentRepeatedRule.Sorted(
                new AlignmentRepeatedSubrule(RecordTypes.SGNM, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.SAKD, Single: false),
                new AlignmentRepeatedSubrule(RecordTypes.STKD, Single: false),
                new AlignmentRepeatedSubrule(RecordTypes.SAPT, Single: false),
                new AlignmentRepeatedSubrule(RecordTypes.SRAF, Single: true)
                {
                    Ender = true
                }),
            RecordTypes.PTOP,
            RecordTypes.NTOP,
            AlignmentRepeatedRule.Basic(
                RecordTypes.MSID,
                RecordTypes.MSM0,
                RecordTypes.MSM1),
            RecordTypes.MLSI,
            RecordTypes.HNAM,
            RecordTypes.HLTX,
            RecordTypes.QSTI
        );
        ret.AddAlignments(
            RecordTypes.STAT,
            RecordTypes.MODL,
            RecordTypes.MODC,
            RecordTypes.MODT,
            RecordTypes.MODS,
            RecordTypes.PRPS,
            RecordTypes.FULL,
            RecordTypes.DNAM,
            RecordTypes.XXXX,
            RecordTypes.NVNM,
            RecordTypes.MNAM
        );
        ret.AddAlignments(
            RecordTypes.MSTT,
            RecordTypes.EDID,
            RecordTypes.VMAD,
            RecordTypes.OBND,
            RecordTypes.PTRN,
            RecordTypes.FULL,
            RecordTypes.MODL,
            RecordTypes.MODC,
            RecordTypes.MODT,
            RecordTypes.MODS,
            RecordTypes.MODF
        );
        ret.StopMarkers.Add(RecordTypes.MSTT, new[]
        {
            RecordTypes.DEST,
            RecordTypes.DAMC,
            RecordTypes.DSTD,
            RecordTypes.DSTA,
            RecordTypes.DMDL,
            RecordTypes.DMDT,
            RecordTypes.DMDS,
            RecordTypes.DSTF,
            RecordTypes.KWDA,
            RecordTypes.KSIZ,
            RecordTypes.PRPS,
            RecordTypes.DATA,
            RecordTypes.SNAM
        });
        ret.AddAlignments(
            RecordTypes.LVLI,
            RecordTypes.EDID,
            RecordTypes.OBND,
            RecordTypes.LVLD,
            RecordTypes.LVLM,
            RecordTypes.LVLF
        );
        ret.StopMarkers.Add(RecordTypes.LVLI, new[]
        {
            RecordTypes.LVLG,
            RecordTypes.LLCT,
            RecordTypes.LLKC,
            RecordTypes.LVSG,
            RecordTypes.ONAM,
        });
        ret.AddAlignments(
            RecordTypes.CELL,
            RecordTypes.EDID,
            RecordTypes.FULL,
            RecordTypes.DATA,
            RecordTypes.VISI,
            RecordTypes.RVIS,
            RecordTypes.PCMB,
            RecordTypes.XCLC,
            RecordTypes.XCLL,
            RecordTypes.CNAM,
            RecordTypes.ZNAM,
            RecordTypes.TVDT,
            RecordTypes.MHDT,
            RecordTypes.LTMP,
            RecordTypes.XCLW,
            RecordTypes.XCLR,
            RecordTypes.XLCN,
            RecordTypes.XWCN,
            RecordTypes.XWCU,
            RecordTypes.XCWT,
            RecordTypes.XOWN,
            RecordTypes.XRNK,
            RecordTypes.XILL,
            RecordTypes.XILW,
            RecordTypes.XWEM,
            RecordTypes.XCCM,
            RecordTypes.XCAS,
            RecordTypes.XEZN,
            RecordTypes.XCMO,
            RecordTypes.XCIM,
            RecordTypes.XGDR,
            RecordTypes.XPRI,
            RecordTypes.XCRI
        );
        ret.StopMarkers.Add(RecordTypes.CELL, new[]
        {
            RecordTypes.NAVM,
            RecordTypes.NVNM,
            RecordTypes.ONAM,
            RecordTypes.NNAM,
            RecordTypes.MNAM,
            RecordTypes.ACHR,
            RecordTypes.REFR,
            RecordTypes.VMAD,
            RecordTypes.NAME,
            RecordTypes.XHTW,
            RecordTypes.XFVC,
            RecordTypes.XPWR,
            RecordTypes.XLKR,
            RecordTypes.XAPD,
            RecordTypes.XASP,
            RecordTypes.XATP,
            RecordTypes.XAMC,
            RecordTypes.XLKT,
            RecordTypes.XLYR,
            RecordTypes.XMSP,
            RecordTypes.XRFG,
            RecordTypes.XCVR,
            RecordTypes.XESP,
            RecordTypes.XEMI,
            RecordTypes.XMBR,
            RecordTypes.XIS2,
            RecordTypes.XLRT,
            RecordTypes.XLRL,
            RecordTypes.XSCL,
            RecordTypes.XLOD,
            RecordTypes.XXXX,
        });
        var trapRules = new AlignmentRule[]
        {
            RecordTypes.EDID,
            RecordTypes.VMAD,
            RecordTypes.NAME,
            RecordTypes.XEZN,
            RecordTypes.XHTW,
            RecordTypes.XFVC,
            RecordTypes.XPWR,
            RecordTypes.XLKR,
            RecordTypes.XAPD,
            RecordTypes.XAPR,
            RecordTypes.XASP,
            RecordTypes.XATP,
            RecordTypes.XAMC,
            RecordTypes.XLKT,
            RecordTypes.XLYR,
            RecordTypes.XMSP,
            RecordTypes.XRFG,
            RecordTypes.XCVR,
            RecordTypes.XESP,
            RecordTypes.XOWN,
            RecordTypes.XRNK,
            RecordTypes.XEMI,
            RecordTypes.XMBR,
            RecordTypes.XIS2,
            AlignmentRepeatedRule.Basic(RecordTypes.XLRT),
            RecordTypes.XLRL,
            RecordTypes.XSCL,
            AlignmentRepeatedRule.Basic(RecordTypes.XLOD),
            RecordTypes.DATA,
            RecordTypes.MNAM
        };
        ret.AddAlignments(
            RecordTypes.PHZD,
            trapRules);
        ret.AddAlignments(
            RecordTypes.PGRE,
            trapRules);
        ret.AddAlignments(
            RecordTypes.ACHR,
            RecordTypes.EDID,
            RecordTypes.VMAD,
            RecordTypes.NAME,
            RecordTypes.XEZN,
            RecordTypes.XRGD,
            RecordTypes.XRGB,
            RecordTypes.XPRD,
            RecordTypes.INAM,
            RecordTypes.PDTO,
            RecordTypes.TNAM,
            RecordTypes.XLCM,
            RecordTypes.XCNT,
            RecordTypes.XRDS,
            RecordTypes.XHLT,
            AlignmentRepeatedRule.Basic(RecordTypes.XLKR),
            RecordTypes.XAPD,
            AlignmentRepeatedRule.Basic(RecordTypes.XAPR),
            RecordTypes.XATP,
            RecordTypes.XLKT,
            RecordTypes.XRFG,
            RecordTypes.XLYR,
            RecordTypes.XMSP,
            RecordTypes.XLCN,
            RecordTypes.XLRL,
            AlignmentRepeatedRule.Basic(RecordTypes.XLRT),
            RecordTypes.XIS2,
            AlignmentRepeatedRule.Basic(RecordTypes.XPLK),
            RecordTypes.XHTW,
            RecordTypes.XFVC,
            RecordTypes.XESP,
            RecordTypes.XOWN,
            RecordTypes.XRNK,
            RecordTypes.XEMI,
            RecordTypes.XMBR,
            RecordTypes.XIBS,
            RecordTypes.XSCL,
            RecordTypes.DATA,
            RecordTypes.MNAM);
        ret.AddAlignments(
            RecordTypes.REFR,
            RecordTypes.EDID,
            RecordTypes.VMAD,
            RecordTypes.NAME,
            RecordTypes.XMBO,
            RecordTypes.XPRM,
            AlignmentRepeatedRule.Basic(RecordTypes.XPOD),
            RecordTypes.XPTL,
            RecordTypes.XORD,
            RecordTypes.XOCP,
            RecordTypes.XRMR,
            RecordTypes.LNAM,
            RecordTypes.INAM,
            AlignmentRepeatedRule.Basic(RecordTypes.XLRM),
            RecordTypes.XMBP,
            RecordTypes.XRGD,
            RecordTypes.XRGB,
            RecordTypes.XRDS,
            RecordTypes.XEMI,
            RecordTypes.XLIG,
            AlignmentRepeatedRule.Basic(RecordTypes.XLTW),
            RecordTypes.XALP,
            RecordTypes.XTEL,
            RecordTypes.XTNM,
            RecordTypes.XMBR,
            RecordTypes.XWCN,
            RecordTypes.XWCU,
            RecordTypes.XASP,
            RecordTypes.XATP,
            RecordTypes.XAMC,
            RecordTypes.XLKT,
            RecordTypes.XLYR,
            RecordTypes.XMSP,
            RecordTypes.XRFG,
            RecordTypes.XRDO,
            RecordTypes.XBSD,
            RecordTypes.XPDD,
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
            RecordTypes.XHLT,
            RecordTypes.XESP,
            AlignmentRepeatedRule.Basic(RecordTypes.XLKR),
            AlignmentRepeatedRule.Sorted(
                new AlignmentRepeatedSubrule(RecordTypes.XPRD, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.XPPA, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.INAM, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.PDTO, Single: false),
                new AlignmentRepeatedSubrule(RecordTypes.TNAM, Single: true)),
            RecordTypes.XACT,
            RecordTypes.XHTW,
            RecordTypes.XFVC,
            RecordTypes.ONAM,
            AlignmentRepeatedRule.Sorted(
                new AlignmentRepeatedSubrule(RecordTypes.XMRK, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.FNAM, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.FULL, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.TNAM, Single: true)),
            RecordTypes.XATR,
            AlignmentRepeatedRule.Basic(RecordTypes.XPLK),
            AlignmentRepeatedRule.Sorted(
                new AlignmentRepeatedSubrule(RecordTypes.XWPG, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.XWPN, Single: false)),
            RecordTypes.XCVR,
            RecordTypes.XCVL,
            RecordTypes.XCZR,
            RecordTypes.XCZA,
            RecordTypes.XCZC,
            RecordTypes.XSCL,
            RecordTypes.XLOD,
            RecordTypes.DATA,
            RecordTypes.MNAM
        );
        ret.AddAlignments(
            RecordTypes.ANIO,
            RecordTypes.EDID,
            RecordTypes.MODL,
            RecordTypes.MODC,
            RecordTypes.MODT,
            RecordTypes.MODS,
            RecordTypes.BNAM);
        ret.SetGroupAlignment(
            (int)GroupTypeEnum.CellTemporaryChildren,
            RecordTypes.LAND,
            RecordTypes.NAVM);
        ret.SetGroupAlignment(
            (int)GroupTypeEnum.QuestChildren,
            RecordTypes.DLBR,
            RecordTypes.DIAL,
            RecordTypes.SCEN);
        ret.AddAlignments(
            RecordTypes.ARMA,
            RecordTypes.MOD2,
            RecordTypes.MO2C,
            RecordTypes.MO2T,
            RecordTypes.MO2S,
            RecordTypes.MO2F,
            RecordTypes.MOD3,
            RecordTypes.MO3C,
            RecordTypes.MO3T,
            RecordTypes.MO3S,
            RecordTypes.MO3F,
            RecordTypes.MOD4,
            RecordTypes.MO4C,
            RecordTypes.MO4T,
            RecordTypes.MO4S,
            RecordTypes.MO4F,
            RecordTypes.MOD5,
            RecordTypes.MO5C,
            RecordTypes.MO5T,
            RecordTypes.MO5S,
            RecordTypes.MO5F
        );
        ret.AddAlignments(
            RecordTypes.ARTO,
            RecordTypes.MODL,
            RecordTypes.MODC,
            RecordTypes.MODT,
            RecordTypes.MODS
        );
        ret.AddAlignments(
            RecordTypes.ACTI,
            RecordTypes.MODL,
            RecordTypes.MODC,
            RecordTypes.MODT,
            RecordTypes.MODS,
            RecordTypes.KSIZ,
            AlignmentRepeatedRule.Basic(RecordTypes.KWDA),
            RecordTypes.PNAM,
            RecordTypes.FNAM
        );
        ret.AddAlignments(
            RecordTypes.SCOL,
            RecordTypes.MODL,
            RecordTypes.MODC,
            RecordTypes.MODT,
            RecordTypes.MODS,
            RecordTypes.FULL,
            RecordTypes.FLTR
        );
        ret.AddAlignments(
            RecordTypes.BPTD,
            RecordTypes.EDID,
            RecordTypes.MODL,
            RecordTypes.MODC,
            RecordTypes.MODT,
            RecordTypes.MODS
        );
        ret.AddAlignments(
            RecordTypes.FURN,
            RecordTypes.MODL,
            RecordTypes.MODC,
            RecordTypes.MODT,
            RecordTypes.MODS,
            RecordTypes.DEST,
            RecordTypes.DAMC,
            AlignmentRepeatedRule.Sorted(
                new AlignmentRepeatedSubrule(RecordTypes.DSTD, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.DSTA, Single: false),
                new AlignmentRepeatedSubrule(RecordTypes.DMDL, Single: false),
                new AlignmentRepeatedSubrule(RecordTypes.DMDT, Single: false),
                new AlignmentRepeatedSubrule(RecordTypes.DMDS, Single: false),
                new AlignmentRepeatedSubrule(RecordTypes.DSTF, Single: false)),
            RecordTypes.KSIZ,
            RecordTypes.KWDA
        );
        ret.AddAlignments(
            RecordTypes.CONT,
            RecordTypes.PRPS,
            RecordTypes.NTRM,
            RecordTypes.SNAM,
            RecordTypes.QNAM,
            RecordTypes.TNAM,
            RecordTypes.ONAM
        );
        return ret;
    }

    protected override async Task<IModDisposeGetter> ImportBinaryOverlay(FilePath path)
    {
        return Fallout4ModBinaryOverlay.Fallout4ModFactory(
            new ModPath(ModKey, path));
    }

    protected override async Task<IMod> ImportBinary(FilePath path)
    {
        return Fallout4Mod.CreateFromBinary(
            new ModPath(ModKey, path.Path),
            parallel: Settings.ParallelProccessingSteps);
    }

    protected override async Task<IMod> ImportCopyIn(FilePath file)
    {
        var wrapper = Fallout4Mod.CreateFromBinaryOverlay(file.Path);
        var ret = new Fallout4Mod(ModKey);
        ret.DeepCopyIn(wrapper);
        return ret;
    }

    protected override Processor ProcessorFactory() => new Fallout4Processor(Settings.ParallelProccessingSteps);
}