using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
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

    private MutagenBinaryReadStream GetModStream()
    {
        byte[] b = TestDataPathing.GetBytes(TestDataPathing.SmallOblivionMod);
        return new MutagenBinaryReadStream(
            new MemoryStream(b),
            new ParsingMeta(
                GameConstants.Get(GameRelease.Oblivion),
                ModKey.FromNameAndExtension("SmallOblivionMod.esp"),
                SeparatedMasterPackage.EmptyNull));
    }

    [Fact]
    public void ReadModHeaderFrame()
    {
        using var stream = GetModStream();
        stream.ReadModHeaderFrame();
        stream.Position.Should().Be(0x45);
    }

    [Fact]
    public void TryReadGroup()
    {
        using var stream = GetModStream();
        stream.ReadModHeaderFrame();
        List<RecordType> types = new();
        while (stream.TryReadGroup(out var g))
        {
            types.Add(g.ContainedRecordType);
        }

        types.Should().Equal(new RecordType[]
        {
            RecordTypes.RACE,
            RecordTypes.WEAP,
            RecordTypes.NPC_,
        });
    }

    [Fact]
    public void EnumerateRecords()
    {
        using var stream = GetModStream();
        stream.ReadModHeaderFrame();
        int i = 0;
        while (stream.TryReadGroup(out var group))
        {
            if (group.ContainedRecordType == RecordTypes.NPC_)
            {
                i++;
                var recs = group.ToArray();
                recs.Length.Should().Be(1);
                recs[0].Location.Should().Be(0x14);
                recs[0].IsGroup.Should().BeFalse();
                recs[0].RecordType.Should().Be(RecordTypes.NPC_);
            }
            else if (group.ContainedRecordType == RecordTypes.WEAP)
            {
                i++;
                var recs = group.ToArray();
                recs.Length.Should().Be(2);
                recs[0].Location.Should().Be(0x14);
                recs[0].IsGroup.Should().BeFalse();
                recs[0].RecordType.Should().Be(RecordTypes.WEAP);
                recs[1].Location.Should().Be(0x28);
                recs[1].IsGroup.Should().BeFalse();
                recs[1].RecordType.Should().Be(RecordTypes.WEAP);
            }
            else if (group.ContainedRecordType == RecordTypes.RACE)
            {
                i++;
                var recs = group.ToArray();
                recs.Length.Should().Be(1);
                recs[0].Location.Should().Be(0x14);
                recs[0].IsGroup.Should().BeFalse();
                recs[0].RecordType.Should().Be(RecordTypes.RACE);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        i.Should().Be(3);
    }

    [Fact]
    public void EnumerateMajorRecords()
    {
        using var stream = GetModStream();
        stream.ReadModHeaderFrame();
        int i = 0;
        while (stream.TryReadGroup(out var group))
        {
            if (group.ContainedRecordType == RecordTypes.NPC_)
            {
                i++;
                var recs = group.EnumerateMajorRecords().ToArray();
                recs.Length.Should().Be(1);
                recs[0].Location.Should().Be(0x14);
                recs[0].FormID.ID.Should().Be(0x000D62);
                recs[0].FormID.ModIndex.ID.Should().Be(1);
            }
            else if (group.ContainedRecordType == RecordTypes.WEAP)
            {
                i++;
                var recs = group.EnumerateMajorRecords().ToArray();
                recs.Length.Should().Be(2);
                recs[0].Location.Should().Be(0x14);
                recs[0].FormID.ID.Should().Be(0x123456);
                recs[0].FormID.ModIndex.ID.Should().Be(0);
                recs[1].Location.Should().Be(0x28);
                recs[1].FormID.ID.Should().Be(0x123457);
                recs[1].FormID.ModIndex.ID.Should().Be(0);
            }
            else if (group.ContainedRecordType == RecordTypes.RACE)
            {
                i++;
                var recs = group.EnumerateMajorRecords().ToArray();
                recs.Length.Should().Be(1);
                recs[0].Location.Should().Be(0x14);
                recs[0].FormID.ID.Should().Be(0x000D63);
                recs[0].FormID.ModIndex.ID.Should().Be(1);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        i.Should().Be(3);
    }
}