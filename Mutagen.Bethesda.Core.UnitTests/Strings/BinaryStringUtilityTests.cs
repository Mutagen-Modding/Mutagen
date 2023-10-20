using FluentAssertions;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Strings;

public class BinaryStringUtilityTests
{
    [Fact]
    public void ToZStringFrenchSeExample()
    {
        var ret = new MutagenEncodingProvider().GetEncoding(GameRelease.SkyrimSE, Language.French)
            .GetString(File.ReadAllBytes(TestDataPathing.FrenchSeString));
        ret.Should().Be("Livre de sort - Paralysie générale");
    }

    [Fact]
    public void ToZStringRussianLeExample()
    {
        var ret = new MutagenEncodingProvider().GetEncoding(GameRelease.SkyrimLE, Language.Russian)
            .GetString(File.ReadAllBytes(TestDataPathing.RussianLeString));
        ret.Should().Be("Распорядок Гая Марона");
    }

    [Fact]
    public void ParsePrependedStringTypical()
    {
        BinaryStringUtility.ParsePrependedString(File.ReadAllBytes(TestDataPathing.PrependedString), 2, MutagenEncoding._1252)
            .Should().Be("HelloWorld");
    }

    [Fact]
    public void ParsePrependedStringZeroLength()
    {
        BinaryStringUtility.ParsePrependedString(File.ReadAllBytes(TestDataPathing.ZeroContentPrependedString), 2, MutagenEncoding._1252)
            .Should().Be("");
    }

    [Fact]
    public void ProcessNullTerminationOnEmpty()
    {
        var span = BinaryStringUtility.ProcessNullTermination(Array.Empty<byte>().AsSpan());
        span.Length.Should().Be(0);
    }
}