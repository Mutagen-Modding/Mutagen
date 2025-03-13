using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings.DI;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Placeholders;
using Xunit;
using Noggog.Testing.Extensions;

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
        recs.ShouldHaveCount(3);
        recs[0].RecordType.ShouldBe(RecordTypes.MAST);
        recs[1].RecordType.ShouldBe(RecordTypes.DATA);
        recs[2].RecordType.ShouldBe(RecordTypes.ONAM);
        recs[0].ContentLength.ShouldBe(0x0E);
        recs[1].ContentLength.ShouldBe(0x08);
        recs[2].ContentLength.ShouldBe(0x04);
        recs[2].AsInt32().ShouldBe(0x04030201);
    }

    [Fact]
    public void MastersEnumerationWithOverflow()
    {
        byte[] b = TestDataPathing.GetBytes(TestDataPathing.HeaderOverflow);
        var modHeader = new ModHeaderFrame(GameConstants.SkyrimSE, b);
        modHeader.MasterSubrecords().Select(x => x.AsString(MutagenEncoding._1252))
            .ShouldEqual("Dawnguard.esm");
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
        stream.Position.ShouldBe(0x45);
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

        types.ShouldBe(new[]
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
                recs.Length.ShouldBe(1);
                recs[0].Location.ShouldBe(0x14);
                recs[0].IsGroup.ShouldBeFalse();
                recs[0].RecordType.ShouldBe(RecordTypes.NPC_);
            }
            else if (group.ContainedRecordType == RecordTypes.WEAP)
            {
                i++;
                var recs = group.ToArray();
                recs.Length.ShouldBe(2);
                recs[0].Location.ShouldBe(0x14);
                recs[0].IsGroup.ShouldBeFalse();
                recs[0].RecordType.ShouldBe(RecordTypes.WEAP);
                recs[1].Location.ShouldBe(0x28);
                recs[1].IsGroup.ShouldBeFalse();
                recs[1].RecordType.ShouldBe(RecordTypes.WEAP);
            }
            else if (group.ContainedRecordType == RecordTypes.RACE)
            {
                i++;
                var recs = group.ToArray();
                recs.Length.ShouldBe(1);
                recs[0].Location.ShouldBe(0x14);
                recs[0].IsGroup.ShouldBeFalse();
                recs[0].RecordType.ShouldBe(RecordTypes.RACE);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        i.ShouldBe(3);
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
                recs.Length.ShouldBe(1);
                recs[0].Location.ShouldBe(0x14);
                recs[0].FormID.Raw.ShouldEqual(0x01000D62);
            }
            else if (group.ContainedRecordType == RecordTypes.WEAP)
            {
                i++;
                var recs = group.EnumerateMajorRecords().ToArray();
                recs.Length.ShouldBe(2);
                recs[0].Location.ShouldBe(0x14);
                recs[0].FormID.Raw.ShouldEqual(0x00123456);
                recs[1].Location.ShouldBe(0x28);
                recs[1].FormID.Raw.ShouldEqual(0x00123457);
            }
            else if (group.ContainedRecordType == RecordTypes.RACE)
            {
                i++;
                var recs = group.EnumerateMajorRecords().ToArray();
                recs.Length.ShouldBe(1);
                recs[0].Location.ShouldBe(0x14);
                recs[0].FormID.Raw.ShouldEqual(0x01000D63);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        i.ShouldBe(3);
    }
}