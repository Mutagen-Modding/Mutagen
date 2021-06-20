using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using Xunit;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class ListingProviderTests
    {
        private const string BaseFolder = "C:/BaseFolder";

        private LoadOrderListingsProvider GetRetriever(IFileSystem fs)
        {
            var pathProvider = new PluginPathProvider(fs);
            var cccPathProvider = new CreationClubPathProvider(fs);
            return new LoadOrderListingsProvider(
                fs,
                new OrderListings(),
                pathProvider,
                new PluginListingsProvider(
                    fs,
                    pathProvider,
                    new TimestampAligner(fs)),
                cccPathProvider,
                new CreationClubListingsProvider(fs, cccPathProvider));
        }
        
        [Fact]
        public void GetListings()
        {
            var fs = new MockFileSystem();
            fs.Directory.CreateDirectory(BaseFolder);
            var cccPath = Path.Combine(BaseFolder, "Skyrim.ccc");
            var pluginsPath = Path.Combine(BaseFolder, "Plugins.txt");
            var dataPath = Path.Combine(BaseFolder, "Data");
            fs.File.WriteAllLines(cccPath,
                new string[]
                {
                    Utility.LightMasterModKey.FileName,
                    Utility.LightMasterModKey2.FileName,
                });
            fs.File.WriteAllLines(pluginsPath,
                new string[]
                {
                    $"*{Utility.MasterModKey.FileName}",
                    $"{Utility.MasterModKey2.FileName}",
                    $"*{Utility.LightMasterModKey3.FileName}",
                    $"{Utility.LightMasterModKey4.FileName}",
                    $"*{Utility.PluginModKey.FileName}",
                    $"{Utility.PluginModKey2.FileName}",
                });
            fs.Directory.CreateDirectory(dataPath);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.MasterModKey.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.MasterModKey2.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey3.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey4.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey2.FileName), string.Empty);
            var listingsGetter = GetRetriever(fs);
            var results = listingsGetter.GetListings(
                    game: GameRelease.SkyrimSE,
                    dataPath: dataPath,
                    pluginsFilePath: pluginsPath,
                    creationClubFilePath: cccPath)
                .ToList();
            results.Should().HaveCount(7);
            results.Should().Equal(new ModListing[]
            {
                new ModListing(Utility.LightMasterModKey, enabled: true),
                new ModListing(Utility.MasterModKey, enabled: true),
                new ModListing(Utility.MasterModKey2, enabled: false),
                new ModListing(Utility.LightMasterModKey3, enabled: true),
                new ModListing(Utility.LightMasterModKey4, enabled: false),
                new ModListing(Utility.PluginModKey, enabled: true),
                new ModListing(Utility.PluginModKey2, enabled: false),
            });
        }

        [Fact]
        public void CreationClubMissing()
        {
            var fs = new MockFileSystem();
            fs.Directory.CreateDirectory(BaseFolder);
            var cccPath = Path.Combine(BaseFolder, "Skyrim.ccc");
            var pluginsPath = Path.Combine(BaseFolder, "Plugins.txt");
            var dataPath = Path.Combine(BaseFolder, "Data");
            fs.File.WriteAllLines(pluginsPath,
                new string[]
                {
                    $"*{Utility.MasterModKey.FileName}",
                    $"{Utility.MasterModKey2.FileName}",
                    $"*{Utility.LightMasterModKey3.FileName}",
                    $"{Utility.LightMasterModKey4.FileName}",
                    $"*{Utility.PluginModKey.FileName}",
                    $"{Utility.PluginModKey2.FileName}",
                });
            fs.Directory.CreateDirectory(dataPath);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.MasterModKey.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.MasterModKey2.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey3.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey4.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey2.FileName), string.Empty);
            var listingsGetter = GetRetriever(fs);
            var results = listingsGetter.GetListings(
                    game: GameRelease.SkyrimSE,
                    dataPath: dataPath,
                    pluginsFilePath: pluginsPath,
                    creationClubFilePath: cccPath)
                .ToList();
            results.Should().HaveCount(6);
            results.Should().Equal(new ModListing[]
            {
                new ModListing(Utility.MasterModKey, enabled: true),
                new ModListing(Utility.MasterModKey2, enabled: false),
                new ModListing(Utility.LightMasterModKey3, enabled: true),
                new ModListing(Utility.LightMasterModKey4, enabled: false),
                new ModListing(Utility.PluginModKey, enabled: true),
                new ModListing(Utility.PluginModKey2, enabled: false),
            });
        }

        [Fact]
        public void NoCreationClub()
        {
            var fs = new MockFileSystem();
            fs.Directory.CreateDirectory(BaseFolder);
            var pluginsPath = Path.Combine(BaseFolder, "Plugins.txt");
            var dataPath = Path.Combine(BaseFolder, "Data");
            fs.File.WriteAllLines(pluginsPath,
                new string[]
                {
                    $"{Utility.MasterModKey.FileName}",
                    $"{Utility.PluginModKey.FileName}",
                });
            fs.Directory.CreateDirectory(dataPath);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.MasterModKey.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey.FileName), string.Empty);
            var listingsGetter = GetRetriever(fs);
            var results = listingsGetter.GetListings(
                    game: GameRelease.Oblivion,
                    dataPath: dataPath,
                    pluginsFilePath: pluginsPath,
                    creationClubFilePath: null)
                .ToList();
            results.Should().HaveCount(2);
            results.Should().Equal(new ModListing[]
            {
                new ModListing(Utility.MasterModKey, enabled: true),
                new ModListing(Utility.PluginModKey, enabled: true),
            });
        }

        /// <summary>
        /// Vortex lists creation club entries at the start of the plugins.txt, but leaves them marked
        /// as not active?
        /// </summary>
        [Fact]
        public void VortexCreationClub()
        {
            var fs = new MockFileSystem();
            fs.Directory.CreateDirectory(BaseFolder);
            var cccPath = Path.Combine(BaseFolder, "Skyrim.ccc");
            var pluginsPath = Path.Combine(BaseFolder, "Plugins.txt");
            var dataPath = Path.Combine(BaseFolder, "Data");
            fs.File.WriteAllLines(cccPath,
                new string[]
                {
                    Utility.LightMasterModKey.FileName,
                    Utility.LightMasterModKey2.FileName,
                });
            fs.File.WriteAllLines(pluginsPath,
                new string[]
                {
                    Utility.LightMasterModKey2.FileName,
                    Utility.LightMasterModKey.FileName,
                    $"*{Utility.MasterModKey.FileName}",
                    $"{Utility.MasterModKey2.FileName}",
                    $"*{Utility.LightMasterModKey3.FileName}",
                    $"{Utility.LightMasterModKey4.FileName}",
                    $"*{Utility.PluginModKey.FileName}",
                    $"{Utility.PluginModKey2.FileName}",
                });
            fs.Directory.CreateDirectory(dataPath);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey2.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.MasterModKey.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.MasterModKey2.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey3.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey4.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey.FileName), string.Empty);
            fs.File.WriteAllText(Path.Combine(dataPath, Utility.PluginModKey2.FileName), string.Empty);
            var listingsGetter = GetRetriever(fs);
            var results = listingsGetter.GetListings(
                    game: GameRelease.SkyrimSE,
                    dataPath: dataPath,
                    pluginsFilePath: pluginsPath,
                    creationClubFilePath: cccPath)
                .ToList();
            results.Should().HaveCount(8);
            results.Should().Equal(new ModListing[]
            {
                new ModListing(Utility.LightMasterModKey2, enabled: true),
                new ModListing(Utility.LightMasterModKey, enabled: true),
                new ModListing(Utility.MasterModKey, enabled: true),
                new ModListing(Utility.MasterModKey2, enabled: false),
                new ModListing(Utility.LightMasterModKey3, enabled: true),
                new ModListing(Utility.LightMasterModKey4, enabled: false),
                new ModListing(Utility.PluginModKey, enabled: true),
                new ModListing(Utility.PluginModKey2, enabled: false),
            });
        }

        [Fact]
        public void FromPathMissingWithImplicit()
        {
            using var tmpFolder = Utility.GetTempFolder(nameof(ModListings_Tests));
            using var file = File.Create(Path.Combine(tmpFolder.Dir.Path, "Skyrim.esm"));
            var missingPath = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            LoadOrder.GetListings(
                    pluginsFilePath: missingPath,
                    creationClubFilePath: null,
                    game: GameRelease.SkyrimSE,
                    dataPath: tmpFolder.Dir.Path)
                .Should().Equal(new ModListing("Skyrim.esm", true));
            // var fs = new MockFileSystem();
            // fs.Directory.CreateDirectory(BaseFolder);
            // using var file = fs.File.Create(Path.Combine(BaseFolder, "Skyrim.esm"));
            // var missingPath = Path.Combine(BaseFolder, "Plugins.txt");
            // var listingsGetter = GetRetriever(fs);
            // listingsGetter.GetListings(
            //         pluginsFilePath: missingPath,
            //         creationClubFilePath: null,
            //         game: GameRelease.SkyrimSE,
            //         dataPath: BaseFolder)
            //     .Should().Equal(new ModListing("Skyrim.esm", true));
        }
    }
}