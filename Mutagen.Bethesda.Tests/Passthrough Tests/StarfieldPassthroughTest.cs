using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace Mutagen.Bethesda.Tests;

public class StarfieldPassthroughTest : PassthroughTest
{
    public StarfieldPassthroughTest(PassthroughTestParams param, GameRelease gameRelease)
        : base(param, gameRelease)
    {
    }

    protected override async Task<IModDisposeGetter> ImportBinaryOverlay(FilePath path, StringsReadParameters stringsParams)
    {
        return StarfieldMod.Create(GameRelease.ToStarfieldRelease())
            .FromPath(
                new ModPath(ModKey, path.Path))
            .WithKnownMasters(MasterFlagsLookup.ListedOrder.ToArray())
            .Parallel(parallel: Settings.ParallelModTranslations)
            .WithStringsParameters(stringsParams)
            .ThrowIfUnknownSubrecord()
            .Construct();
    }

    protected override async Task<IMod> ImportBinary(FilePath path, StringsReadParameters stringsParams)
    {
        return StarfieldMod.Create(GameRelease.ToStarfieldRelease())
            .FromPath(
                new ModPath(ModKey, path.Path))
            .WithDefaultLoadOrder()
            .Parallel(parallel: Settings.ParallelModTranslations)
            .WithStringsParameters(stringsParams)
            .ThrowIfUnknownSubrecord()
            .Mutable()
            .Construct();
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

    protected override Processor ProcessorFactory() => new StarfieldProcessor(WorkDropoff, MasterFlagsLookup);
    
    public override AlignmentRules GetAlignmentRules()
    {
        var ret = new AlignmentRules();

        ret.SetTopLevelGroupOrder(
            RecordTypes.GMST, RecordTypes.KYWD, RecordTypes.FFKW, RecordTypes.LCRT,
            RecordTypes.AACT, RecordTypes.TRNS, RecordTypes.TXST, RecordTypes.GLOB,
            RecordTypes.DMGT, RecordTypes.CLAS, RecordTypes.FACT, RecordTypes.AFFE,
            RecordTypes.HDPT, RecordTypes.RACE, RecordTypes.SOUN, RecordTypes.SECH,
            RecordTypes.ASPC, RecordTypes.AOPF, RecordTypes.MGEF, RecordTypes.LTEX,
            RecordTypes.PDCL, RecordTypes.ENCH, RecordTypes.SPEL, RecordTypes.ACTI,
            RecordTypes.CURV, RecordTypes.CUR3, RecordTypes.ARMO, RecordTypes.BOOK,
            RecordTypes.CONT, RecordTypes.DOOR, RecordTypes.LIGH, RecordTypes.MISC,
            RecordTypes.STAT, RecordTypes.SCOL, RecordTypes.PKIN, RecordTypes.MSTT,
            RecordTypes.GRAS, RecordTypes.FLOR, RecordTypes.FURN, RecordTypes.WEAP,
            RecordTypes.AMMO, RecordTypes.NPC_, RecordTypes.LVLN, RecordTypes.LVLP,
            RecordTypes.KEYM, RecordTypes.ALCH, RecordTypes.IDLM, RecordTypes.BMMO,
            RecordTypes.NOTE, RecordTypes.PROJ, RecordTypes.HAZD, RecordTypes.BNDS,
            RecordTypes.TERM, RecordTypes.LVLI, RecordTypes.GBFT, RecordTypes.GBFM,
            RecordTypes.LVLB, RecordTypes.WTHR, RecordTypes.WTHS, RecordTypes.CLMT,
            RecordTypes.SPGD, RecordTypes.REGN, RecordTypes.NAVI, RecordTypes.CELL,
            RecordTypes.WRLD, RecordTypes.QUST, RecordTypes.IDLE, RecordTypes.PACK,
            RecordTypes.CSTY, RecordTypes.LSCR, RecordTypes.ANIO, RecordTypes.WATR,
            RecordTypes.EFSH, RecordTypes.EXPL, RecordTypes.DEBR, RecordTypes.IMGS,
            RecordTypes.IMAD, RecordTypes.FLST, RecordTypes.PERK, RecordTypes.BPTD,
            RecordTypes.ADDN, RecordTypes.AVIF, RecordTypes.CAMS, RecordTypes.CPTH,
            RecordTypes.VTYP, RecordTypes.MATT, RecordTypes.IPCT, RecordTypes.IPDS,
            RecordTypes.ARMA, RecordTypes.LCTN, RecordTypes.MESG, RecordTypes.DOBJ,
            RecordTypes.DFOB, RecordTypes.LGTM, RecordTypes.MUSC, RecordTypes.FSTP,
            RecordTypes.FSTS, RecordTypes.SMBN, RecordTypes.SMQN, RecordTypes.SMEN,
            RecordTypes.MUST, RecordTypes.EQUP, RecordTypes.OTFT, RecordTypes.ARTO,
            RecordTypes.MOVT, RecordTypes.COLL, RecordTypes.CLFM, RecordTypes.REVB,
            RecordTypes.RFGP, RecordTypes.AMDL, RecordTypes.AAMD, RecordTypes.MAAM,
            RecordTypes.LAYR, RecordTypes.COBJ, RecordTypes.OMOD, RecordTypes.ZOOM,
            RecordTypes.INNR, RecordTypes.KSSM, RecordTypes.AORU, RecordTypes.SCCO,
            RecordTypes.STAG, RecordTypes.IRES, RecordTypes.BIOM, RecordTypes.NOCM,
            RecordTypes.LENS, RecordTypes.OVIS, RecordTypes.STND, RecordTypes.STMP,
            RecordTypes.GCVR, RecordTypes.MRPH, RecordTypes.TRAV, RecordTypes.RSGD,
            RecordTypes.OSWP, RecordTypes.ATMO, RecordTypes.LVSC, RecordTypes.SPCH,
            RecordTypes.AAPD, RecordTypes.VOLI, RecordTypes.SFBK, RecordTypes.SFPC,
            RecordTypes.SFPT, RecordTypes.SFTR, RecordTypes.PCMT, RecordTypes.BMOD,
            RecordTypes.STBH, RecordTypes.PNDT, RecordTypes.CNDF, RecordTypes.PCBN,
            RecordTypes.PCCN, RecordTypes.STDT, RecordTypes.WWED, RecordTypes.RSPJ,
            RecordTypes.AOPS, RecordTypes.AMBS, RecordTypes.WBAR, RecordTypes.PTST,
            RecordTypes.LMSW, RecordTypes.FORC, RecordTypes.TMLM, RecordTypes.EFSQ,
            RecordTypes.SDLT, RecordTypes.MTPT, RecordTypes.CLDF, RecordTypes.FOGV,
            RecordTypes.WKMF, RecordTypes.LGDI, RecordTypes.PSDC, RecordTypes.SUNP,
            RecordTypes.PMFT, RecordTypes.GPOF, RecordTypes.GPOG, RecordTypes.TODD,
            RecordTypes.AVMD, RecordTypes.CHAL, RecordTypes.FXPD, RecordTypes.PERS,
            RecordTypes.GWED);

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
            RecordTypes.XMSP,
            RecordTypes.XPWR,
            RecordTypes.XLTW,
            RecordTypes.XTRV,
            RecordTypes.XVL2,
            RecordTypes.XSAD,
            RecordTypes.XLCM,
            RecordTypes.XACT,
            RecordTypes.XPRM,
            AlignmentRepeatedRule.Basic(RecordTypes.XCZR, RecordTypes.XCZA),
            RecordTypes.XVOI,
            RecordTypes.XDTS,
            RecordTypes.XDTF,
            RecordTypes.XEMI,
            RecordTypes.XRDS,
            RecordTypes.XLIG,
            RecordTypes.XLBD,
            RecordTypes.XALD,
            AlignmentRepeatedRule.Basic(RecordTypes.XCZR, RecordTypes.XCZA),
            RecordTypes.XPRD,
            RecordTypes.XPPA,
            RecordTypes.INAM,
            RecordTypes.PDTO,
            RecordTypes.TNAM,
            RecordTypes.XRGD,
            RecordTypes.XTEL,
            RecordTypes.XTNM,
            RecordTypes.XRFG,
            AlignmentRepeatedRule.Basic(RecordTypes.XLRT),
            AlignmentRepeatedRule.Basic(RecordTypes.XLMS),
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
            RecordTypes.BOLV,
            RecordTypes.XWCN,
            RecordTypes.XWCU,
            RecordTypes.XLRL,
            RecordTypes.XTRI,
            RecordTypes.XLRD,
            AlignmentRepeatedRule.Basic(RecordTypes.XLKR),
            RecordTypes.XLKT,
            RecordTypes.XLIB,
            RecordTypes.XSL1,
            RecordTypes.XEZN,
            RecordTypes.XGDS,
            RecordTypes.XLOC,
            RecordTypes.XPPS,
            RecordTypes.XEED,
            RecordTypes.XHTW,
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