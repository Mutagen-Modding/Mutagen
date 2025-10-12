using Shouldly;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class ParseRepeatingSubrecord : RecordSpanExtensionTests
{
    [Fact]
    public void ParseRepeatingSubrecordEmptyData()
    {
        byte[] b = [];
        RecordSpanExtensions.ParseRepeatingSubrecord(new ReadOnlyMemorySlice<byte>(b), GameConstants.Oblivion, RecordTypes.EDID, out var len)
            .ShouldBeEmpty();
        len.ShouldBe(0);
    }
    
    [Fact]
    public void ParseRepeatingSubrecordTypical()
    {
        var result = RecordSpanExtensions.ParseRepeatingSubrecord(
            Repeating(),
            GameConstants.Oblivion, RecordTypes.EDID, out var len);
        result.ShouldHaveCount(4);
        result.Select(x => x.RecordType).ShouldAllBe(x => x == RecordTypes.EDID);
        len.ShouldBe(0x26);
    }
    
    [Fact]
    public void ParseRepeatingSubrecordOnNonMatching()
    {
        RecordSpanExtensions.ParseRepeatingSubrecord(
                FnamStart(),
                GameConstants.Oblivion, RecordTypes.EDID, out var len)
            .ShouldBeEmpty();
        len.ShouldBe(0);
    }
}