using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class FindSubrecordTests : RecordSpanExtensionTests
{
    [Fact]
    public void FindFirstSubrecordEmpty()
    {
        var b = new byte[0];
        Assert.Null(RecordSpanExtensions.TryFindSubrecord(b, GameConstants.Oblivion, SecondType));
    }

    [Fact]
    public void FindFirstSubrecordTypical()
    {
        Assert.Equal(SecondLocation, RecordSpanExtensions.TryFindSubrecord(GetTypical(), GameConstants.Oblivion, SecondType)?.Location);
    }

    [Fact]
    public void FindFirstSubrecordDuplicate()
    {
        Assert.Equal(FirstLocation, RecordSpanExtensions.TryFindSubrecord(GetTypical(), GameConstants.Oblivion, FirstType)?.Location);
    }
}