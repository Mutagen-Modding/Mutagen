using Shouldly;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class FindAllOfSubrecordTests : RecordSpanExtensionTests
{
    [Fact]
    public void Empty()
    {
        byte[] b = [];
        RecordSpanExtensions.FindAllOfSubrecord(
                new ReadOnlyMemorySlice<byte>(b), GameConstants.Oblivion,
                TestRecordTypes.EDID)
            .ShouldBeEmpty();
    }
    
    [Fact]
    public void Typical()
    {
        var result = RecordSpanExtensions.FindAllOfSubrecord(
            Repeating(), GameConstants.Oblivion,
            TestRecordTypes.EDID);
        result.Count.ShouldBe(5);
        result.Select(x => x.RecordType).ShouldAllBe(t => t.Equals(TestRecordTypes.EDID));
        result.Select(x => x.ContentLength).ShouldEqualEnumerable(7, 2, 3, 2, 2);
        result.Select(x => x.Location).ShouldEqualEnumerable(0, 0xD, 0x15, 0x1E, 0x35);
    }
    
    [Fact]
    public void WithOverflow()
    {
        var result = RecordSpanExtensions.FindAllOfSubrecord(
            Overflow(), GameConstants.Oblivion,
            TestRecordTypes.DATA);
        result.Count.ShouldBe(1);
        result.Select(x => x.RecordType).ShouldEqualEnumerable(TestRecordTypes.DATA);
        result.Select(x => x.ContentLength).ShouldEqualEnumerable(2);
        result.Select(x => x.Location).ShouldEqualEnumerable(0x14);
        result[0].AsInt16().ShouldEqual(0x0809);
    }
    
    [Fact]
    public void PastOverflow()
    {
        var result = RecordSpanExtensions.FindAllOfSubrecord(
            Overflow(), GameConstants.Oblivion,
            TestRecordTypes.EDID);
        result.Count.ShouldBe(1);
        result[0].RecordType.ShouldBe(TestRecordTypes.EDID);
        result[0].ContentLength.ShouldBe(4);
        result[0].Location.ShouldBe(0x1C);
        result[0].AsInt32().ShouldBe(0x44332211);
    }
}