using Shouldly;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Mutagen.Bethesda.Skyrim;
using Xunit;
using RecordTypes = Mutagen.Bethesda.Skyrim.Internals.RecordTypes;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Mapping;

public class RecordTypeLookupTests
{
    [Fact]
    public void TypicalGetMajorRecordType()
    {
        RecordTypeLookup.GetMajorRecordType<IArmorGetter>()
            .ShouldBe(RecordTypes.ARMO);
        RecordTypeLookup.GetMajorRecordType<IArmor>()
            .ShouldBe(RecordTypes.ARMO);
        RecordTypeLookup.GetMajorRecordType<Armor>()
            .ShouldBe(RecordTypes.ARMO);
        RecordTypeLookup.GetMajorRecordType<IArmorInternal>()
            .ShouldBe(RecordTypes.ARMO);
    }
}