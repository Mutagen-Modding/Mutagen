using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using Xunit;
using Path = System.IO.Path;
using FileNotFoundException = System.IO.FileNotFoundException;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class PluginListingProviderTests
    {
        private const string BaseFolder = "C:/BaseFolder";

        private PluginListingsProvider GetProvider(IFileSystem fs)
        {
            return new PluginListingsProvider(
                fs,
                new PluginPathProvider(fs),
                new TimestampAligner(fs));
        }

        [Fact]
        public void FromPathMissing()
        {
            var fs = new MockFileSystem();
            fs.Directory.CreateDirectory(BaseFolder);
            var missingPath = Path.Combine(BaseFolder, "Plugins.txt");
            Action a = () =>
                GetProvider(fs).ListingsFromPath(
                    pluginTextPath: missingPath,
                    game: GameRelease.Oblivion,
                    dataPath: default);
            a.Should().Throw<FileNotFoundException>();
        }
    }
}