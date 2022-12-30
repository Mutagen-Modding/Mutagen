using FluentAssertions;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class FindAllOfSubrecordsTests : RecordSpanExtensionTests
{
    [Fact]
    public void Empty()
    {
        byte[] b = Array.Empty<byte>();
        RecordSpanExtensions.FindAllOfSubrecords(
                new ReadOnlyMemorySlice<byte>(b), GameConstants.Oblivion,
                TestRecordTypes.EDID, TestRecordTypes.FNAM)
            .Should().BeEmpty();
    }
    
    [Fact]
    public void Scattered()
    {
        var result = RecordSpanExtensions.FindAllOfSubrecords(
            Repeating(), GameConstants.Oblivion,
            TestRecordTypes.FNAM, TestRecordTypes.XNAM);
        result.Should().HaveCount(2);
        result[0].RecordType.Should().Be(TestRecordTypes.FNAM);
        result[0].ContentLength.Should().Be(9);
        result[0].Location.Should().Be(0x26);
        result[1].RecordType.Should().Be(TestRecordTypes.XNAM);
        result[1].ContentLength.Should().Be(5);
        result[1].Location.Should().Be(0x3D);
    }
    
    [Fact]
    public void Typical()
    {
        var result = RecordSpanExtensions.FindAllOfSubrecords(
            Repeating(), GameConstants.Oblivion,
            TestRecordTypes.EDID);
        result.Should().HaveCount(5);
        result.Select(x => x.RecordType).Should().AllBeEquivalentTo(TestRecordTypes.EDID);
        result.Select(x => x.ContentLength).Should().Equal(7, 2, 3, 2, 2);
        result.Select(x => x.Location).Should().Equal(0, 0xD, 0x15, 0x1E, 0x35);
    }
    
    [Fact]
    public void WithOverflow()
    {
        var result = RecordSpanExtensions.FindAllOfSubrecords(
            Overflow(), GameConstants.Oblivion,
            TestRecordTypes.MAST, TestRecordTypes.DATA, TestRecordTypes.EDID);
        result.Should().HaveCount(3);
        result.Select(x => x.RecordType).Should().Equal(TestRecordTypes.MAST, TestRecordTypes.DATA, TestRecordTypes.EDID);
        result.Select(x => x.ContentLength).Should().Equal(4, 2, 4);
        result.Select(x => x.Location).Should().Equal(0, 0x14, 0x1C);
        result[0].AsInt32().Should().Be(0x04030201);
        result[1].AsInt16().Should().Be(0x0809);
        result[2].AsInt32().Should().Be(0x44332211);
    }
    
    [Fact]
    public void PastOverflow()
    {
        var result = RecordSpanExtensions.FindAllOfSubrecords(
            Overflow(), GameConstants.Oblivion,
            TestRecordTypes.EDID);
        result.Should().HaveCount(1);
        result[0].RecordType.Should().Be(TestRecordTypes.EDID);
        result[0].ContentLength.Should().Be(4);
        result[0].Location.Should().Be(0x1C);
        result[0].AsInt32().Should().Be(0x44332211);
    }
}