using FluentAssertions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records
{
    public class MajorRecordTypePrinter_Tests
    {
        [Fact]
        public void CommonInterface()
        {
            MajorRecordPrinter<IMajorRecordCommonGetter>.TypeString.Should().Be("IMajorRecordCommonGetter");
        }

        [Fact]
        public void GameSpecificDirect()
        {
            MajorRecordPrinter<Skyrim.AcousticSpace>.TypeString.Should().Be("Skyrim.AcousticSpace");
        }

        [Fact]
        public void GameSpecificGetter()
        {
            MajorRecordPrinter<Skyrim.IAcousticSpace>.TypeString.Should().Be("Skyrim.IAcousticSpace");
        }
    }
}
