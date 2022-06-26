using FluentAssertions;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class TryFindSubrecordTests : RecordSpanExtensionTests
{
    [Fact]
    public void TryFindSubrecordEmpty()
    {
        var b = Array.Empty<byte>();
        RecordSpanExtensions.TryFindSubrecord(b, GameConstants.Oblivion, SecondType).Should().BeNull();
    }
    
    [Fact]
    public void TryFindSubrecordCollectionEmpty()
    {
        var b = Array.Empty<byte>();
        RecordSpanExtensions.TryFindSubrecord(b, GameConstants.Oblivion, FirstType, SecondType).Should().BeNull();
    }

    [Fact]
    public void TryFindSubrecordTypical()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(GetTypical(), GameConstants.Oblivion, SecondType);
        rec.Should().NotBeNull();
        rec!.Value.RecordType.Should().Be(SecondType);
        rec!.Value.ContentLength.Should().Be(9);
        rec!.Value.Location.Should().Be(SecondLocation);
    }

    [Fact]
    public void TryFindSubrecordCollectionTypical()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(GetTypical(), GameConstants.Oblivion, FirstType, SecondType);
        rec.Should().NotBeNull();
        rec!.Value.RecordType.Should().Be(FirstType);
        rec!.Value.ContentLength.Should().Be(FirstLength);
        rec!.Value.Location.Should().Be(FirstLocation);
    }

    [Fact]
    public void TryFindSubrecordOverflow()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(Overflow(), GameConstants.Oblivion, RecordTypes.DATA);
        rec.Should().NotBeNull();
        rec!.Value.RecordType.Should().Be(RecordTypes.DATA);
        rec!.Value.Location.Should().Be(0x14);
        rec!.Value.ContentLength.Should().Be(2);
        rec!.Value.AsUInt16().Should().Be(0x0809);
    }

    [Fact]
    public void TryFindSubrecordPastOverflow()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(Overflow(), GameConstants.Oblivion, RecordTypes.EDID);
        rec.Should().NotBeNull();
        rec!.Value.RecordType.Should().Be(RecordTypes.EDID);
        rec!.Value.Location.Should().Be(0x1C);
        rec!.Value.ContentLength.Should().Be(4);
        rec!.Value.AsUInt32().Should().Be(0x44332211);
    }

    [Fact]
    public void TryFindSubrecordCollectionOverflow()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(Overflow(), GameConstants.Oblivion, RecordTypes.DATA, RecordTypes.ACHR);
        rec.Should().NotBeNull();
        rec!.Value.RecordType.Should().Be(RecordTypes.DATA);
        rec!.Value.Location.Should().Be(0x14);
        rec!.Value.ContentLength.Should().Be(2);
        rec!.Value.AsUInt16().Should().Be(0x0809);
    }

    [Fact]
    public void TryFindSubrecordCollectionPastOverflow()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(Overflow(), GameConstants.Oblivion, RecordTypes.EDID, RecordTypes.ACHR);
        rec.Should().NotBeNull();
        rec!.Value.RecordType.Should().Be(RecordTypes.EDID);
        rec!.Value.Location.Should().Be(0x1C);
        rec!.Value.ContentLength.Should().Be(4);
        rec!.Value.AsUInt32().Should().Be(0x44332211);
    }

    [Fact]
    public void TryFindSubrecordDuplicate()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(Repeating(), GameConstants.Oblivion, FirstType);
        rec.Should().NotBeNull();
        rec!.Value.RecordType.Should().Be(FirstType);
        rec!.Value.ContentLength.Should().Be(7);
        rec!.Value.Location.Should().Be(FirstLocation);
    }

    [Fact]
    public void TryFindSubrecordCollectionDuplicate()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(Repeating(), GameConstants.Oblivion, FirstType, SecondType);
        rec.Should().NotBeNull();
        rec!.Value.RecordType.Should().Be(FirstType);
        rec!.Value.ContentLength.Should().Be(7);
        rec!.Value.Location.Should().Be(FirstLocation);
    }
}