using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Skyrim.Internals;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class FindAllOfSubrecordTests : RecordSpanExtensionTests
{
    [Fact]
    public void Empty()
    {
        byte[] b = Array.Empty<byte>();
        RecordSpanExtensions.FindAllOfSubrecord(
                new ReadOnlyMemorySlice<byte>(b), GameConstants.Oblivion,
                RecordTypes.EDID)
            .Should().BeEmpty();
    }
    
    [Fact]
    public void Typical()
    {
        var result = RecordSpanExtensions.FindAllOfSubrecord(
            Repeating(), GameConstants.Oblivion,
            RecordTypes.EDID);
        result.Should().HaveCount(5);
        result.Select(x => x.RecordType).Should().AllBeEquivalentTo(RecordTypes.EDID);
        result.Select(x => x.ContentLength).Should().Equal(7, 2, 3, 2, 2);
        result.Select(x => x.Location).Should().Equal(0, 0xD, 0x15, 0x1E, 0x35);
    }
    
    [Fact]
    public void WithOverflow()
    {
        var result = RecordSpanExtensions.FindAllOfSubrecord(
            Overflow(), GameConstants.Oblivion,
            RecordTypes.DATA);
        result.Should().HaveCount(1);
        result.Select(x => x.RecordType).Should().Equal(RecordTypes.DATA);
        result.Select(x => x.ContentLength).Should().Equal(2);
        result.Select(x => x.Location).Should().Equal(0x14);
        result[0].AsInt16().Should().Be(0x0809);
    }
    
    [Fact]
    public void PastOverflow()
    {
        var result = RecordSpanExtensions.FindAllOfSubrecord(
            Overflow(), GameConstants.Oblivion,
            RecordTypes.EDID);
        result.Should().HaveCount(1);
        result[0].RecordType.Should().Be(RecordTypes.EDID);
        result[0].ContentLength.Should().Be(4);
        result[0].Location.Should().Be(0x1C);
        result[0].AsInt32().Should().Be(0x44332211);
    }
}