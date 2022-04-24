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
        ret.StartMarkers.Add(RecordTypes.STAT, new[]
        {
            RecordTypes.MODL,
            RecordTypes.MODC,
            RecordTypes.MODT,
            RecordTypes.MODS
        });
        ret.StopMarkers.Add(RecordTypes.STAT, new[]
        {
            RecordTypes.PRPS,
            RecordTypes.FULL,
            RecordTypes.DNAM
        });
        ret.AddAlignments(
            RecordTypes.STAT,
            RecordTypes.MODL,
            RecordTypes.MODC,
            RecordTypes.MODT,
            RecordTypes.MODS
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
        return ret;
    }

    protected override async Task<IModDisposeGetter> ImportBinaryOverlay(FilePath path)
    {
        return Fallout4ModBinaryOverlay.Fallout4ModFactory(
            new ModPath(ModKey, FilePath.Path));
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