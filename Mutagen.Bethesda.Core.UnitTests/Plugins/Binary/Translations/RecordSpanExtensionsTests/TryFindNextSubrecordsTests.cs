using Shouldly;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class TryFindNextSubrecordsTests : RecordSpanExtensionTests
{
    [Fact]
    public void TryFindNextSubrecordsEmpty()
    {
        byte[] b = Array.Empty<byte>();
        var result = RecordSpanExtensions.TryFindNextSubrecords(new ReadOnlyMemorySlice<byte>(b),
            GameConstants.Oblivion,
            out var lenParsed, false, TestRecordTypes.EDID, TestRecordTypes.FNAM);
        lenParsed.ShouldBe(0);
        result.ShouldHaveCount(2);
        result.ShouldAllBe(x => x.Equals(default(SubrecordPinFrame?)));
    }
    
    [Fact]
    public void TryFindNextSubrecordsNoStop()
    {
        var result = RecordSpanExtensions.TryFindNextSubrecords(Repeating(), GameConstants.Oblivion,
            out var lenParsed, false, TestRecordTypes.EDID, TestRecordTypes.FNAM);
        result.ShouldHaveCount(2);
        lenParsed.ShouldBe(0x35);
        result[0].ShouldNotBeNull();
        result[0]!.Value.RecordType.ShouldBe(TestRecordTypes.EDID);
        result[0]!.Value.Location.ShouldBe(0);
        result[0]!.Value.ContentLength.ShouldBe(7);
        result[1].ShouldNotBeNull();
        result[1]!.Value.RecordType.ShouldBe(TestRecordTypes.FNAM);
        result[1]!.Value.Location.ShouldBe(0x26);
        result[1]!.Value.ContentLength.ShouldBe(9);
    }
    
    [Fact]
    public void TryFindNextSubrecordsWithStop()
    {
        var result = RecordSpanExtensions.TryFindNextSubrecords(Repeating(), GameConstants.Oblivion,
            out var lenParsed, true, TestRecordTypes.EDID, TestRecordTypes.FNAM);
        result.ShouldHaveCount(2);
        lenParsed.ShouldBe(0xD);
        result[0].ShouldNotBeNull();
        result[0]!.Value.RecordType.ShouldBe(TestRecordTypes.EDID);
        result[0]!.Value.Location.ShouldBe(0);
        result[0]!.Value.ContentLength.ShouldBe(7);
        result[1].ShouldBeNull();
    }
    
    [Fact]
    public void TryFindNextSubrecordsOverflow()
    {
        var result = RecordSpanExtensions.TryFindNextSubrecords(Overflow(), GameConstants.Oblivion,
            out var lenParsed, true, TestRecordTypes.MAST, TestRecordTypes.DATA);
        result.ShouldHaveCount(2);
        lenParsed.ShouldBe(0x1C);
        result[0].ShouldNotBeNull();
        result[0]!.Value.RecordType.ShouldBe(TestRecordTypes.MAST);
        result[0]!.Value.Location.ShouldBe(0);
        result[0]!.Value.ContentLength.ShouldBe(4);
        result[0]!.Value.AsInt32().ShouldBe(0x04030201);
        result[1].ShouldNotBeNull();
        result[1]!.Value.RecordType.ShouldBe(TestRecordTypes.DATA);
        result[1]!.Value.Location.ShouldBe(0x14);
        result[1]!.Value.ContentLength.ShouldBe(2);
        result[1]!.Value.AsInt16().ShouldEqual(0x0809);
    }
    
    [Fact]
    public void TryFindNextSubrecordsPastOverflow()
    {
        var result = RecordSpanExtensions.TryFindNextSubrecords(Overflow(), GameConstants.Oblivion,
            out var lenParsed, false, TestRecordTypes.MAST, TestRecordTypes.DATA, TestRecordTypes.EDID);
        result.ShouldHaveCount(3);
        lenParsed.ShouldBe(0x26);
        result.Select(x => x?.RecordType).ShouldEqualEnumerable(TestRecordTypes.MAST, TestRecordTypes.DATA, TestRecordTypes.EDID);
        result.Select(x => x?.Location).ShouldEqualEnumerable(0x0, 0x14, 0x1C);
        result.Select(x => x?.ContentLength).ShouldEqualEnumerable(0x4, 2, 4);
        result[0]!.Value.AsInt32().ShouldEqual(0x04030201);
        result[1]!.Value.AsInt16().ShouldEqual(0x0809);
        result[2]!.Value.AsInt32().ShouldEqual(0x44332211);
    }
}