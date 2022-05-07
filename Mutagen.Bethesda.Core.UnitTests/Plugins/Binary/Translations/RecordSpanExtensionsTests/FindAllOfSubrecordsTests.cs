using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Skyrim.Internals;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class FindAllOfSubrecordsTests : RecordSpanExtensionTests
{
    [Fact]
    public void FindAllOfSubrecordsEmpty()
    {
        byte[] b = Array.Empty<byte>();
        RecordSpanExtensions.FindAllOfSubrecords(
                new ReadOnlyMemorySlice<byte>(b), GameConstants.Oblivion,
                RecordTypes.EDID, RecordTypes.FNAM)
            .Should().BeEmpty();
    }
    
    [Fact]
    public void FindAllOfSubrecordsScattered()
    {
        byte[] b = Array.Empty<byte>();
        var result = RecordSpanExtensions.FindAllOfSubrecords(
            Repeating(), GameConstants.Oblivion,
            RecordTypes.FNAM, RecordTypes.XNAM);
        result.Should().HaveCount(2);
        result[0].RecordType.Should().Be(RecordTypes.FNAM);
        result[0].ContentLength.Should().Be(9);
        result[0].Location.Should().Be(0x26);
        result[1].RecordType.Should().Be(RecordTypes.XNAM);
        result[1].ContentLength.Should().Be(5);
        result[1].Location.Should().Be(0x3D);
    }
    
    [Fact]
    public void FindRepeating()
    {
        byte[] b = Array.Empty<byte>();
        var result = RecordSpanExtensions.FindAllOfSubrecords(
            Repeating(), GameConstants.Oblivion,
            RecordTypes.EDID);
        result.Should().HaveCount(5);
        result.Select(x => x.RecordType).Should().AllBeEquivalentTo(RecordTypes.EDID);
        result.Select(x => x.ContentLength).Should().Equal(7, 2, 3, 2, 2);
        result.Select(x => x.Location).Should().Equal(0, 0xD, 0x15, 0x1E, 0x35);
    }
}