using FluentAssertions;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings.DI;
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
        Assert.True(majorFrame.TryFindSubrecordHeader(RecordTypes.DATA, out var rec));
        Assert.Equal(DataPos, rec.Location);
    }

    [Fact]
    public void MajorFrameTryLocateSubrecordFrame()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        Assert.True(majorFrame.TryFindSubrecord(RecordTypes.DATA, out var subFrame));
        Assert.Equal(DataPos, subFrame.Location);
        Assert.Equal(DataValue, subFrame.AsInt32());
    }

    [Fact]
    public void MajorFrameLocateSubrecord()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        var rec = majorFrame.FindSubrecordHeader(RecordTypes.DATA);
        Assert.Equal(DataPos, rec.Location);
    }

    [Fact]
    public void MajorFrameLocateSubrecordFrame()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        var subFrame = majorFrame.FindSubrecord(RecordTypes.DATA);
        Assert.Equal(DataPos, subFrame.Location);
        Assert.Equal(DataValue, subFrame.AsInt32());
    }

    [Fact]
    public void MajorMemoryFrameTryLocateSubrecord()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        Assert.True(majorFrame.TryFindSubrecordHeader(RecordTypes.DATA, out var rec));
        Assert.Equal(DataPos, rec.Location);
    }

    [Fact]
    public void MajorMemoryFrameTryLocateSubrecordFrame()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        Assert.True(majorFrame.TryFindSubrecord(RecordTypes.DATA, out var subFrame));
        Assert.Equal(DataPos, subFrame.Location);
        Assert.Equal(DataValue, subFrame.AsInt32());
    }

    [Fact]
    public void MajorMemoryFrameLocateSubrecord()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        var rec = majorFrame.FindSubrecordHeader(RecordTypes.DATA);
        Assert.Equal(DataPos, rec.Location);
    }

    [Fact]
    public void MajorMemoryFrameLocateSubrecordFrame()
    {
        var majorFrame = new MajorRecordFrame(GameConstants.Oblivion, GetMajorBytes());
        var subFrame = majorFrame.FindSubrecord(RecordTypes.DATA);
        Assert.Equal(DataPos, subFrame.Location);
        Assert.Equal(DataValue, subFrame.AsInt32());
    }

    [Fact]
    public void ModHeaderFrameOverflow()
    {
        byte[] b = TestDataPathing.GetBytes(TestDataPathing.HeaderOverflow);
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

    [Fact]
    public void MastersEnumerationWithOverflow()
    {
        byte[] b = TestDataPathing.GetBytes(TestDataPathing.HeaderOverflow);
        var modHeader = new ModHeaderFrame(GameConstants.SkyrimSE, b);
        modHeader.Masters().Select(x => x.AsString(MutagenEncoding._1252))
            .Should().Equal("Dawnguard.esm");
    }
}