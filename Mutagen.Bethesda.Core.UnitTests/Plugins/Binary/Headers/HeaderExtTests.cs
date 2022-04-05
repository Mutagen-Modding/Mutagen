using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Placeholders;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Headers;

public class HeaderExtTests
{
    public const int DataValue = 0x04030201;
    public const int DataPos = 0x22;

    private byte[] GetMajorBytes() => TestDataPathing.GetBytes("Plugins/Binary/Headers/MajorBytes");

    [Fact]
    public void MajorFrameTryLocateSubrecord()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        Assert.True(majorFrame.TryLocateSubrecord(RecordTypes.DATA, out var _, out var loc));
        Assert.Equal(DataPos, loc);
    }

    [Fact]
    public void MajorFrameTryLocateSubrecordFrame()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        Assert.True(majorFrame.TryLocateSubrecordFrame(RecordTypes.DATA, out var subFrame, out var loc));
        Assert.Equal(DataPos, loc);
        Assert.Equal(DataValue, subFrame.AsInt32());
    }

    [Fact]
    public void MajorFrameLocateSubrecord()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        majorFrame.LocateSubrecord(RecordTypes.DATA, out var loc);
        Assert.Equal(DataPos, loc);
    }

    [Fact]
    public void MajorFrameLocateSubrecordFrame()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        var subFrame = majorFrame.LocateSubrecordFrame(RecordTypes.DATA, out var loc);
        Assert.Equal(DataPos, loc);
        Assert.Equal(DataValue, subFrame.AsInt32());
    }

    [Fact]
    public void MajorMemoryFrameTryLocateSubrecord()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        Assert.True(majorFrame.TryLocateSubrecord(RecordTypes.DATA, out var _, out var loc));
        Assert.Equal(DataPos, loc);
    }

    [Fact]
    public void MajorMemoryFrameTryLocateSubrecordFrame()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        Assert.True(majorFrame.TryLocateSubrecordFrame(RecordTypes.DATA, out var subFrame, out var loc));
        Assert.Equal(DataPos, loc);
        Assert.Equal(DataValue, subFrame.AsInt32());
    }

    [Fact]
    public void MajorMemoryFrameLocateSubrecord()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        majorFrame.LocateSubrecord(RecordTypes.DATA, out var loc);
        Assert.Equal(DataPos, loc);
    }

    [Fact]
    public void MajorMemoryFrameLocateSubrecordFrame()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        var subFrame = majorFrame.LocateSubrecordFrame(RecordTypes.DATA, out var loc);
        Assert.Equal(DataPos, loc);
        Assert.Equal(DataValue, subFrame.AsInt32());
    }

    [Fact]
    public void EnumerateSubrecordsDirect()
    {
        byte[] b = TestDataPathing.GetBytes("Plugins/Binary/Headers/EnumerateSubrecordsDirect");
        var recs = HeaderExt.EnumerateSubrecords(b, GameConstants.Oblivion, 2)
            .ToList();
        recs.Should().HaveCount(2);
        recs[0].RecordType.Should().Be(RecordTypes.MAST);
        recs[1].RecordType.Should().Be(RecordTypes.DATA);
        recs[0].ContentLength.Should().Be(4);
        recs[1].ContentLength.Should().Be(4);
        recs[0].AsInt32().Should().Be(0x04030201);
        recs[1].AsInt32().Should().Be(0x06070809);
    }

    [Fact]
    public void EnumerateSubrecordsDirectWithOverflow()
    {
        byte[] b = TestDataPathing.GetBytes("Plugins/Binary/Headers/EnumerateSubrecordsDirectWithOverflow");
        var recs = HeaderExt.EnumerateSubrecords(b, GameConstants.Oblivion, 2, new List<RecordType>() { RecordTypes.XXXX })
            .ToList();
        recs.Should().HaveCount(2);
        recs[0].RecordType.Should().Be(RecordTypes.MAST);
        recs[1].RecordType.Should().Be(RecordTypes.DATA);
        recs[0].ContentLength.Should().Be(4);
        recs[1].ContentLength.Should().Be(2);
        recs[0].AsInt32().Should().Be(0x04030201);
        recs[1].AsInt16().Should().Be(0x0809);
    }

    [Fact]
    public void ModHeaderOverflow()
    {
        byte[] b = TestDataPathing.GetBytes("Plugins/Binary/Headers/ModHeaderOverflow");
        var modHeader = new ModHeaderFrame(GameConstants.SkyrimSE, b);
        var recs = modHeader.ToList();
        recs.Should().HaveCount(3);
        recs[0].RecordType.Should().Be(RecordTypes.MAST);
        recs[1].RecordType.Should().Be(RecordTypes.DATA);
        recs[2].RecordType.Should().Be(RecordTypes.ONAM);
        recs[0].ContentLength.Should().Be(0x0E);
        recs[1].ContentLength.Should().Be(0x08);
        recs[2].ContentLength.Should().Be(0x04);
        recs[2].AsInt32().Should().Be(0x04030201);
    }
}