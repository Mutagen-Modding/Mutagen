using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Noggog.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class BinaryOverlay_Tests
    {
        #region ParseRecordLocationsByCount Collection Trigger
        [Fact]
        public void ParseRecordLocationsByCount_Single_EndOfStream()
        {
            MemoryTributary data = new MemoryTributary();
            using (MutagenWriter writer = new MutagenWriter(data, GameConstants.Oblivion))
            {
                using (HeaderExport.Subrecord(writer, RecordTypes.EDID))
                {
                    writer.Write(0);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.EDID))
                {
                    writer.Write(1);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.EDID))
                {
                    writer.Write(2);
                }
            }

            data.Position = 0;
            var triggers = new HashSet<RecordType>()
            {
                RecordTypes.EDID
            };
            var stream = new OverlayStream(data.ToArray(), new ParsingBundle(GameConstants.Oblivion, masterReferences: null!));
            var pos = BinaryOverlay.ParseRecordLocationsByCount(
                stream,
                3,
                triggers.ToGetter(),
                GameConstants.Oblivion.SubConstants,
                skipHeader: false);
            Assert.Equal(3, pos.Length);
            Assert.Equal(0, pos[0]);
            Assert.Equal(10, pos[1]);
            Assert.Equal(20, pos[2]);
            Assert.Equal(30, stream.Position);
            Assert.True(stream.Complete);
        }

        [Fact]
        public void ParseRecordLocationsByCount_Single_MoreRecords()
        {
            MemoryTributary data = new MemoryTributary();
            using (MutagenWriter writer = new MutagenWriter(data, GameConstants.Oblivion))
            {
                using (HeaderExport.Subrecord(writer, RecordTypes.EDID))
                {
                    writer.Write(0);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.EDID))
                {
                    writer.Write(1);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.EDID))
                {
                    writer.Write(2);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.MAST))
                {
                    writer.Write(2);
                }
            }

            data.Position = 0;
            var triggers = new HashSet<RecordType>()
            {
                RecordTypes.EDID
            };
            var stream = new OverlayStream(data.ToArray(), new ParsingBundle(GameConstants.Oblivion, masterReferences: null!));
            var pos = BinaryOverlay.ParseRecordLocationsByCount(
                stream,
                3,
                triggers.ToGetter(),
                GameConstants.Oblivion.SubConstants,
                skipHeader: false);
            Assert.Equal(3, pos.Length);
            Assert.Equal(0, pos[0]);
            Assert.Equal(10, pos[1]);
            Assert.Equal(20, pos[2]);
            Assert.Equal(30, stream.Position);
            Assert.False(stream.Complete);
        }

        [Fact]
        public void ParseRecordLocationsByCount_Alternating_EndOfStream()
        {
            MemoryTributary data = new MemoryTributary();
            using (MutagenWriter writer = new MutagenWriter(data, GameConstants.Oblivion))
            {
                using (HeaderExport.Subrecord(writer, RecordTypes.EDID))
                {
                    writer.Write(1);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
                {
                    writer.Write(-1);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.EDID))
                {
                    writer.Write(2);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
                {
                    writer.Write(-2);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.EDID))
                {
                    writer.Write(3);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
                {
                    writer.Write(-3);
                }
            }

            data.Position = 0;
            var triggers = new HashSet<RecordType>()
            {
                RecordTypes.EDID,
                RecordTypes.DATA,
            };
            var stream = new OverlayStream(data.ToArray(), new ParsingBundle(GameConstants.Oblivion, masterReferences: null!));
            var pos = BinaryOverlay.ParseRecordLocationsByCount(
                stream,
                3,
                triggers.ToGetter(),
                GameConstants.Oblivion.SubConstants,
                skipHeader: false);
            Assert.Equal(3, pos.Length);
            Assert.Equal(0, pos[0]);
            Assert.Equal(20, pos[1]);
            Assert.Equal(40, pos[2]);
            Assert.Equal(60, stream.Position);
            Assert.True(stream.Complete);
        }

        [Fact]
        public void ParseRecordLocationsByCount_Alternating_MoreData()
        {
            MemoryTributary data = new MemoryTributary();
            using (MutagenWriter writer = new MutagenWriter(data, GameConstants.Oblivion))
            {
                using (HeaderExport.Subrecord(writer, RecordTypes.EDID))
                {
                    writer.Write(1);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
                {
                    writer.Write(-1);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.EDID))
                {
                    writer.Write(2);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
                {
                    writer.Write(-2);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.EDID))
                {
                    writer.Write(3);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
                {
                    writer.Write(-3);
                }
                using (HeaderExport.Subrecord(writer, RecordTypes.MAST))
                {
                    writer.Write(-3);
                }
            }

            data.Position = 0;
            var triggers = new HashSet<RecordType>()
            {
                RecordTypes.EDID,
                RecordTypes.DATA,
            };
            var stream = new OverlayStream(data.ToArray(), new ParsingBundle(GameConstants.Oblivion, masterReferences: null!));
            var pos = BinaryOverlay.ParseRecordLocationsByCount(
                stream,
                3,
                triggers.ToGetter(),
                GameConstants.Oblivion.SubConstants,
                skipHeader: false);
            Assert.Equal(3, pos.Length);
            Assert.Equal(0, pos[0]);
            Assert.Equal(20, pos[1]);
            Assert.Equal(40, pos[2]);
            Assert.Equal(60, stream.Position);
            Assert.False(stream.Complete);
        }

        [Fact]
        public void DisposedIfException()
        {
            using var tmpFolder = new TempFolder(Path.Combine(Utility.TempFolderPath, nameof(DisposedIfException)));
            var modPath = Path.Combine(tmpFolder.Dir.Path, "Test.esp");
            try
            {
                File.WriteAllText(modPath, "DERP");
                var mod = SkyrimMod.CreateFromBinaryOverlay(modPath, SkyrimRelease.SkyrimLE);
            }
            catch (ArgumentException)
            {
            }
            // Assert that file is released from wrapper's internal stream
            File.Delete(modPath);
        }
        #endregion
    }
}
