using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class TryFindFirstSubrecordsTests : RecordSpanExtensionTests
{
    [Fact]
    public void FindFirstSubrecords_Empty()
    {
        var b = new byte[0];
        var ret = RecordSpanExtensions.TryFindFirstSubrecords(b, GameConstants.Oblivion, FirstTypicalType, SecondTypicalType);
        Assert.Equal(2, ret.Length);
        Assert.Null(ret[0]);
        Assert.Null(ret[1]);
    }

    [Fact]
    public void FindFirstSubrecords_Typical()
    {
        var ret = RecordSpanExtensions.TryFindFirstSubrecords(GetTypical(), GameConstants.Oblivion, SecondTypicalType, FirstTypicalType);
        Assert.Equal(2, ret.Length);
        Assert.Equal(SecondTypicalLocation, ret[0]?.Location);
        Assert.Equal(FirstTypicalLocation, ret[1]?.Location);
    }

    [Fact]
    public void FindFirstSubrecords_Single()
    {
        var ret = RecordSpanExtensions.TryFindFirstSubrecords(GetTypical(), GameConstants.Oblivion, SecondTypicalType);
        Assert.Single(ret);
        Assert.Equal(SecondTypicalLocation, ret[0]?.Location);
    }

    [Fact]
    public void FindFirstSubrecords_Duplicate()
    {
        var ret = RecordSpanExtensions.TryFindFirstSubrecords(GetDuplicate(), GameConstants.Oblivion, SecondTypicalType, FirstTypicalType);
        Assert.Equal(2, ret.Length);
        Assert.Equal(SecondTypicalLocation, ret[0]?.Location);
        Assert.Equal(FirstTypicalLocation, ret[1]?.Location);
    }
}