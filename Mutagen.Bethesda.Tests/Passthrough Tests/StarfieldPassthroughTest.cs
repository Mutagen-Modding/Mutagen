using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;
using Mutagen.Bethesda.Plugins.Records;
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

    protected override async Task<IModDisposeGetter> ImportBinaryOverlay(FilePath path)
    {
        return StarfieldModBinaryOverlay.StarfieldModFactory(
            new ModPath(ModKey, path));
    }

    protected override async Task<IMod> ImportBinary(FilePath path)
    {
        return StarfieldMod.CreateFromBinary(
            new ModPath(ModKey, path.Path),
            parallel: Settings.ParallelProcessingSteps);
    }

    protected override async Task<IMod> ImportCopyIn(FilePath file)
    {
        var wrapper = StarfieldMod.CreateFromBinaryOverlay(file.Path);
        var ret = new StarfieldMod(ModKey);
        ret.DeepCopyIn(wrapper);
        return ret;
    }

    protected override Processor ProcessorFactory() => new StarfieldProcessor(Settings.ParallelProcessingSteps);
    
    public override AlignmentRules GetAlignmentRules()
    {
        throw new NotImplementedException();
    }
}