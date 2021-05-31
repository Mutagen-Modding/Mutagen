using FluentAssertions;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records
{
    public class TranslationMask_Tests
    {
        [Fact]
        public void FalseLoquiSpecification()
        {
            ICell cell = new Cell(Utility.Form1, SkyrimRelease.SkyrimLE);
            cell.Landscape = new Landscape(Utility.Form2, SkyrimRelease.SkyrimLE);
            var copy = (Cell)cell.DeepCopy(new Cell.TranslationMask(true)
            {
                Landscape = false
            });
            copy.Landscape.Should().BeNull();
        }
    }
}
