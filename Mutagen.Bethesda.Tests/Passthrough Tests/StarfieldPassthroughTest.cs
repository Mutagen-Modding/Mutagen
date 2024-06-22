using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace Mutagen.Bethesda.Tests;

public class StarfieldPassthroughTest : PassthroughTest
{
    public override GameRelease GameRelease { get; }

    public StarfieldPassthroughTest(PassthroughTestParams param)
        : base(param)
    {
        GameRelease = GameRelease.Starfield;
    }

    protected override async Task<IModDisposeGetter> ImportBinaryOverlay(FilePath path, StringsReadParameters stringsParams)
    {
        return StarfieldModBinaryOverlay.StarfieldModFactory(
            new ModPath(ModKey, path),
            StarfieldRelease.Starfield,
            new BinaryReadParameters()
            {
                StringsParam = stringsParams,
                ThrowOnUnknownSubrecord = Settings.ThrowOnUnknown
            });
    }

    protected override async Task<IMod> ImportBinary(FilePath path, StringsReadParameters stringsParams)
    {
        return StarfieldMod.CreateFromBinary(
            new ModPath(ModKey, path.Path),
            StarfieldRelease.Starfield,
            new BinaryReadParameters()
            {
                Parallel = Settings.ParallelProcessingSteps,
                StringsParam = stringsParams,
                ThrowOnUnknownSubrecord = Settings.ThrowOnUnknown
            });
    }

    protected override async Task<IMod> ImportCopyIn(FilePath file)
    {
        var wrapper = StarfieldMod.CreateFromBinaryOverlay(file.Path,
            StarfieldRelease.Starfield);
        var ret = new StarfieldMod(ModKey,
            StarfieldRelease.Starfield);
        ret.DeepCopyIn(wrapper);
        return ret;
    }

    protected override Processor ProcessorFactory() => new StarfieldProcessor(Settings.ParallelProcessingSteps);
    
    public override AlignmentRules GetAlignmentRules()
    {
        var ret = new AlignmentRules();
        
        ret.SetGroupAlignment(
            (int)GroupTypeEnum.QuestChildren,
            RecordTypes.DLBR,
            RecordTypes.DIAL,
            RecordTypes.SCEN);
        ret.SetGroupAlignment(
            (int)GroupTypeEnum.CellTemporaryChildren,
            RecordTypes.NAVM);
        
        ret.StartMarkers.Add(RecordTypes.REFR, new[]
        {
            RecordTypes.NAME
        });
        
        ret.AddAlignments(
            RecordTypes.REFR,
            RecordTypes.NAME,
            RecordTypes.XVL2,
            RecordTypes.XSAD,
            RecordTypes.XLCM,
            RecordTypes.XACT,
            RecordTypes.XPRM,
            RecordTypes.XVOI,
            RecordTypes.XDTS,
            RecordTypes.XDTF,
            RecordTypes.XEMI,
            RecordTypes.XRDS,
            RecordTypes.XRDS,
            RecordTypes.XLIG,
            RecordTypes.XLBD,
            RecordTypes.XALD,
            RecordTypes.XCZC,
            RecordTypes.XCZA,
            RecordTypes.XPRD,
            RecordTypes.XPPA,
            RecordTypes.INAM,
            RecordTypes.PDTO,
            RecordTypes.TNAM,
            RecordTypes.XRGD,
            RecordTypes.XTEL,
            RecordTypes.XTNM,
            RecordTypes.XRFG,
            RecordTypes.XLRT,
            RecordTypes.XLMS,
            RecordTypes.XPCK,
            RecordTypes.XPCS,
            RecordTypes.XLCN,
            RecordTypes.XPDD,
            RecordTypes.XPDO,
            RecordTypes.XCDD,
            RecordTypes.XIS2,
            RecordTypes.XRNK,
            RecordTypes.XLGD,
            RecordTypes.XCOL,
            AlignmentRepeatedRule.Basic(RecordTypes.XPLK),
            RecordTypes.XCNT,
            RecordTypes.XFLG,
            RecordTypes.XLFD,
            AlignmentRepeatedRule.Sorted(
                new AlignmentRepeatedSubrule(RecordTypes.XMRK, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.FNAM, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.FULL, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.TNAM, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.VNAM, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.UNAM, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.VISI, Single: true)),
            RecordTypes.XLLD,
            RecordTypes.XLSM,
            RecordTypes.XLVD,
            RecordTypes.XOWN,
            AlignmentRepeatedRule.Basic(RecordTypes.XLCD),
            
            new SandwichedMarkersRule(
                RecordTypes.XWPK,
                RecordTypes.GNAM, 
                RecordTypes.HNAM, 
                RecordTypes.INAM, 
                RecordTypes.JNAM, 
                RecordTypes.LNAM, 
                RecordTypes.XGOM),
            
            RecordTypes.XBPO,
            RecordTypes.XLYR,
            RecordTypes.XLRL,
            RecordTypes.XLRD,
            AlignmentRepeatedRule.Basic(RecordTypes.XLKR),
            RecordTypes.XLKT,
            RecordTypes.XSL1,
            RecordTypes.XEZN,
            RecordTypes.XGDS,
            RecordTypes.XLOC,
            RecordTypes.XPPS,
            RecordTypes.XEED,
            RecordTypes.XHTW,
            RecordTypes.BOLV,
            RecordTypes.XBSD,
            RecordTypes.XNSE,
            RecordTypes.XATR,
            RecordTypes.XRGB,
            RecordTypes.XHLT,
            RecordTypes.TODD,
            RecordTypes.XESP,
            RecordTypes.XTV2,
            RecordTypes.XNDP,
            RecordTypes.XATP,
            RecordTypes.XSCL,
            RecordTypes.ONAM,
            RecordTypes.DATA,
            RecordTypes.MNAM
        );
        
        ret.StartMarkers.Add(RecordTypes.CELL, new[]
        {
            RecordTypes.DATA,
        });
        
        ret.AddAlignments(
            RecordTypes.CELL,
            RecordTypes.DATA,
            RecordTypes.XCLC,
            RecordTypes.XCLL,
            RecordTypes.MHDT,
            RecordTypes.LTMP,
            RecordTypes.XCLW,
            RecordTypes.XILS,
            AlignmentRepeatedRule.Basic(
                RecordTypes.XCLA,
                RecordTypes.XCLD),
            RecordTypes.XWCN,
            RecordTypes.XCCM,
            RecordTypes.XOWN,
            RecordTypes.XLCN,
            RecordTypes.XCWT,
            RecordTypes.XCWM,
            AlignmentRepeatedRule.Basic(RecordTypes.XBPS),
            RecordTypes.XWCU,
            RecordTypes.XCAS,
            RecordTypes.XCIM,
            RecordTypes.XWEM,
            RecordTypes.XCMO,
            RecordTypes.XCGD,
            RecordTypes.XCIB,
            RecordTypes.TODD,
            RecordTypes.XEZN,
            AlignmentRepeatedRule.Basic(RecordTypes.XLKR),
            RecordTypes.XLKT,
            RecordTypes.XEMP,
            RecordTypes.XTV2
        );
        
        ret.StartMarkers.Add(RecordTypes.ACHR, new[]
        {
            RecordTypes.NAME,
        });
        
        ret.AddAlignments(
            RecordTypes.ACHR,
            RecordTypes.NAME,
            RecordTypes.XLCM,
            RecordTypes.XEMI,
            RecordTypes.XRDS,
            RecordTypes.XRGD,
            RecordTypes.XRFG,
            RecordTypes.XPCS,
            RecordTypes.XLCN,
            RecordTypes.XIS2,
            RecordTypes.XRNK,
            AlignmentRepeatedRule.Basic(RecordTypes.XPLK),
            AlignmentRepeatedRule.Basic(RecordTypes.XLKR),
            RecordTypes.XEED,
            RecordTypes.XOWN,
            RecordTypes.XEZN,
            RecordTypes.XLYR,
            RecordTypes.XHTW,
            RecordTypes.XLRT,
            RecordTypes.XRGB,
            RecordTypes.XHLT,
            RecordTypes.XESP,
            RecordTypes.XATP,
            RecordTypes.XSCL,
            RecordTypes.DATA,
            RecordTypes.MNAM
        );
        
        ret.StartMarkers.Add(RecordTypes.PGRE, new[]
        {
            RecordTypes.NAME,
        });
        
        ret.AddAlignments(
            RecordTypes.PGRE,
            RecordTypes.NAME,
            RecordTypes.XEMI,
            RecordTypes.XRGD,
            RecordTypes.XRFG,
            RecordTypes.XPCS,
            RecordTypes.XIS2,
            RecordTypes.XRNK,
            AlignmentRepeatedRule.Basic(RecordTypes.XLKR),
            RecordTypes.XLKT,
            RecordTypes.XOWN,
            RecordTypes.XEZN,
            RecordTypes.XLYR,
            RecordTypes.XHTW,
            RecordTypes.XLRT,
            RecordTypes.XESP,
            RecordTypes.XATP,
            RecordTypes.XSCL,
            RecordTypes.DATA,
            RecordTypes.MNAM
        );
        
        ret.StartMarkers.Add(RecordTypes.PHZD, new[]
        {
            RecordTypes.NAME,
        });
        
        ret.AddAlignments(
            RecordTypes.PHZD,
            RecordTypes.NAME,
            RecordTypes.XEMI,
            RecordTypes.XRGD,
            RecordTypes.XRFG,
            RecordTypes.XPCS,
            RecordTypes.XIS2,
            RecordTypes.XRNK,
            AlignmentRepeatedRule.Basic(RecordTypes.XLKR),
            RecordTypes.XLKT,
            RecordTypes.XOWN,
            RecordTypes.XEZN,
            RecordTypes.XLYR,
            RecordTypes.XHTW,
            RecordTypes.XLRT,
            RecordTypes.XESP,
            RecordTypes.XATP,
            RecordTypes.XSCL,
            RecordTypes.DATA,
            RecordTypes.MNAM
        );

        ret.StartMarkers.Add(RecordTypes.RACE, new[]
        {
            RecordTypes.SAKD,
            RecordTypes.SGNM,
            RecordTypes.STKD,
            RecordTypes.SAPT,
            RecordTypes.SRAF,
        });
        ret.StopMarkers.Add(RecordTypes.RACE, new[]
        {
            RecordTypes.PTOP,
            RecordTypes.NTOP,
            RecordTypes.QSTI,
            RecordTypes.MSSS,
            RecordTypes.MSSI,
            RecordTypes.MSSA,
            RecordTypes.SNAM,
        });
        ret.AddAlignments(
            RecordTypes.RACE,
            AlignmentRepeatedRule.Sorted(
                new AlignmentRepeatedSubrule(RecordTypes.SAKD, Single: true),
                new AlignmentRepeatedSubrule(RecordTypes.SGNM, Single: false),
                new AlignmentRepeatedSubrule(RecordTypes.SAPT, Single: false),
                new AlignmentRepeatedSubrule(RecordTypes.STKD, Single: false),
                new AlignmentRepeatedSubrule(RecordTypes.SRAF, Single: true)
                {
                    Ender = true
                })
        );
        return ret;
    }
}