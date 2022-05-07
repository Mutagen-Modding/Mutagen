using FluentAssertions;
using Mutagen.Bethesda.Fallout4.Internals;
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
        byte[] b = Array.Empty<byte>();
        RecordSpanExtensions.SkipPastAll(
                new ReadOnlyMemorySlice<byte>(b), GameConstants.Oblivion,
                RecordTypes.EDID, out var numPassed)
            .Should().Be(0);
        numPassed.Should().Be(0);
    }
    
    [Fact]
    public void SkipSome()
    {
        RecordSpanExtensions.SkipPastAll(
                Repeating(), GameConstants.Oblivion,
                RecordTypes.EDID, out var numPassed)
            .Should().Be(0x26);
        numPassed.Should().Be(4);
    }
    
    [Fact]
    public void SkipNone()
    {
        RecordSpanExtensions.SkipPastAll(
                Repeating(), GameConstants.Oblivion,
                RecordTypes.FNAM, out var numPassed)
            .Should().Be(0);
        numPassed.Should().Be(0);
    }
}