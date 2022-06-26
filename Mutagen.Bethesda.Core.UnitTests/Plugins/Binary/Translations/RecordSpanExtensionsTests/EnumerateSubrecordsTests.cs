using FluentAssertions;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class EnumerateSubrecordsTests : RecordSpanExtensionTests
{
    [Fact]
    public void EnumerateSubrecordsEmpty()
    {
        byte[] b = Array.Empty<byte>();
        RecordSpanExtensions.EnumerateSubrecords(new ReadOnlyMemorySlice<byte>(b), GameConstants.Oblivion)
            .Should().BeEmpty();
    }

    [Fact]
    public void EnumerateSubrecordsTypical()
    {
        var ret = RecordSpanExtensions.EnumerateSubrecords(GetTypical(), GameConstants.Oblivion).ToArray();
        ret.Length.Should().Be(2);
        ret[0].RecordType.Should().Be(FirstType);
        ret[0].Location.Should().Be(FirstLocation);
        ret[0].ContentLength.Should().Be(FirstLength);
        ret[1].RecordType.Should().Be(SecondType);
        ret[1].Location.Should().Be(SecondLocation);
        ret[1].ContentLength.Should().Be(SecondLength);
    }

    [Fact]
    public void EnumerateSubrecordsOffset()
    {
        var ret = RecordSpanExtensions.EnumerateSubrecords(Offset(), GameConstants.Oblivion, offset: OffsetAmount).ToArray();
        ret.Length.Should().Be(2);
        ret[0].RecordType.Should().Be(FirstType);
        ret[0].Location.Should().Be(FirstLocation + OffsetAmount);
        ret[0].ContentLength.Should().Be(FirstLength);
        ret[1].RecordType.Should().Be(SecondType);
        ret[1].Location.Should().Be(SecondLocation + OffsetAmount);
        ret[1].ContentLength.Should().Be(SecondLength);
    }

    [Fact]
    public void EnumerateSubrecordsWithOverflow()
    {
        var recs = RecordSpanExtensions.EnumerateSubrecords(Overflow(), GameConstants.Oblivion).ToArray();
        recs.Length.Should().Be(3);
        recs.Select(x => x.RecordType).Should().Equal(
            RecordTypes.MAST,
            RecordTypes.DATA,
            RecordTypes.EDID);
        recs.Select(x => x.ContentLength).Should().Equal(4, 2, 4);
        recs[0].AsInt32().Should().Be(0x04030201);
        recs[1].AsInt16().Should().Be(0x0809);
        recs[2].AsInt32().Should().Be(0x44332211);
    }

    [Fact]
    public void EnumerateSubrecordsDuplicate()
    {
        var ret = RecordSpanExtensions.EnumerateSubrecords(GetDuplicate(), GameConstants.Oblivion).ToArray();
        ret.Length.Should().Be(3);
        ret[0].RecordType.Should().Be(FirstType);
        ret[0].Location.Should().Be(FirstLocation);
        ret[0].ContentLength.Should().Be(FirstLength);
        ret[1].RecordType.Should().Be(SecondType);
        ret[1].Location.Should().Be(SecondLocation);
        ret[1].ContentLength.Should().Be(SecondLength);
        ret[2].RecordType.Should().Be(DuplicateType);
        ret[2].Location.Should().Be(DuplicateLocation);
        ret[2].ContentLength.Should().Be(DuplicateLength);
    }
    
    [Fact]
    public void EnumerateSubrecordsActionEmpty()
    {
        byte[] b = Array.Empty<byte>();
        List<SubrecordPinFrame> ret = new();
        RecordSpanExtensions.EnumerateSubrecords(new ReadOnlyMemorySlice<byte>(b), GameConstants.Oblivion, ret.Add);
        ret.Should().BeEmpty();
    }

    [Fact]
    public void EnumerateSubrecordsActionTypical()
    {
        List<SubrecordPinFrame> ret = new();
        RecordSpanExtensions.EnumerateSubrecords(GetTypical(), GameConstants.Oblivion, ret.Add);
        ret.Count.Should().Be(2);
        ret[0].RecordType.Should().Be(FirstType);
        ret[0].Location.Should().Be(FirstLocation);
        ret[0].ContentLength.Should().Be(FirstLength);
        ret[1].RecordType.Should().Be(SecondType);
        ret[1].Location.Should().Be(SecondLocation);
        ret[1].ContentLength.Should().Be(SecondLength);
    }

    [Fact]
    public void EnumerateSubrecordsActionOffset()
    {
        List<SubrecordPinFrame> ret = new();
        RecordSpanExtensions.EnumerateSubrecords(Offset(), GameConstants.Oblivion, ret.Add, offset: OffsetAmount);
        ret.Count.Should().Be(2);
        ret[0].RecordType.Should().Be(FirstType);
        ret[0].Location.Should().Be(FirstLocation + OffsetAmount);
        ret[0].ContentLength.Should().Be(FirstLength);
        ret[1].RecordType.Should().Be(SecondType);
        ret[1].Location.Should().Be(SecondLocation + OffsetAmount);
        ret[1].ContentLength.Should().Be(SecondLength);
    }

    [Fact]
    public void EnumerateSubrecordsActionWithOverflow()
    {
        List<SubrecordPinFrame> recs = new();
        RecordSpanExtensions.EnumerateSubrecords(Overflow(), GameConstants.Oblivion, recs.Add);
        recs.Count.Should().Be(3);
        recs.Select(x => x.RecordType).Should().Equal(
            RecordTypes.MAST,
            RecordTypes.DATA,
            RecordTypes.EDID);
        recs.Select(x => x.ContentLength).Should().Equal(4, 2, 4);
        recs[0].AsInt32().Should().Be(0x04030201);
        recs[1].AsInt16().Should().Be(0x0809);
        recs[2].AsInt32().Should().Be(0x44332211);
    }

    [Fact]
    public void EnumerateSubrecordsActionDuplicate()
    {
        List<SubrecordPinFrame> ret = new();
        RecordSpanExtensions.EnumerateSubrecords(GetDuplicate(), GameConstants.Oblivion, ret.Add);
        ret.Count.Should().Be(3);
        ret[0].RecordType.Should().Be(FirstType);
        ret[0].Location.Should().Be(FirstLocation);
        ret[0].ContentLength.Should().Be(FirstLength);
        ret[1].RecordType.Should().Be(SecondType);
        ret[1].Location.Should().Be(SecondLocation);
        ret[1].ContentLength.Should().Be(SecondLength);
        ret[2].RecordType.Should().Be(DuplicateType);
        ret[2].Location.Should().Be(DuplicateLocation);
        ret[2].ContentLength.Should().Be(DuplicateLength);
    }
}