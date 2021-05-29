using DynamicData;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class CreationClubListings_Test
    {
        [Fact]
        public void FromCreationClubPathMissing()
        {
            using var tmpFolder = Utility.GetTempFolder(nameof(CreationClubListings_Test));
            var missingPath = Path.Combine(tmpFolder.Dir.Path, "Skyrim.ccc");
            Action a = () =>
                CreationClubListings.ListingsFromPath(
                    cccFilePath: missingPath,
                    dataPath: default);
            a.Should().Throw<FileNotFoundException>();
        }

        [Fact]
        public void FromCreationClubPath()
        {
            using var tmpFolder = Utility.GetTempFolder(nameof(CreationClubListings_Test));
            var cccPath = Path.Combine(tmpFolder.Dir.Path, "Skyrim.ccc");
            var dataPath = Path.Combine(tmpFolder.Dir.Path, "Data");
            File.WriteAllLines(cccPath,
                new string[]
                {
                    Utility.LightMasterModKey.FileName,
                    Utility.LightMasterModKey2.FileName,
                });
            Directory.CreateDirectory(dataPath);
            File.WriteAllText(Path.Combine(dataPath, Utility.LightMasterModKey.FileName), string.Empty);
            var results = CreationClubListings.ListingsFromPath(
                    cccFilePath: cccPath,
                    dataPath: dataPath)
                .ToList();
            results.Should().HaveCount(1);
            results[0].Should().BeEquivalentTo(new ModListing(Utility.LightMasterModKey, enabled: true));
        }

        [Fact]
        public async Task LiveLoadOrder()
        {
            using var tmpFolder = Utility.GetTempFolder(nameof(CreationClubListings_Test));
            var path = Path.Combine(tmpFolder.Dir.Path, "Skyrim.ccc");
            var data = Path.Combine(tmpFolder.Dir.Path, "Data");
            Directory.CreateDirectory(data);
            File.WriteAllText(Path.Combine(data, Skyrim.Constants.Skyrim.FileName), string.Empty);
            File.WriteAllText(Path.Combine(data, Skyrim.Constants.Update.FileName), string.Empty);
            File.WriteAllLines(path,
                new string[]
                {
                    Skyrim.Constants.Skyrim.ToString(),
                    Skyrim.Constants.Dawnguard.ToString(),
                });
            var live = CreationClubListings.GetLiveLoadOrder(path, data, out var state);
            {
                var list = live.AsObservableList();
                Assert.Equal(1, list.Count);
                Assert.Equal(Skyrim.Constants.Skyrim, list.Items.ElementAt(0).ModKey);
                File.WriteAllText(Path.Combine(data, Skyrim.Constants.Dawnguard.FileName), string.Empty);
                await Task.Delay(200);
                Assert.Equal(2, list.Count);
                Assert.Equal(Skyrim.Constants.Skyrim, list.Items.ElementAt(0).ModKey);
                Assert.Equal(Skyrim.Constants.Dawnguard, list.Items.ElementAt(1).ModKey);
                File.WriteAllLines(path,
                    new string[]
                    {
                        Skyrim.Constants.Skyrim.ToString(),
                        Skyrim.Constants.Update.ToString(),
                        Skyrim.Constants.Dawnguard.ToString(),
                    });
                await Task.Delay(200);
                Assert.Equal(3, list.Count);
                Assert.Equal(Skyrim.Constants.Skyrim, list.Items.ElementAt(0).ModKey);
                Assert.Equal(Skyrim.Constants.Update, list.Items.ElementAt(1).ModKey);
                Assert.Equal(Skyrim.Constants.Dawnguard, list.Items.ElementAt(2).ModKey);
                File.Delete(Path.Combine(data, Skyrim.Constants.Dawnguard.FileName));
                await Task.Delay(200);
                Assert.Equal(2, list.Count);
                Assert.Equal(Skyrim.Constants.Skyrim, list.Items.ElementAt(0).ModKey);
                Assert.Equal(Skyrim.Constants.Update, list.Items.ElementAt(1).ModKey);
            }
            await Task.Delay(200);
        }
    }
}
