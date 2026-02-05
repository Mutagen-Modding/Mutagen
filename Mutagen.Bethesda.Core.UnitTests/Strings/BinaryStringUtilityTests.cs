using Shouldly;
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
        ret.ShouldBe("Livre de sort - Paralysie générale");
    }

    [Fact]
    public void ToZStringRussianLeExample()
    {
        var ret = new MutagenEncodingProvider().GetEncoding(GameRelease.SkyrimLE, Language.Russian)
            .GetString(File.ReadAllBytes(TestDataPathing.RussianLeString));
        ret.ShouldBe("Распорядок Гая Марона");
    }

    [Fact]
    public void ParsePrependedStringTypical()
    {
        BinaryStringUtility.ParsePrependedString(File.ReadAllBytes(TestDataPathing.PrependedString), 2, MutagenEncoding._1252)
            .ShouldBe("HelloWorld");
    }

    [Fact]
    public void ParsePrependedStringZeroLength()
    {
        BinaryStringUtility.ParsePrependedString(File.ReadAllBytes(TestDataPathing.ZeroContentPrependedString), 2, MutagenEncoding._1252)
            .ShouldBe("");
    }

    [Fact]
    public void ProcessNullTerminationOnEmpty()
    {
        var span = BinaryStringUtility.ProcessNullTermination(Array.Empty<byte>().AsSpan());
        span.Length.ShouldBe(0);
    }

    [Fact]
    public void ProcessNullTerminationOnExtraFluff()
    {
        var span = BinaryStringUtility.ProcessNullTermination(File.ReadAllBytes(TestDataPathing.FluffedNullString));
        span.Length.ShouldBe(0xB);
        var str = BinaryStringUtility.ToZString(span, MutagenEncoding._utf8);
        str.ShouldBe("Hello world");
    }
}