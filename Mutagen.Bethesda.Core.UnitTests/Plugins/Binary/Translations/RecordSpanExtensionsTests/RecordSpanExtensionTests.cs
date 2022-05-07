using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Testing;
using Noggog;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Translations.RecordSpanExtensionsTests;

public abstract class RecordSpanExtensionTests
{
    public static readonly RecordType FirstType = new("EDID");
    public static readonly int FirstLocation = 0;
    public static readonly ushort FirstLength = 7;
    public static readonly RecordType SecondType = new("FNAM");
    public static readonly int SecondLocation = FirstLength + GameConstants.Oblivion.SubConstants.HeaderLength;
    public static readonly ushort SecondLength = 9;
    public static readonly RecordType DuplicateType = new("EDID");
    public static readonly int DuplicateLocation = FirstLength + GameConstants.Oblivion.SubConstants.HeaderLength * 2 + SecondLength;
    public static readonly ushort DuplicateLength = 3;
    
    public static ReadOnlyMemorySlice<byte> GetTypical()
    {
        return TestDataPathing.GetBytes("Plugins/Binary/Translations/RecordSpanExtensionsTests/SomeSubrecords");
    }
    
    public static ReadOnlyMemorySlice<byte> GetDuplicate()
    {
        return TestDataPathing.GetBytes("Plugins/Binary/Translations/RecordSpanExtensionsTests/DuplicateSubrecord");
    }
    
    public static ReadOnlyMemorySlice<byte> Repeating()
    {
        return TestDataPathing.GetBytes("Plugins/Binary/Translations/RecordSpanExtensionsTests/RepeatingSubrecords");
    }
    
    public static ReadOnlyMemorySlice<byte> FnamStart()
    {
        return TestDataPathing.GetBytes("Plugins/Binary/Translations/RecordSpanExtensionsTests/FnamStart");
    }
}