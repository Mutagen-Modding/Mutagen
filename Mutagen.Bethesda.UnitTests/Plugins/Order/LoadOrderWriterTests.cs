using System.IO.Abstractions.TestingHelpers;
using Mutagen.Bethesda.Plugins.Order;
using Xunit;
using System.Linq;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class LoadOrderWriterTests
    {
        private const string BaseFolder = "C:/BaseFolder";
        
        [Fact]
        public void WriteExclude()
        {
            var fs = new MockFileSystem();
            fs.Directory.CreateDirectory(BaseFolder);
            var path = Path.Combine(BaseFolder, "Plugins.txt");
            new LoadOrderWriter(fs).Write(
                path,
                GameRelease.Oblivion,
                new ModListing[]
                {
                    new ModListing(Utility.PluginModKey, false),
                    new ModListing(Utility.PluginModKey2, true),
                    new ModListing(Utility.PluginModKey3, false),
                });
            var lines = fs.File.ReadAllLines(path).ToList();
            Assert.Single(lines);
            Assert.Equal(Utility.PluginModKey2.FileName, lines[0]);
        }

        [Fact]
        public void WriteMarkers()
        {
            var fs = new MockFileSystem();
            fs.Directory.CreateDirectory(BaseFolder);
            var path = Path.Combine(BaseFolder, "Plugins.txt");
            new LoadOrderWriter(fs).Write(
                path,
                GameRelease.SkyrimSE,
                new ModListing[]
                {
                    new ModListing(Utility.PluginModKey, false),
                    new ModListing(Utility.PluginModKey2, true),
                    new ModListing(Utility.PluginModKey3, false),
                });
            var lines = fs.File.ReadAllLines(path).ToList();
            Assert.Equal(3, lines.Count);
            Assert.Equal($"{Utility.PluginModKey.FileName}", lines[0]);
            Assert.Equal($"*{Utility.PluginModKey2.FileName}", lines[1]);
            Assert.Equal($"{Utility.PluginModKey3.FileName}", lines[2]);
        }

        [Fact]
        public void WriteImplicitFilteredOut()
        {
            var fs = new MockFileSystem();
            fs.Directory.CreateDirectory(BaseFolder);
            var path = Path.Combine(BaseFolder, "Plugins.txt");
            new LoadOrderWriter(fs).Write(
                path,
                GameRelease.SkyrimSE,
                new ModListing[]
                {
                    new ModListing(Utility.Skyrim, true),
                    new ModListing(Utility.PluginModKey, true),
                    new ModListing(Utility.PluginModKey2, false),
                },
                removeImplicitMods: true);
            var lines = fs.File.ReadAllLines(path).ToList();
            Assert.Equal(2, lines.Count);
            Assert.Equal($"*{Utility.PluginModKey.FileName}", lines[0]);
            Assert.Equal($"{Utility.PluginModKey2.FileName}", lines[1]);
        }

        [Fact]
        public void WriteImplicit()
        {
            var fs = new MockFileSystem();
            fs.Directory.CreateDirectory(BaseFolder);
            var path = Path.Combine(BaseFolder, "Plugins.txt");
            new LoadOrderWriter(fs).Write(
                path,
                GameRelease.SkyrimSE,
                new ModListing[]
                {
                    new ModListing(Utility.Skyrim, true),
                    new ModListing(Utility.PluginModKey, true),
                    new ModListing(Utility.PluginModKey2, false),
                },
                removeImplicitMods: false);
            var lines = fs.File.ReadAllLines(path).ToList();
            Assert.Equal(3, lines.Count);
            Assert.Equal($"*{Utility.Skyrim.FileName}", lines[0]);
            Assert.Equal($"*{Utility.PluginModKey.FileName}", lines[1]);
            Assert.Equal($"{Utility.PluginModKey2.FileName}", lines[2]);
        }
    }
}