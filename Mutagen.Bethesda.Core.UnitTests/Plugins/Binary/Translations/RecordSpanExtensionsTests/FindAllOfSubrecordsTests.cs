using Shouldly;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Noggog.Testing.Extensions;
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
            .ShouldBeEmpty();
    }
    
    [Fact]
    public void Scattered()
    {
        var result = RecordSpanExtensions.FindAllOfSubrecords(
            Repeating(), GameConstants.Oblivion,
            TestRecordTypes.FNAM, TestRecordTypes.XNAM);
        result.ShouldHaveCount(2);
        result[0].RecordType.ShouldBe(TestRecordTypes.FNAM);
        result[0].ContentLength.ShouldBe(9);
        result[0].Location.ShouldBe(0x26);
        result[1].RecordType.ShouldBe(TestRecordTypes.XNAM);
        result[1].ContentLength.ShouldBe(5);
        result[1].Location.ShouldBe(0x3D);
    }
    
    [Fact]
    public void Typical()
    {
        var result = RecordSpanExtensions.FindAllOfSubrecords(
            Repeating(), GameConstants.Oblivion,
            TestRecordTypes.EDID);
        result.ShouldHaveCount(5);
        result.Select(x => x.RecordType).ShouldAllBe(TestRecordTypes.EDID);
        result.Select(x => x.ContentLength).ShouldEqual(7, 2, 3, 2, 2);
        result.Select(x => x.Location).ShouldEqual(0, 0xD, 0x15, 0x1E, 0x35);
    }
    
    [Fact]
    public void WithOverflow()
    {
        var result = RecordSpanExtensions.FindAllOfSubrecords(
            Overflow(), GameConstants.Oblivion,
            TestRecordTypes.MAST, TestRecordTypes.DATA, TestRecordTypes.EDID);
        result.ShouldHaveCount(3);
        result.Select(x => x.RecordType).ShouldEqual(TestRecordTypes.MAST, TestRecordTypes.DATA, TestRecordTypes.EDID);
        result.Select(x => x.ContentLength).ShouldEqual(4, 2, 4);
        result.Select(x => x.Location).ShouldEqual(0, 0x14, 0x1C);
        result[0].AsInt32().ShouldBe(0x04030201);
        result[1].AsInt16().ShouldEqual(0x0809);
        result[2].AsInt32().ShouldBe(0x44332211);
    }
    
    [Fact]
    public void PastOverflow()
    {
        var result = RecordSpanExtensions.FindAllOfSubrecords(
            Overflow(), GameConstants.Oblivion,
            TestRecordTypes.EDID);
        result.ShouldHaveCount(1);
        result[0].RecordType.ShouldBe(TestRecordTypes.EDID);
        result[0].ContentLength.ShouldBe(4);
        result[0].Location.ShouldBe(0x1C);
        result[0].AsInt32().ShouldBe(0x44332211);
    }
}