using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class PluginListingsParser
    {
        private Stream GetStream(params string[] listings)
        {
            var sb = new StringBuilder();
            foreach (var listing in listings)
            {
                sb.AppendLine(listing);
            }
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(sb.ToString());
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        
        [Fact]
        public void Typical()
        {
            var parser = new Bethesda.Plugins.Order.PluginListingsParser( 
                new ModListingParser(
                    new HasEnabledMarkersInjector(true)));
            var result = parser.Parse(GetStream(@"*ModA.esm
ModB.esp
*ModC.esp"))
                .ToList();
            result.Should().Equal(
                new ModListing("ModA.esm", true),
                new ModListing("ModB.esp", false),
                new ModListing("ModC.esp", true));
        }
        
        [Fact]
        public void CommentsWithNoEntry()
        {
            var parser = new Bethesda.Plugins.Order.PluginListingsParser(
                new ModListingParser(
                    new HasEnabledMarkersInjector(true)));
            var result = parser.Parse(GetStream(@"*ModA.esm
#ModB.esp
*ModC.esp"))
                .ToList();
            result.Should().Equal(
                new ModListing("ModA.esm", true),
                new ModListing("ModC.esp", true));
        }
        
        [Fact]
        public void CommentTrimming()
        {
            var parser = new Bethesda.Plugins.Order.PluginListingsParser(
                new ModListingParser(
                    new HasEnabledMarkersInjector(true)));
            var result = parser.Parse(GetStream(@"*ModA.esm
ModB.esp#Hello
*ModC.esp"))
                .ToList();
            result.Should().Equal(
                new ModListing("ModA.esm", true),
                new ModListing("ModB.esp", false),
                new ModListing("ModC.esp", true));
        }
    }
}