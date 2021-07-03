using DynamicData;
using FluentAssertions;
using Mutagen.Bethesda.Plugins.Order;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Plugins;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class CreationClubListingsTests 
    {
        [Fact]
        public void FromCreationClubPathMissing()
        {
            using var tmpFolder = Utility.GetTempFolder(nameof(CreationClubListingsTests));
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
            var sch = new TestScheduler();
            sch.Schedule(TimeSpan.FromTicks(1), () => { });
        
            using var tmpFolder = Utility.GetTempFolder(nameof(CreationClubListingsTests));
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
            results[0].Should().Be(new ModListing(Utility.LightMasterModKey, enabled: true));
        }

        [Fact]
        public async Task LiveLoadOrder()
        {
            var modified = new MockFileSystemWatcher();
            var dataFolder = "C:/DataFolder";
            var path = Path.Combine(dataFolder, "Skyrim.ccc");
            var fs = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { Path.Combine(dataFolder, Skyrim.Constants.Skyrim.FileName), string.Empty },
                { Path.Combine(dataFolder, Skyrim.Constants.Update.FileName), string.Empty },
                { path, @$"{Skyrim.Constants.Skyrim}
{Skyrim.Constants.Dawnguard}"}
            })
            {
                FileSystemWatcher = new MockFileSystemWatcherFactory(modified)
            };
            ErrorResponse err = ErrorResponse.Failure;
            var live = CreationClubListings.GetLiveLoadOrder(path, dataFolder, out var state,
                fileSystem: fs);
            {
                var list = live.AsObservableList();
                state.Subscribe(x => err = x);
                Assert.Equal(1, list.Count);
                Assert.Equal(Skyrim.Constants.Skyrim, list.Items.ElementAt(0).ModKey);
                var dawnguardPath = Path.Combine(dataFolder, Skyrim.Constants.Dawnguard.FileName);
                fs.File.WriteAllText(dawnguardPath, string.Empty);
                modified.MarkCreated(dawnguardPath);
                Assert.Equal(2, list.Count);
                Assert.Equal(Skyrim.Constants.Skyrim, list.Items.ElementAt(0).ModKey);
                Assert.Equal(Skyrim.Constants.Dawnguard, list.Items.ElementAt(1).ModKey);
                err.Succeeded.Should().BeTrue();
                fs.File.WriteAllLines(path,
                    new string[]
                    {
                        Skyrim.Constants.Skyrim.ToString(),
                        Skyrim.Constants.Update.ToString(),
                        Skyrim.Constants.Dawnguard.ToString(),
                    });
                modified.MarkChanged(path);
                Assert.Equal(3, list.Count);
                Assert.Equal(Skyrim.Constants.Skyrim, list.Items.ElementAt(0).ModKey);
                Assert.Equal(Skyrim.Constants.Update, list.Items.ElementAt(1).ModKey);
                Assert.Equal(Skyrim.Constants.Dawnguard, list.Items.ElementAt(2).ModKey);
                err.Succeeded.Should().BeTrue();
                fs.File.Delete(dawnguardPath);
                modified.MarkDeleted(dawnguardPath);
                Assert.Equal(2, list.Count);
                Assert.Equal(Skyrim.Constants.Skyrim, list.Items.ElementAt(0).ModKey);
                Assert.Equal(Skyrim.Constants.Update, list.Items.ElementAt(1).ModKey);
                err.Succeeded.Should().BeTrue();
            }
        }
    }
}
