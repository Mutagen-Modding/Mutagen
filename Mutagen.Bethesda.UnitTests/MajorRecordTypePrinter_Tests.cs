using FluentAssertions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Utility;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class MajorRecordTypePrinter_Tests
    {
        [Fact]
        public void CommonInterface()
        {
            MajorRecordTypePrinter<IMajorRecordCommonGetter>.TypeString.Should().Be("IMajorRecordCommonGetter");
        }

        [Fact]
        public void GameSpecificDirect()
        {
            MajorRecordTypePrinter<Skyrim.AcousticSpace>.TypeString.Should().Be("Skyrim.AcousticSpace");
        }

        [Fact]
        public void GameSpecificGetter()
        {
            MajorRecordTypePrinter<Skyrim.IAcousticSpace>.TypeString.Should().Be("Skyrim.IAcousticSpace");
        }
    }
}
