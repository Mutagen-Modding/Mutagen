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
            stringsParams);
    }

    protected override async Task<IMod> ImportBinary(FilePath path, StringsReadParameters stringsParams)
    {
        return StarfieldMod.CreateFromBinary(
            new ModPath(ModKey, path.Path),
            StarfieldRelease.Starfield,
            parallel: Settings.ParallelProcessingSteps,
            stringsParam: stringsParams);
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
        return new AlignmentRules();
    }
}