using Mutagen.Bethesda.Strings;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Strings;

public class TryRetrieveInfoFromStringTests
{
    [Fact]
    public void TryRetrieveInfoFromString_Full()
    {
        Assert.True(StringsUtility.TryRetrieveInfoFromString(
            StringsLanguageFormat.FullName,
            "Skyrim_French.ILSTRINGS", 
            out var source, 
            out var language, 
            out var modName));
        Assert.Equal(StringsSource.IL, source);
        Assert.Equal(Language.French, language);
        Assert.Equal("Skyrim", modName.ToString());
    }

    [Fact]
    public void TryRetrieveInfoFromString_Full_Fail()
    {
        Assert.False(StringsUtility.TryRetrieveInfoFromString(
            StringsLanguageFormat.FullName,
            "Skyrim_FrenchILSTRINGS",
            out var _, out var _, out var _));
        Assert.False(StringsUtility.TryRetrieveInfoFromString(
            StringsLanguageFormat.FullName,
            "SkyrimFrench.ILSTRINGS",
            out var _, out var _, out var _));
        Assert.False(StringsUtility.TryRetrieveInfoFromString(
            StringsLanguageFormat.FullName,
            "Skyrim_fr.ILSTRINGS",
            out var _, out var _, out var _));
    }

    [Fact]
    public void TryRetrieveInfoFromString_Iso()
    {
        Assert.True(StringsUtility.TryRetrieveInfoFromString(
            StringsLanguageFormat.Iso,
            "Skyrim_fr.ILSTRINGS",
            out var source, 
            out var language,
            out var modName));
        Assert.Equal(StringsSource.IL, source);
        Assert.Equal(Language.French, language);
        Assert.Equal("Skyrim", modName.ToString());
    }

    [Fact]
    public void TryRetrieveInfoFromString_Iso_Fail()
    {
        Assert.False(StringsUtility.TryRetrieveInfoFromString(
            StringsLanguageFormat.Iso,
            "Skyrim_frILSTRINGS",
            out var _, out var _, out var _));
        Assert.False(StringsUtility.TryRetrieveInfoFromString(
            StringsLanguageFormat.Iso, 
            "Skyrimfr.ILSTRINGS",
            out var _, out var _, out var _));
        Assert.False(StringsUtility.TryRetrieveInfoFromString(
            StringsLanguageFormat.Iso, 
            "Skyrim_French.ILSTRINGS",
            out var _, out var _, out var _));
        Assert.False(StringsUtility.TryRetrieveInfoFromString(
            StringsLanguageFormat.Iso,
            "Skyrim_zz.ILSTRINGS",
            out var _, out var _, out var _));
    }
}