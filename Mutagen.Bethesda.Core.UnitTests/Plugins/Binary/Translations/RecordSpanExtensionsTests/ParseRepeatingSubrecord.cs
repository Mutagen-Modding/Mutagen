using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class ParseRepeatingSubrecord : RecordSpanExtensionTests
{
    [Fact]
    public void ParseRepeatingSubrecordEmptyData()
    {
        byte[] b = Array.Empty<byte>();
        RecordSpanExtensions.ParseRepeatingSubrecord(new ReadOnlyMemorySlice<byte>(b), GameConstants.Oblivion, RecordTypes.EDID, out var len)
            .Should().BeEmpty();
        len.Should().Be(0);
    }
    
    [Fact]
    public void ParseRepeatingSubrecordTypical()
    {
        var result = RecordSpanExtensions.ParseRepeatingSubrecord(
            Repeating(),
            GameConstants.Oblivion, RecordTypes.EDID, out var len);
        result.Should().HaveCount(4);
        result.Select(x => x.RecordType).Should().AllBeEquivalentTo(RecordTypes.EDID);
        len.Should().Be(0x26);
    }
    
    [Fact]
    public void ParseRepeatingSubrecordOnNonMatching()
    {
        var result = RecordSpanExtensions.ParseRepeatingSubrecord(
                FnamStart(),
                GameConstants.Oblivion, RecordTypes.EDID, out var len)
            .Should().BeEmpty();
        len.Should().Be(0);
    }
}