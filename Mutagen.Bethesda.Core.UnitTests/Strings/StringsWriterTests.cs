using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Mutagen.Bethesda.Core.UnitTests.AutoData;
using Mutagen.Bethesda.Strings;
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
            .Should().Be(1);
        sut.Register(StringsSource.DL,
                new KeyValuePair<Language, string>(Language.English, "There"),
                new KeyValuePair<Language, string>(Language.French, "Le"))
            .Should().Be(2);
        sut.Register(StringsSource.IL,
                new KeyValuePair<Language, string>(Language.English, "World"),
                new KeyValuePair<Language, string>(Language.French, "Monde"))
            .Should().Be(3);
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
        var path = Path.Combine(sut.WriteDir, StringsUtility.GetFileName(
            sut.LanguageFormat,
            sut.ModKey, lang, source));
        var reader = new StringsLookupOverlay(
            sut.FileSystem.File.ReadAllBytes(path),
            source, 
            sut.EncodingProvider.GetEncoding(GameRelease.SkyrimSE, lang));
        reader.Count.Should().Be(1);
        var kv = reader.First();
        kv.Key.Should().Be(index);
        kv.Value.Should().Be(str);
    }
}