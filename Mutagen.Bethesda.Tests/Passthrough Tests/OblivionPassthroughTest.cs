using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Mutagen.Bethesda.Tests.ModRecordAligner;

namespace Mutagen.Bethesda.Tests
{
    public class OblivionPassthroughTest : PassthroughTest
    {
        public override GameRelease GameRelease => GameRelease.Oblivion;

        protected override Processor ProcessorFactory() => new OblivionProcessor(Settings.ParallelProccessingSteps);

        public OblivionPassthroughTest(PassthroughTestParams param)
            : base(param)
        {
        }

        protected override async Task<IModDisposeGetter> ImportBinaryOverlay(FilePath path)
        {
            return OblivionModBinaryOverlay.OblivionModFactory(new ModPath(ModKey, FilePath.Path));
        }

        protected override async Task<IMod> ImportBinary(FilePath path)
        {
            return OblivionMod.CreateFromBinary(
                new ModPath(ModKey, path.Path),
                parallel: this.Settings.ParallelProccessingSteps);
        }

        protected override async Task<IMod> ImportCopyIn(FilePath file)
        {
            var wrapper = OblivionMod.CreateFromBinaryOverlay(file.Path);
            var ret = new OblivionMod(this.ModKey);
            ret.DeepCopyIn(wrapper);
            return ret;
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
                new RecordType("XCLR"),
                new RecordType("XCMT"),
                new RecordType("XCLW"),
                new RecordType("XCCM"),
                new RecordType("XCWT"),
                new RecordType("XOWN"),
                new RecordType("XRNK"),
                new RecordType("XGLB"));
            ret.AddAlignments(
                RecordTypes.WRLD,
                new RecordType("EDID"),
                new RecordType("FULL"),
                new RecordType("WNAM"),
                new RecordType("CNAM"),
                new RecordType("NAM2"),
                new RecordType("ICON"),
                new RecordType("MNAM"),
                new RecordType("DATA"),
                new RecordType("NAM0"),
                new RecordType("NAM9"),
                new RecordType("SNAM"),
                new RecordType("XXXX"));
            ret.StopMarkers[RecordTypes.WRLD] = new List<RecordType>()
            {
                new RecordType("OFST"),
            };
            ret.AddAlignments(
                RecordTypes.REFR,
                new ModRecordAligner.AlignmentStraightRecord("EDID"),
                new ModRecordAligner.AlignmentStraightRecord("NAME"),
                new ModRecordAligner.AlignmentStraightRecord("XPCI"),
                new ModRecordAligner.AlignmentStraightRecord("FULL"),
                new ModRecordAligner.AlignmentStraightRecord("XTEL"),
                new ModRecordAligner.AlignmentStraightRecord("XLOC"),
                new ModRecordAligner.AlignmentStraightRecord("XOWN"),
                new ModRecordAligner.AlignmentStraightRecord("XRNK"),
                new ModRecordAligner.AlignmentStraightRecord("XGLB"),
                new ModRecordAligner.AlignmentStraightRecord("XESP"),
                new ModRecordAligner.AlignmentStraightRecord("XTRG"),
                new ModRecordAligner.AlignmentStraightRecord("XSED"),
                new ModRecordAligner.AlignmentStraightRecord("XLOD"),
                new ModRecordAligner.AlignmentStraightRecord("XCHG"),
                new ModRecordAligner.AlignmentStraightRecord("XHLT"),
                new ModRecordAligner.AlignmentStraightRecord("XLCM"),
                new ModRecordAligner.AlignmentStraightRecord("XRTM"),
                new ModRecordAligner.AlignmentStraightRecord("XACT"),
                new ModRecordAligner.AlignmentStraightRecord("XCNT"),
                new ModRecordAligner.AlignmentSubRule(
                    new RecordType("XMRK"),
                    new RecordType("FNAM"),
                    new RecordType("FULL"),
                    new RecordType("TNAM")),
                new ModRecordAligner.AlignmentStraightRecord("ONAM"),
                new ModRecordAligner.AlignmentStraightRecord("XRGD"),
                new ModRecordAligner.AlignmentStraightRecord("XSCL"),
                new ModRecordAligner.AlignmentStraightRecord("XSOL"),
                new ModRecordAligner.AlignmentStraightRecord("DATA"));
            ret.AddAlignments(
                RecordTypes.ACRE,
                new RecordType("EDID"),
                new RecordType("NAME"),
                new RecordType("XOWN"),
                new RecordType("XRNK"),
                new RecordType("XGLB"),
                new RecordType("XESP"),
                new RecordType("XRGD"),
                new RecordType("XSCL"),
                new RecordType("DATA"));
            ret.AddAlignments(
                RecordTypes.ACHR,
                new RecordType("EDID"),
                new RecordType("NAME"),
                new RecordType("XPCI"),
                new RecordType("FULL"),
                new RecordType("XLOD"),
                new RecordType("XESP"),
                new RecordType("XMRC"),
                new RecordType("XHRS"),
                new RecordType("XRGD"),
                new RecordType("XSCL"),
                new RecordType("DATA"));
            ret.AddAlignments(
                RecordTypes.CREA,
                new RecordType("EDID"),
                new RecordType("FULL"),
                new RecordType("MODL"),
                new RecordType("MODB"),
                new RecordType("MODT"),
                new AlignmentRepeatedRule(new RecordType("CNTO")),
                new AlignmentRepeatedRule(new RecordType("SPLO")),
                new AlignmentRepeatedRule(new RecordType("NIFZ")),
                new RecordType("NIFT"),
                new RecordType("ACBS"),
                new AlignmentRepeatedRule(new RecordType("SNAM")),
                new RecordType("INAM"),
                new RecordType("SCRI"),
                new RecordType("AIDT"),
                new AlignmentRepeatedRule(new RecordType("PKID")),
                new AlignmentRepeatedRule(new RecordType("KFFZ")),
                new RecordType("DATA"),
                new RecordType("RNAM"),
                new RecordType("ZNAM"),
                new RecordType("TNAM"),
                new RecordType("BNAM"),
                new RecordType("WNAM"),
                new RecordType("NAM0"),
                new RecordType("NAM1"));
            ret.StopMarkers[RecordTypes.CREA] = new List<RecordType>()
            {
                new RecordType("DATA"),
            };
            ret.SetGroupAlignment(
                (int)GroupTypeEnum.CellTemporaryChildren,
                new RecordType("LAND"),
                new RecordType("PGRD"));
            return ret;
        }
    }
}
