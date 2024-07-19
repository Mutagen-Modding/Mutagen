using FluentAssertions;
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
            .Should().Be(RecordTypes.ARMO);
        RecordTypeLookup.GetMajorRecordType<IArmor>()
            .Should().Be(RecordTypes.ARMO);
        RecordTypeLookup.GetMajorRecordType<Armor>()
            .Should().Be(RecordTypes.ARMO);
        RecordTypeLookup.GetMajorRecordType<IArmorInternal>()
            .Should().Be(RecordTypes.ARMO);
    }
}