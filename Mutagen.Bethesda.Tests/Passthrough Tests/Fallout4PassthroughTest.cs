using System.Threading.Tasks;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Tests
{
    public class Fallout4PassthroughTest : PassthroughTest
    {
        public override GameRelease GameRelease { get; }

        public Fallout4PassthroughTest(PassthroughTestParams param)
            : base(param)
        {
            GameRelease = GameRelease.Fallout4;
        }

        public override ModRecordAligner.AlignmentRules GetAlignmentRules()
        {
            var ret = new ModRecordAligner.AlignmentRules();
            // Alignment rules to come
            return ret;
        }

        protected override async Task<IModDisposeGetter> ImportBinaryOverlay(FilePath path)
        {
            return Fallout4ModBinaryOverlay.Fallout4ModFactory(
                new ModPath(this.ModKey, this.FilePath.Path));
        }

        protected override async Task<IMod> ImportBinary(FilePath path)
        {
            return Fallout4Mod.CreateFromBinary(
                new ModPath(this.ModKey, path.Path),
                parallel: this.Settings.ParallelProccessingSteps);
        }

        protected override async Task<IMod> ImportCopyIn(FilePath file)
        {
            var wrapper = Fallout4Mod.CreateFromBinaryOverlay(file.Path);
            var ret = new Fallout4Mod(this.ModKey);
            ret.DeepCopyIn(wrapper);
            return ret;
        }

        protected override Processor ProcessorFactory() => new Fallout4Processor(Settings.ParallelProccessingSteps);
    }
}
