using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public class EnumerateSubrecordsTests : RecordSpanExtensionTests
{
    [Fact]
    public void EnumerateSubrecordsEmpty()
    {
        byte[] b = Array.Empty<byte>();
        Assert.Empty(RecordSpanExtensions.EnumerateSubrecords(new ReadOnlyMemorySlice<byte>(b), GameConstants.Oblivion));
    }

    [Fact]
    public void EnumerateSubrecordsTypical()
    {
        var ret = RecordSpanExtensions.EnumerateSubrecords(GetTypical(), GameConstants.Oblivion).ToArray();
        Assert.Equal(2, ret.Length);
        Assert.Equal(FirstTypicalType, ret[0].Key);
        Assert.Equal(FirstTypicalLocation, ret[0].Value);
        Assert.Equal(SecondTypicalType, ret[1].Key);
        Assert.Equal(SecondTypicalLocation, ret[1].Value);
    }

    [Fact]
    public void EnumerateSubrecordsDuplicate()
    {
        var ret = RecordSpanExtensions.EnumerateSubrecords(GetDuplicate(), GameConstants.Oblivion).ToArray();
        Assert.Equal(3, ret.Length);
        Assert.Equal(FirstTypicalType, ret[0].Key);
        Assert.Equal(FirstTypicalLocation, ret[0].Value);
        Assert.Equal(SecondTypicalType, ret[1].Key);
        Assert.Equal(SecondTypicalLocation, ret[1].Value);
        Assert.Equal(DuplicateType, ret[2].Key);
        Assert.Equal(DuplicateLocation, ret[2].Value);
    }
}