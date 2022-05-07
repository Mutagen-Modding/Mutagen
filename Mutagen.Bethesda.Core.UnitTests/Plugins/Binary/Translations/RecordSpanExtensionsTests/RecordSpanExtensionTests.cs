using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Testing;
using Noggog;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public abstract class RecordSpanExtensionTests
{
    public static readonly RecordType FirstTypicalType = new("EDID");
    public static readonly int FirstTypicalLocation = 0;
    public static readonly RecordType SecondTypicalType = new("FNAM");
    public static readonly int SecondTypicalLocation = 7 + GameConstants.Oblivion.SubConstants.HeaderLength;
    public static readonly RecordType DuplicateType = new("EDID");
    public static readonly int DuplicateLocation = 7 + GameConstants.Oblivion.SubConstants.HeaderLength * 2 + 9;
    
    public static ReadOnlyMemorySlice<byte> GetTypical()
    {
        return TestDataPathing.GetBytes("Plugins/Binary/Translations/RecordSpanExtensionsTests/SomeSubrecords");
    }
    
    public static ReadOnlyMemorySlice<byte> GetDuplicate()
    {
        return TestDataPathing.GetBytes("Plugins/Binary/Translations/RecordSpanExtensionsTests/DuplicateSubrecord");
    }
}