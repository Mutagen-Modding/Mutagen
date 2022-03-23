using System.Threading.Tasks;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Processing;
using Mutagen.Bethesda.Plugins.Binary.Processing.Alignment;
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
