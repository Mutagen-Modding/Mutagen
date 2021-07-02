using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Order;
using Xunit;
using Path = System.IO.Path;
using FileNotFoundException = System.IO.FileNotFoundException;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class PluginListingProviderTests
    {
        private const string BaseFolder = "C:/BaseFolder";

        private PluginListingsProvider GetProvider(IFileSystem fs, string pluginPath)
        {
            var parser = new Bethesda.Plugins.Order.PluginListingsParser(
                new ModListingParser(
                    new HasEnabledMarkersInjector(true)));
            var pluginPathContext = new PluginPathInjection(pluginPath);
            return new PluginListingsProvider(
                new GameReleaseInjection(GameRelease.Oblivion),
                new TimestampedPluginListingsProvider(
                    new TimestampAligner(fs),
                    new TimestampedPluginListingsPreferences(),
                    new PluginRawListingsReader(
                        fs,
                        parser),
                    new DataDirectoryInjection("Something"),
                    pluginPathContext),
                new EnabledPluginListingsProvider(
                    fs,
                    parser,
                    pluginPathContext));
        }

        [Fact]
        public void FromPathMissing()
        {
            var fs = new MockFileSystem();
            fs.Directory.CreateDirectory(BaseFolder);
            var missingPath = Path.Combine(BaseFolder, "Plugins.txt");
            Action a = () =>
                GetProvider(fs, missingPath).Get();
            a.Should().Throw<FileNotFoundException>();
        }
    }
}