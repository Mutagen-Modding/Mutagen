using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class FindFirstSubrecordTests : RecordSpanExtensionTests
{
    [Fact]
    public void FindFirstSubrecord_Empty()
    {
        var b = new byte[0];
        Assert.Null(RecordSpanExtensions.TryFindSubrecord(b, GameConstants.Oblivion, SecondTypicalType));
    }

    [Fact]
    public void FindFirstSubrecord_Typical()
    {
        Assert.Equal(SecondTypicalLocation, RecordSpanExtensions.TryFindSubrecord(GetTypical(), GameConstants.Oblivion, SecondTypicalType)?.Location);
    }

    [Fact]
    public void FindFirstSubrecord_Duplicate()
    {
        Assert.Equal(FirstTypicalLocation, RecordSpanExtensions.TryFindSubrecord(GetTypical(), GameConstants.Oblivion, FirstTypicalType)?.Location);
    }
}