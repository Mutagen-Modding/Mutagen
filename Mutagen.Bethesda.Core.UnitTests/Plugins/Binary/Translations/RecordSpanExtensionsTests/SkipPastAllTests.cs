using Shouldly;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class SkipPastAllTests : RecordSpanExtensionTests
{
    [Fact]
    public void SkipPastAllEmpty()
    {
        byte[] b = [];
        RecordSpanExtensions.SkipPastAll(
                new ReadOnlyMemorySlice<byte>(b), GameConstants.Oblivion,
                TestRecordTypes.EDID, out var numPassed)
            .ShouldBe(0);
        numPassed.ShouldBe(0);
    }
    
    [Fact]
    public void SkipSome()
    {
        RecordSpanExtensions.SkipPastAll(
                Repeating(), GameConstants.Oblivion,
                TestRecordTypes.EDID, out var numPassed)
            .ShouldBe(0x26);
        numPassed.ShouldBe(4);
    }
    
    [Fact]
    public void SkipNone()
    {
        RecordSpanExtensions.SkipPastAll(
                Repeating(), GameConstants.Oblivion,
                TestRecordTypes.FNAM, out var numPassed)
            .ShouldBe(0);
        numPassed.ShouldBe(0);
    }
}