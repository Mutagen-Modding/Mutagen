using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class EnabledPluginsListingProviderTests : IClassFixture<Fixture>
    {
        private readonly Fixture _fixture;

        public EnabledPluginsListingProviderTests(Fixture fixture)
        {
            _fixture = fixture;
        }
        
        [Fact]
        public void PathDoesNotExist()
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                new EnabledPluginListingsProvider(
                        new MockFileSystem(),
                        _fixture.Inject.Create<IPluginListingsParser>(),
                        new PluginPathInjection("NonExistant"))
                    .Get()
                    .ToArray();
            });
        }

        [Fact]
        public void Typical()
        {
            var listings = new ModListing[]
            {
                new ModListing("ModA.esp", true),
                new ModListing("ModB.esp", false),
            };
            var path = new PluginPathInjection("C:/Plugin.txt");
            var someStream = Substitute.For<Stream>();
            var fs = Substitute.For<IFileSystem>();
            fs.File.Exists(path.Path).Returns(true);
            fs.FileStream
                .Create(path.Path, FileMode.Open, FileAccess.Read, FileShare.Read)
                .Returns(someStream);
            var parser = Substitute.For<IPluginListingsParser>();
            parser.Parse(someStream).Returns(listings);
            new EnabledPluginListingsProvider(
                    fs,
                    parser,
                    path)
                .Get()
                .Should().Equal(listings);
        }
    }
}