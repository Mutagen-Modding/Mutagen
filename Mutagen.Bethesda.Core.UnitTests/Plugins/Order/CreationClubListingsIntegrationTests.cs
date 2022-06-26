using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using AutoFixture.Xunit2;
using DynamicData;
using FluentAssertions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.AutoData;
using Noggog;
using Noggog.Testing.FileSystem;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class CreationClubListingsIntegrationTests 
{
    [Theory, MutagenAutoData]
    public void FromCreationClubPathMissing(
        [Frozen]IDataDirectoryProvider dataDirectoryProvider,
        [Frozen]IFileSystem fs)
    {
        var missingPath = Path.Combine(dataDirectoryProvider.Path, "Skyrim.ccc");
        Action a = () =>
            CreationClubListings.LoadOrderListingsFromPath(
                cccFilePath: missingPath,
                dataPath: default,
                fileSystem: fs);
        a.Should().Throw<FileNotFoundException>();
    }

    [Theory, MutagenAutoData]
    public void FromCreationClubPath(
        [Frozen]MockFileSystem fs,
        [Frozen]ModPath existingModPath,
        [Frozen]IDataDirectoryProvider dataDir,
        [Frozen]ICreationClubListingsPathProvider cccPath)
    {
        fs.File.WriteAllLines(cccPath.Path,
            new string[]
            {
                existingModPath.ModKey.FileName,
                TestConstants.LightMasterModKey2.FileName,
            });
        var results = CreationClubListings.LoadOrderListingsFromPath(
                cccFilePath: cccPath.Path!.Value,
                dataPath: dataDir.Path,
                fileSystem: fs)
            .ToList();
        results.Should().HaveCount(1);
        results[0].Should().Be(new ModListing(existingModPath.ModKey, enabled: true, existsOnDisk: true));
    }

    [Theory, MutagenAutoData]
    public async Task LiveLoadOrder(
        IDataDirectoryProvider dataFolder,
        [Frozen]MockFileSystemWatcher modified,
        [Frozen]MockFileSystem fs)
    {
        var ccPath = Path.Combine(dataFolder.Path, "Skyrim.ccc");
        fs.File.WriteAllText(Path.Combine(dataFolder.Path, TestConstants.PluginModKey.FileName), string.Empty);
        fs.File.WriteAllText(Path.Combine(dataFolder.Path, TestConstants.PluginModKey2.FileName), string.Empty);
        fs.File.WriteAllText(ccPath, @$"{TestConstants.PluginModKey.FileName}
{TestConstants.PluginModKey3.FileName}");
        ErrorResponse err = ErrorResponse.Failure;
        var live = CreationClubListings.GetLiveLoadOrderListings(ccPath, dataFolder.Path, out var state,
            fileSystem: fs);
        {
            var list = live.AsObservableList();
            state.Subscribe(x => err = x);
            Assert.Equal(1, list.Count);
            Assert.Equal(TestConstants.PluginModKey, list.Items.ElementAt(0).ModKey);
            var thirdPath = Path.Combine(dataFolder.Path, TestConstants.PluginModKey3.FileName);
            fs.File.WriteAllText(thirdPath, string.Empty);
            modified.MarkCreated(thirdPath);
            Assert.Equal(2, list.Count);
            Assert.Equal(TestConstants.PluginModKey, list.Items.ElementAt(0).ModKey);
            Assert.Equal(TestConstants.PluginModKey3, list.Items.ElementAt(1).ModKey);
            err.Succeeded.Should().BeTrue();
            fs.File.WriteAllLines(ccPath,
                new string[]
                {
                    TestConstants.PluginModKey.ToString(),
                    TestConstants.PluginModKey2.ToString(),
                    TestConstants.PluginModKey3.ToString(),
                });
            modified.MarkChanged(ccPath);
            Assert.Equal(3, list.Count);
            Assert.Equal(TestConstants.PluginModKey, list.Items.ElementAt(0).ModKey);
            Assert.Equal(TestConstants.PluginModKey2, list.Items.ElementAt(1).ModKey);
            Assert.Equal(TestConstants.PluginModKey3, list.Items.ElementAt(2).ModKey);
            err.Succeeded.Should().BeTrue();
            fs.File.Delete(thirdPath);
            modified.MarkDeleted(thirdPath);
            Assert.Equal(2, list.Count);
            Assert.Equal(TestConstants.PluginModKey, list.Items.ElementAt(0).ModKey);
            Assert.Equal(TestConstants.PluginModKey2, list.Items.ElementAt(1).ModKey);
            err.Succeeded.Should().BeTrue();
        }
    }
}