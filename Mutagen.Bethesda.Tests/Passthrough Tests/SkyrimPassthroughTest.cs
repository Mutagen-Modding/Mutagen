using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Noggog;
using static Mutagen.Bethesda.Tests.ModRecordAligner;

namespace Mutagen.Bethesda.Tests
{
    public class SkyrimPassthroughTest : PassthroughTest
    {
        public override GameMode GameMode { get; }

        public SkyrimPassthroughTest(TestingSettings settings, TargetGroup group, Target target, GameMode mode)
            : base(settings, group, target)
        {
            GameMode = mode;
        }

        public override ModRecordAligner.AlignmentRules GetAlignmentRules()
        {
            var ret = new ModRecordAligner.AlignmentRules();
            ret.AddAlignments(
                RecordTypes.CELL,
                new RecordType("EDID"),
                new RecordType("FULL"),
                new RecordType("DATA"),
                new RecordType("XCLC"),
                new RecordType("XCLL"),
                new RecordType("TVDT"),
                new RecordType("MHDT"),
                new RecordType("LTMP"),
                new RecordType("LNAM"),
                new RecordType("XCLW"),
                new RecordType("XNAM"),
                new RecordType("XCLR"),
                new RecordType("XLCN"),
                new RecordType("XWCN"),
                new RecordType("XWCS"),
                new RecordType("XWCU"),
                new RecordType("XCWT"),
                new RecordType("XOWN"),
                new RecordType("XRNK"),
                new RecordType("XILL"),
                new RecordType("XWEM"),
                new RecordType("XCCM"),
                new RecordType("XCAS"),
                new RecordType("XEZN"),
                new RecordType("XCMO"),
                new RecordType("XCIM")
            );
            ret.AddAlignments(
                RecordTypes.REFR,
                new RecordType("EDID"),
                new RecordType("VMAD"),
                new RecordType("NAME"),
                new RecordType("XMBO"),
                new RecordType("XPRM"),
                new RecordType("XORD"),
                new RecordType("XOCP"),
                new RecordType("XPOD"),
                new RecordType("XPTL"),
                new RecordType("XRMR"),
                new RecordType("LNAM"),
                new RecordType("INAM"),
                new AlignmentRepeatedRule(new RecordType("XLRM")),
                new RecordType("XMBP"),
                new RecordType("XRGD"),
                new RecordType("XRGB"),
                new RecordType("XRDS"),
                new AlignmentRepeatedRule(new RecordType("XPWR")),
                new AlignmentRepeatedRule(new RecordType("XLTW")),
                new RecordType("XEMI"),
                new RecordType("XLIG"),
                new RecordType("XALP"),
                new RecordType("XTEL"),
                new RecordType("XTNM"),
                new RecordType("XMBR"),
                new RecordType("XWCN"),
                new RecordType("XWCS"),
                new RecordType("XWCU"),
                new RecordType("XCVL"),
                new RecordType("XCZR"),
                new RecordType("XCZA"),
                new RecordType("XCZC"),
                new RecordType("XSCL"),
                new RecordType("XSPC"),
                new RecordType("XAPD"),
                new AlignmentRepeatedRule(new RecordType("XAPR")),
                new RecordType("XLIB"),
                new RecordType("XLCM"),
                new RecordType("XLCN"),
                new RecordType("XTRI"),
                new RecordType("XLOC"),
                new RecordType("XEZN"),
                new RecordType("XNDP"),
                new AlignmentRepeatedRule(new RecordType("XLRT")),
                new RecordType("XIS2"),
                new RecordType("XOWN"),
                new RecordType("XRNK"),
                new RecordType("XCNT"),
                new RecordType("XCHG"),
                new RecordType("XLRL"),
                new RecordType("XESP"),
                new AlignmentRepeatedRule(new RecordType("XLKR")),
                new AlignmentSubRule(
                    new RecordType("XPRD"),
                    new RecordType("XPPA"),
                    new RecordType("INAM"),
                    new RecordType("SCHR"),
                    new RecordType("SCTX")),
                new AlignmentRepeatedRule(new RecordType("PDTO")),
                new RecordType("XACT"),
                new RecordType("XHTW"),
                new RecordType("XFVC"),
                new RecordType("ONAM"),
                new RecordType("XMRK"),
                new RecordType("FNAM"),
                new RecordType("FULL"),
                new RecordType("TNAM"),
                new RecordType("XATR"),
                new RecordType("XLOD"),
                new RecordType("DATA")
            );
            ret.AddAlignments(
                RecordTypes.ACHR,
                new RecordType("EDID"),
                new RecordType("VMAD"),
                new RecordType("NAME"),
                new RecordType("XEZN"),
                new RecordType("XRGD"),
                new RecordType("XRGB"),
                new AlignmentSubRule(
                    new RecordType("XPRD"),
                    new RecordType("XPPA"),
                    new RecordType("INAM"),
                    new RecordType("SCHR"),
                    new RecordType("SCTX")),
                new AlignmentRepeatedRule(new RecordType("PDTO")),
                new RecordType("XLCM"),
                new RecordType("XMRC"),
                new RecordType("XCNT"),
                new RecordType("XRDS"),
                new RecordType("XHLP"),
                new AlignmentRepeatedRule(new RecordType("XLKR")),
                new RecordType("XAPD"),
                new AlignmentRepeatedRule(new RecordType("XAPR")),
                new RecordType("XCLP"),
                new RecordType("XLCN"),
                new RecordType("XLRL"),
                new RecordType("XIS2"),
                new AlignmentRepeatedRule(new RecordType("XLRT")),
                new RecordType("XHTW"),
                new RecordType("XHOR"),
                new RecordType("XFVC"),
                new RecordType("XESP"),
                new RecordType("XOWN"),
                new RecordType("XRNK"),
                new RecordType("XEMI"),
                new RecordType("XMBR"),
                new RecordType("XIBS"),
                new RecordType("XSCL"),
                new RecordType("DATA"));
            AlignmentRule[] trapRules = new AlignmentRule[]
            {
                new RecordType("EDID"),
                new RecordType("VMAD"),
                new RecordType("NAME"),
                new RecordType("XEZN"),
                new RecordType("XOWN"),
                new RecordType("XRNK"),
                new RecordType("XHTW"),
                new RecordType("XFVC"),
                new RecordType("XPWR"),
                new RecordType("XLKR"),
                new RecordType("XAPD"),
                new AlignmentRepeatedRule(new RecordType("XAPR")),
                new RecordType("XESP"),
                new RecordType("XEMI"),
                new RecordType("XMBR"),
                new RecordType("XIS2"),
                new RecordType("XLRT"),
                new RecordType("XLRL"),
                new RecordType("XLOD"),
                new RecordType("XSCL"),
                new RecordType("DATA"),
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
            return ret;
        }

        protected override async Task<IModDisposeGetter> ImportBinaryOverlay(FilePath path)
        {
            return SkyrimModBinaryOverlay.SkyrimModFactory(
                this.FilePath.Path,
                this.ModKey);
        }

        protected override async Task<IMod> ImportBinary(FilePath path)
        {
            return SkyrimMod.CreateFromBinary(
                path.Path,
                this.ModKey,
                parallel: this.Settings.Parallel);
        }

        protected override async Task<IMod> ImportCopyIn(FilePath file)
        {
            var wrapper = SkyrimMod.CreateFromBinaryOverlay(file.Path);
            var ret = new SkyrimMod(this.ModKey);
            ret.DeepCopyIn(wrapper);
            return ret;
        }

        protected override Processor ProcessorFactory() => new SkyrimProcessor();
    }
}
