using FluentAssertions;
using Mutagen.Bethesda.Plugins.RecordTypeMapping;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using Xunit;

namespace Mutagen.Bethesda.Tests.Plugins.RecordTypeMapping;

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