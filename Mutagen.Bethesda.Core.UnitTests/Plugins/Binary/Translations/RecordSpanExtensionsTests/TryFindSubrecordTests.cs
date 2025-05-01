using Shouldly;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class TryFindSubrecordTests : RecordSpanExtensionTests
{
    [Fact]
    public void TryFindSubrecordEmpty()
    {
        var b = Array.Empty<byte>();
        RecordSpanExtensions.TryFindSubrecord(b, GameConstants.Oblivion, SecondType).ShouldBeNull();
    }
    
    [Fact]
    public void TryFindSubrecordCollectionEmpty()
    {
        var b = Array.Empty<byte>();
        RecordSpanExtensions.TryFindSubrecord(b, GameConstants.Oblivion, FirstType, SecondType).ShouldBeNull();
    }

    [Fact]
    public void TryFindSubrecordTypical()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(GetTypical(), GameConstants.Oblivion, SecondType);
        rec.ShouldNotBeNull();
        rec!.Value.RecordType.ShouldBe(SecondType);
        rec!.Value.ContentLength.ShouldBe(9);
        rec!.Value.Location.ShouldBe(SecondLocation);
    }

    [Fact]
    public void TryFindSubrecordCollectionTypical()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(GetTypical(), GameConstants.Oblivion, FirstType, SecondType);
        rec.ShouldNotBeNull();
        rec!.Value.RecordType.ShouldBe(FirstType);
        rec!.Value.ContentLength.ShouldBe(FirstLength);
        rec!.Value.Location.ShouldBe(FirstLocation);
    }

    [Fact]
    public void TryFindSubrecordOverflow()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(Overflow(), GameConstants.Oblivion, RecordTypes.DATA);
        rec.ShouldNotBeNull();
        rec!.Value.RecordType.ShouldBe(RecordTypes.DATA);
        rec!.Value.Location.ShouldBe(0x14);
        rec!.Value.ContentLength.ShouldBe(2);
        rec!.Value.AsUInt16().ShouldEqual(0x0809);
    }

    [Fact]
    public void TryFindSubrecordPastOverflow()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(Overflow(), GameConstants.Oblivion, RecordTypes.EDID);
        rec.ShouldNotBeNull();
        rec!.Value.RecordType.ShouldBe(RecordTypes.EDID);
        rec!.Value.Location.ShouldBe(0x1C);
        rec!.Value.ContentLength.ShouldBe(4);
        rec!.Value.AsUInt32().ShouldEqual(0x44332211);
    }

    [Fact]
    public void TryFindSubrecordCollectionOverflow()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(Overflow(), GameConstants.Oblivion, RecordTypes.DATA, RecordTypes.ACHR);
        rec.ShouldNotBeNull();
        rec!.Value.RecordType.ShouldBe(RecordTypes.DATA);
        rec!.Value.Location.ShouldBe(0x14);
        rec!.Value.ContentLength.ShouldBe(2);
        rec!.Value.AsUInt16().ShouldEqual(0x0809);
    }

    [Fact]
    public void TryFindSubrecordCollectionPastOverflow()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(Overflow(), GameConstants.Oblivion, RecordTypes.EDID, RecordTypes.ACHR);
        rec.ShouldNotBeNull();
        rec!.Value.RecordType.ShouldBe(RecordTypes.EDID);
        rec!.Value.Location.ShouldBe(0x1C);
        rec!.Value.ContentLength.ShouldBe(4);
        rec!.Value.AsUInt32().ShouldEqual(0x44332211);
    }

    [Fact]
    public void TryFindSubrecordDuplicate()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(Repeating(), GameConstants.Oblivion, FirstType);
        rec.ShouldNotBeNull();
        rec!.Value.RecordType.ShouldBe(FirstType);
        rec!.Value.ContentLength.ShouldBe(7);
        rec!.Value.Location.ShouldBe(FirstLocation);
    }

    [Fact]
    public void TryFindSubrecordCollectionDuplicate()
    {
        var rec = RecordSpanExtensions.TryFindSubrecord(Repeating(), GameConstants.Oblivion, FirstType, SecondType);
        rec.ShouldNotBeNull();
        rec!.Value.RecordType.ShouldBe(FirstType);
        rec!.Value.ContentLength.ShouldBe(7);
        rec!.Value.Location.ShouldBe(FirstLocation);
    }
}