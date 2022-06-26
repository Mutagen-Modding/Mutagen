using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary;

public class BinaryOverlayTests
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
        var triggers = new RecordTriggerSpecs(RecordCollection.Factory(RecordTypes.EDID));
        var stream = new OverlayStream(data.ToArray(), new ParsingBundle(GameConstants.Oblivion, masterReferences: null!));
        var pos = PluginBinaryOverlay.ParseRecordLocationsByCount(
            stream,
            3,
            triggers,
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
        var triggers = new RecordTriggerSpecs(RecordCollection.Factory(RecordTypes.EDID));
        var stream = new OverlayStream(data.ToArray(), new ParsingBundle(GameConstants.Oblivion, masterReferences: null!));
        var pos = PluginBinaryOverlay.ParseRecordLocationsByCount(
            stream,
            3,
            triggers,
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
        var triggers = new RecordTriggerSpecs(RecordCollection.Factory(RecordTypes.EDID, RecordTypes.DATA));
        var stream = new OverlayStream(data.ToArray(), new ParsingBundle(GameConstants.Oblivion, masterReferences: null!));
        var pos = PluginBinaryOverlay.ParseRecordLocationsByCount(
            stream,
            3,
            triggers,
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
        var triggers = new RecordTriggerSpecs(RecordCollection.Factory(RecordTypes.EDID, RecordTypes.DATA));
        var stream = new OverlayStream(data.ToArray(), new ParsingBundle(GameConstants.Oblivion, masterReferences: null!));
        var pos = PluginBinaryOverlay.ParseRecordLocationsByCount(
            stream,
            3,
            triggers,
            GameConstants.Oblivion.SubConstants,
            skipHeader: false);
        Assert.Equal(3, pos.Length);
        Assert.Equal(0, pos[0]);
        Assert.Equal(20, pos[1]);
        Assert.Equal(40, pos[2]);
        Assert.Equal(60, stream.Position);
        Assert.False(stream.Complete);
    }
    #endregion
}