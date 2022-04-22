using FluentAssertions;
using Mutagen.Bethesda.Plugins.Utility;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class MajorRecordTypePrinterTests
{
    [Fact]
    public void GameSpecificDirect()
    {
        MajorRecordPrinter<AcousticSpace>.TypeString.Should().Be("Skyrim.AcousticSpace");
    }

    [Fact]
    public void GameSpecificGetter()
    {
        MajorRecordPrinter<IAcousticSpace>.TypeString.Should().Be("Skyrim.IAcousticSpace");
    }
}