using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.Tests
{
    public class OblivionPassthroughTest : PassthroughTest
    {
        public override GameMode GameMode => GameMode.Oblivion;

        protected override Processor ProcessorFactory() => new OblivionProcessor();

        public OblivionPassthroughTest(TestingSettings settings, Target target)
            : base(settings, target)
        {
        }

        protected override async Task<IModDisposeGetter> ImportBinaryOverlay(FilePath path)
        {
            return OblivionModBinaryOverlay.OblivionModFactory(
                this.FilePath.Path,
                this.ModKey);
        }

        protected override async Task<IMod> ImportBinary(FilePath path)
        {
            return OblivionMod.CreateFromBinary(
                path.Path,
                this.ModKey,
                parallel: this.Settings.Parallel);
        }

        protected override async Task<IMod> ImportCopyIn(FilePath file)
        {
            var wrapper = OblivionMod.CreateFromBinaryOverlay(file.Path);
            var ret = new OblivionMod(this.ModKey);
            ret.DeepCopyIn(wrapper);
            return ret;
        }

        //protected override async Task<IMod> ImportXmlFolder(DirectoryPath dir)
        //{
        //    return await OblivionMod.CreateFromXmlFolder(dir, this.ModKey);
        //}

        //protected override Task WriteXmlFolder(IModGetter mod, DirectoryPath dir)
        //{
        //    return ((OblivionMod)mod).WriteToXmlFolder(dir);
        //}

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
            ret.SetGroupAlignment(
                GroupTypeEnum.CellTemporaryChildren,
                new RecordType("LAND"),
                new RecordType("PGRD"));
            return ret;
        }
    }
}
