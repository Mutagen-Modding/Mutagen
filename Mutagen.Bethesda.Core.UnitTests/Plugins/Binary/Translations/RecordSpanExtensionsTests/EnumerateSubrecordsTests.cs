using FluentAssertions;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
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