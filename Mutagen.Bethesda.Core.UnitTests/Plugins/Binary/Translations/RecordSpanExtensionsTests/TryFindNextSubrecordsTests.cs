using FluentAssertions;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Skyrim.Internals;
using Noggog;
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
            out var lenParsed, false, RecordTypes.EDID, RecordTypes.FNAM);
        lenParsed.Should().Be(0);
        result.Should().HaveCount(2);
        result.Should().AllBeEquivalentTo(default(SubrecordPinFrame?));
    }
    
    [Fact]
    public void TryFindNextSubrecordsNoStop()
    {
        var result = RecordSpanExtensions.TryFindNextSubrecords(Repeating(), GameConstants.Oblivion,
            out var lenParsed, false, RecordTypes.EDID, RecordTypes.FNAM);
        result.Should().HaveCount(2);
        lenParsed.Should().Be(0x35);
        result[0].Should().NotBeNull();
        result[0]!.Value.RecordType.Should().Be(RecordTypes.EDID);
        result[0]!.Value.Location.Should().Be(0);
        result[0]!.Value.ContentLength.Should().Be(7);
        result[1].Should().NotBeNull();
        result[1]!.Value.RecordType.Should().Be(RecordTypes.FNAM);
        result[1]!.Value.Location.Should().Be(0x26);
        result[1]!.Value.ContentLength.Should().Be(9);
    }
    
    [Fact]
    public void TryFindNextSubrecordsWithStop()
    {
        var result = RecordSpanExtensions.TryFindNextSubrecords(Repeating(), GameConstants.Oblivion,
            out var lenParsed, true, RecordTypes.EDID, RecordTypes.FNAM);
        result.Should().HaveCount(2);
        lenParsed.Should().Be(0xD);
        result[0].Should().NotBeNull();
        result[0]!.Value.RecordType.Should().Be(RecordTypes.EDID);
        result[0]!.Value.Location.Should().Be(0);
        result[0]!.Value.ContentLength.Should().Be(7);
        result[1].Should().BeNull();
    }
    
    [Fact]
    public void TryFindNextSubrecordsOverflow()
    {
        var result = RecordSpanExtensions.TryFindNextSubrecords(Overflow(), GameConstants.Oblivion,
            out var lenParsed, true, RecordTypes.MAST, RecordTypes.DATA);
        result.Should().HaveCount(2);
        lenParsed.Should().Be(0x1C);
        result[0].Should().NotBeNull();
        result[0]!.Value.RecordType.Should().Be(RecordTypes.MAST);
        result[0]!.Value.Location.Should().Be(0);
        result[0]!.Value.ContentLength.Should().Be(4);
        result[0]!.Value.AsInt32().Should().Be(0x04030201);
        result[1].Should().NotBeNull();
        result[1]!.Value.RecordType.Should().Be(RecordTypes.DATA);
        result[1]!.Value.Location.Should().Be(0x14);
        result[1]!.Value.ContentLength.Should().Be(2);
        result[1]!.Value.AsInt16().Should().Be(0x0809);
    }
    
    [Fact]
    public void TryFindNextSubrecordsPastOverflow()
    {
        var result = RecordSpanExtensions.TryFindNextSubrecords(Overflow(), GameConstants.Oblivion,
            out var lenParsed, false, RecordTypes.MAST, RecordTypes.DATA, RecordTypes.EDID);
        result.Should().HaveCount(3);
        lenParsed.Should().Be(0x26);
        result.Select(x => x?.RecordType).Should().Equal(RecordTypes.MAST, RecordTypes.DATA, RecordTypes.EDID);
        result.Select(x => x?.Location).Should().Equal(0x0, 0x14, 0x1C);
        result.Select(x => x?.ContentLength).Should().Equal(0x4, 2, 4);
        result[0]!.Value.AsInt32().Should().Be(0x04030201);
        result[1]!.Value.AsInt16().Should().Be(0x0809);
        result[2]!.Value.AsInt32().Should().Be(0x44332211);
    }
}