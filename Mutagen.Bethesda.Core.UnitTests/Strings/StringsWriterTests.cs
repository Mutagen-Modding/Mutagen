using Shouldly;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Strings;

public class StringsWriterTests
{
    [Theory]
    [MutagenAutoData]
    public void Typical(StringsWriter sut)
    {
        sut.Register(StringsSource.Normal,
                new KeyValuePair<Language, string>(Language.English, "Hello"),
                new KeyValuePair<Language, string>(Language.French, "Bonjour"))
            .ShouldEqual(1);
        sut.Register(StringsSource.DL,
                new KeyValuePair<Language, string>(Language.English, "There"),
                new KeyValuePair<Language, string>(Language.French, "Le"))
            .ShouldEqual(2);
        sut.Register(StringsSource.IL,
                new KeyValuePair<Language, string>(Language.English, "World"),
                new KeyValuePair<Language, string>(Language.French, "Monde"))
            .ShouldEqual(3);
        sut.Dispose();
        AssertStringInFile(sut, Language.English, StringsSource.Normal, "Hello", 1);
        AssertStringInFile(sut, Language.English, StringsSource.DL, "There", 2);
        AssertStringInFile(sut, Language.English, StringsSource.IL, "World", 3);
        AssertStringInFile(sut, Language.French, StringsSource.Normal, "Bonjour", 1);
        AssertStringInFile(sut, Language.French, StringsSource.DL, "Le", 2);
        AssertStringInFile(sut, Language.French, StringsSource.IL, "Monde", 3);
    }

    public void AssertStringInFile(
        StringsWriter sut,
        Language lang, 
        StringsSource source,
        string str, 
        uint index)
    {
        StringsLookupOverlay reader = GetStringOverlay(sut, lang, source);
        reader.Count.ShouldBe(1);
        var kv = reader.First();
        kv.Key.ShouldBe(index);
        kv.Value.ShouldBe(str);
    }

    private static StringsLookupOverlay GetStringOverlay(StringsWriter sut, Language lang, StringsSource source)
    {
        string path = GetStringPath(sut, lang, source);
        var reader = new StringsLookupOverlay(
            sut.FileSystem.File.ReadAllBytes(path),
            source,
            sut.EncodingProvider.GetEncoding(GameRelease.SkyrimSE, lang));
        return reader;
    }

    private static string GetStringPath(StringsWriter sut, Language lang, StringsSource source)
    {
        return Path.Combine(sut.WriteDir, StringsUtility.GetFileName(
            sut.LanguageFormat,
            sut.ModKey, lang, source));
    }

    [Theory]
    [MutagenAutoData]
    public void WritesNothingIfEmpty(StringsWriter sut)
    {
        sut.Dispose();
        foreach (var source in Enum.GetValues<StringsSource>())
        {
            sut.FileSystem.File.Exists(GetStringPath(sut, Language.English, source)).ShouldBeFalse();
        }
    }

    [Theory]
    [MutagenAutoData]
    public void WritesOtherStringFilesIfOnlyOneNeeded(StringsWriter sut)
    {
        sut.Register(StringsSource.Normal,
                new KeyValuePair<Language, string>(Language.English, "Hello"))
            .ShouldEqual(1);
        sut.Dispose();
        foreach (var source in Enum.GetValues<StringsSource>())
        {
            sut.FileSystem.File.Exists(GetStringPath(sut, Language.English, source)).ShouldBeTrue();
        }
    }

    [Theory]
    [MutagenAutoData]
    public void FirstRegistrationIsNotZero(StringsWriter sut)
    {
        var str = new TranslatedString(Language.English, "Hello");
        sut.Register(StringsSource.Normal, str).ShouldEqual(1);
    }

    [Theory]
    [MutagenAutoData]
    public void RegisterNullsReturnZero(StringsWriter sut)
    {
        var str = new TranslatedString(Language.English, default(string?));
        sut.Register(StringsSource.Normal, str).ShouldEqual(0);
    }

    [Theory]
    [MutagenAutoData]
    public void RegisterEmptyReturnsValidIndex(StringsWriter sut)
    {
        var str = new TranslatedString(Language.English, string.Empty);
        sut.Register(StringsSource.Normal, str).ShouldNotBe<uint>(0);
    }
}
