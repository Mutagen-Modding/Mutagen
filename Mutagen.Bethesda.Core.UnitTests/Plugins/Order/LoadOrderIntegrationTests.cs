using System.IO.Abstractions;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Noggog.Testing.Extensions;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order;

public class LoadOrderIntegrationTests
{
    [Fact]
    public void AlignToTimestamps_Typical()
    {
        var lo = new List<(ModKey ModKey, DateTime Write)>
        {
            (TestConstants.PluginModKey4, new DateTime(2020, 8, 8, 10, 11, 0)),
            (TestConstants.PluginModKey, new DateTime(2020, 8, 8, 10, 8, 0)),
            (TestConstants.PluginModKey3, new DateTime(2020, 8, 8, 10, 10, 0)),
            (TestConstants.PluginModKey2, new DateTime(2020, 8, 8, 10, 9, 0)),
        };
        var results = LoadOrder.AlignToTimestamps(lo)
            .ToList();
        Assert.Equal(TestConstants.PluginModKey, results[0]);
        Assert.Equal(TestConstants.PluginModKey2, results[1]);
        Assert.Equal(TestConstants.PluginModKey3, results[2]);
        Assert.Equal(TestConstants.PluginModKey4, results[3]);
    }

    [Fact]
    public void AlignToTimestamps_SameTimestamps()
    {
        var lo = new List<(ModKey ModKey, DateTime Write)>
        {
            (TestConstants.PluginModKey4, new DateTime(2020, 8, 8, 10, 11, 0)),
            (TestConstants.PluginModKey, new DateTime(2020, 8, 8, 10, 8, 0)),
            (TestConstants.PluginModKey3, new DateTime(2020, 8, 8, 10, 10, 0)),
            (TestConstants.PluginModKey2, new DateTime(2020, 8, 8, 10, 11, 0)),
        };
        var results = LoadOrder.AlignToTimestamps(lo)
            .ToList();
        Assert.Equal(TestConstants.PluginModKey, results[0]);
        Assert.Equal(TestConstants.PluginModKey3, results[1]);
        Assert.Equal(TestConstants.PluginModKey4, results[2]);
        Assert.Equal(TestConstants.PluginModKey2, results[3]);
    }

    [Theory, MutagenAutoData]
    public void GetListings(
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        var cccPath = Path.Combine(existingDir.Path, "Skyrim.ccc");
        var pluginsPath = Path.Combine(existingDir.Path, "Plugins.txt");
        var dataPath = Path.Combine(existingDir.Path, "Data");
        fileSystem.File.WriteAllLines(cccPath,
            new string[]
            {
                TestConstants.LightModKey.FileName,
                TestConstants.LightModKey2.FileName,
            });
        fileSystem.File.WriteAllLines(pluginsPath,
            new string[]
            {
                $"*{TestConstants.MasterModKey.FileName}",
                $"{TestConstants.MasterModKey2.FileName}",
                $"*{TestConstants.LightModKey3.FileName}",
                $"{TestConstants.LightModKey4.FileName}",
                $"*{TestConstants.PluginModKey.FileName}",
                $"{TestConstants.PluginModKey2.FileName}",
            });
        fileSystem.Directory.CreateDirectory(dataPath);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.LightModKey.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.MasterModKey.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.MasterModKey2.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.LightModKey3.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.LightModKey4.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.PluginModKey.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.PluginModKey2.FileName), string.Empty);
        var results = LoadOrder.GetLoadOrderListings(
                game: GameRelease.SkyrimSE,
                dataPath: dataPath,
                pluginsFilePath: pluginsPath,
                creationClubFilePath: cccPath,
                fileSystem: fileSystem)
            .ToList();
        results.ShouldHaveCount(7);
        results.Select(x => new LoadOrderListing(x.ModKey, x.Enabled, x.GhostSuffix))
            .ShouldEqual(
                new LoadOrderListing(TestConstants.LightModKey, enabled: true),
                new LoadOrderListing(TestConstants.MasterModKey, enabled: true),
                new LoadOrderListing(TestConstants.MasterModKey2, enabled: false),
                new LoadOrderListing(TestConstants.LightModKey3, enabled: true),
                new LoadOrderListing(TestConstants.LightModKey4, enabled: false),
                new LoadOrderListing(TestConstants.PluginModKey, enabled: true),
                new LoadOrderListing(TestConstants.PluginModKey2, enabled: false));
    }

    [Theory, MutagenAutoData]
    public void GetListings_CreationClubMissing(
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        var cccPath = Path.Combine(existingDir.Path, "Skyrim.ccc");
        var pluginsPath = Path.Combine(existingDir.Path, "Plugins.txt");
        var dataPath = Path.Combine(existingDir.Path, "Data");
        fileSystem.File.WriteAllLines(pluginsPath,
            new string[]
            {
                $"*{TestConstants.MasterModKey.FileName}",
                $"{TestConstants.MasterModKey2.FileName}",
                $"*{TestConstants.LightModKey3.FileName}",
                $"{TestConstants.LightModKey4.FileName}",
                $"*{TestConstants.PluginModKey.FileName}",
                $"{TestConstants.PluginModKey2.FileName}",
            });
        fileSystem.Directory.CreateDirectory(dataPath);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.MasterModKey.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.MasterModKey2.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.LightModKey3.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.LightModKey4.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.PluginModKey.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.PluginModKey2.FileName), string.Empty);
        var results = LoadOrder.GetLoadOrderListings(
                game: GameRelease.SkyrimSE,
                dataPath: dataPath,
                pluginsFilePath: pluginsPath,
                creationClubFilePath: cccPath,
                fileSystem: fileSystem)
            .ToList();
        results.ShouldHaveCount(6);
        results.ShouldEqual(
            new LoadOrderListing(TestConstants.MasterModKey, enabled: true),
            new LoadOrderListing(TestConstants.MasterModKey2, enabled: false),
            new LoadOrderListing(TestConstants.LightModKey3, enabled: true),
            new LoadOrderListing(TestConstants.LightModKey4, enabled: false),
            new LoadOrderListing(TestConstants.PluginModKey, enabled: true),
            new LoadOrderListing(TestConstants.PluginModKey2, enabled: false));
    }

    [Theory, MutagenAutoData]
    public void GetListings_NoCreationClub(
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        var pluginsPath = Path.Combine(existingDir.Path, "Plugins.txt");
        var dataPath = Path.Combine(existingDir.Path, "Data");
        fileSystem.File.WriteAllLines(pluginsPath,
            new string[]
            {
                $"{TestConstants.MasterModKey.FileName}",
                $"{TestConstants.PluginModKey.FileName}",
            });
        fileSystem.Directory.CreateDirectory(dataPath);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.MasterModKey.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.PluginModKey.FileName), string.Empty);
        var results = LoadOrder.GetLoadOrderListings(
                game: GameRelease.Oblivion,
                dataPath: dataPath,
                pluginsFilePath: pluginsPath,
                creationClubFilePath: null,
                fileSystem: fileSystem)
            .ToList();
        results.ShouldHaveCount(2);
        results.ShouldEqual(
            new LoadOrderListing(TestConstants.MasterModKey, enabled: true),
            new LoadOrderListing(TestConstants.PluginModKey, enabled: true));
    }

    /// <summary>
    /// Vortex lists creation club entries at the start of the plugins.txt, but leaves them marked
    /// as not active?
    /// </summary>
    [Theory, MutagenAutoData]
    public void GetListings_VortexCreationClub(
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        var cccPath = Path.Combine(existingDir.Path, "Skyrim.ccc");
        var pluginsPath = Path.Combine(existingDir.Path, "Plugins.txt");
        var dataPath = Path.Combine(existingDir.Path, "Data");
        fileSystem.File.WriteAllLines(cccPath,
            new string[]
            {
                TestConstants.LightModKey.FileName,
                TestConstants.LightModKey2.FileName,
            });
        fileSystem.File.WriteAllLines(pluginsPath,
            new string[]
            {
                TestConstants.LightModKey2.FileName,
                TestConstants.LightModKey.FileName,
                $"*{TestConstants.MasterModKey.FileName}",
                $"{TestConstants.MasterModKey2.FileName}",
                $"*{TestConstants.LightModKey3.FileName}",
                $"{TestConstants.LightModKey4.FileName}",
                $"*{TestConstants.PluginModKey.FileName}",
                $"{TestConstants.PluginModKey2.FileName}",
            });
        fileSystem.Directory.CreateDirectory(dataPath);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.LightModKey.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.LightModKey2.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.MasterModKey.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.MasterModKey2.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.LightModKey3.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.LightModKey4.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.PluginModKey.FileName), string.Empty);
        fileSystem.File.WriteAllText(Path.Combine(dataPath, TestConstants.PluginModKey2.FileName), string.Empty);
        var results = LoadOrder.GetLoadOrderListings(
                game: GameRelease.SkyrimSE,
                dataPath: dataPath,
                pluginsFilePath: pluginsPath,
                creationClubFilePath: cccPath,
                fileSystem: fileSystem)
            .ToList();
        results.ShouldHaveCount(8);
        results
            .Select(x => new LoadOrderListing(x.ModKey, x.Enabled, x.GhostSuffix))
            .ShouldEqual(
                new LoadOrderListing(TestConstants.LightModKey2, enabled: true),
                new LoadOrderListing(TestConstants.LightModKey, enabled: true),
                new LoadOrderListing(TestConstants.MasterModKey, enabled: true),
                new LoadOrderListing(TestConstants.MasterModKey2, enabled: false),
                new LoadOrderListing(TestConstants.LightModKey3, enabled: true),
                new LoadOrderListing(TestConstants.LightModKey4, enabled: false),
                new LoadOrderListing(TestConstants.PluginModKey, enabled: true),
                new LoadOrderListing(TestConstants.PluginModKey2, enabled: false));
    }

    [Fact]
    public void OrderListings()
    {
        ModKey baseEsm = new ModKey("Base", ModType.Master);
        ModKey baseEsm2 = new ModKey("Base2", ModType.Master);
        ModKey ccEsm = new ModKey("CC", ModType.Master);
        ModKey ccEsm2 = new ModKey("CC2", ModType.Master);
        ModKey ccEsl = new ModKey("CC", ModType.Light);
        ModKey ccEsl2 = new ModKey("CC2", ModType.Light);
        ModKey esm = new ModKey("Normal", ModType.Master);
        ModKey esm2 = new ModKey("Normal2", ModType.Master);
        ModKey esl = new ModKey("Normal", ModType.Light);
        ModKey esl2 = new ModKey("Normal2", ModType.Light);
        ModKey esp = new ModKey("Normal", ModType.Plugin);
        ModKey esp2 = new ModKey("Normal2", ModType.Plugin);

        var ordered = LoadOrder.OrderListings(
                implicitListings: new ModKey[]
                {
                    baseEsm,
                    baseEsm2,
                },
                creationClubListings: new ModKey[]
                {
                    ccEsl,
                    ccEsl2,
                    ccEsm,
                    ccEsm2,
                },
                pluginsListings: new ModKey[]
                {
                    esm,
                    esm2,
                    esl,
                    esl2,
                    esp,
                    esp2,
                },
                selector: m => m)
            .ToList();
        ordered.ShouldBe(new ModKey[]
        {
            baseEsm,
            baseEsm2,
            ccEsm,
            ccEsm2,
            ccEsl,
            ccEsl2,
            esm,
            esm2,
            esl,
            esl2,
            esp,
            esp2,
        });
    }

    [Fact]
    public void OrderListings_EnsurePluginListedCCsDriveOrder()
    {
        ModKey ccEsm = new ModKey("CC", ModType.Master);
        ModKey ccEsm2 = new ModKey("CC2", ModType.Master);
        ModKey ccEsm3 = new ModKey("CC3", ModType.Master);
        ModKey esm = new ModKey("Normal", ModType.Master);
        ModKey esm2 = new ModKey("Normal2", ModType.Master);

        LoadOrder.OrderListings(
                implicitListings: Array.Empty<ModKey>(),
                creationClubListings:
                [
                    ccEsm,
                    ccEsm2,
                    ccEsm3
                ],
                pluginsListings:
                [
                    ccEsm2,
                    esm,
                    ccEsm,
                    esm2
                ],
                selector: m => m)
            .ShouldEqual(ccEsm3, ccEsm2, ccEsm, esm, esm2);
    }

    [Theory, MutagenAutoData]
    public void WriteExclude(
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        var path = Path.Combine(existingDir.Path, "Plugins.txt");
        LoadOrder.Write(
            path,
            GameRelease.Oblivion,
            new LoadOrderListing[]
            {
                new LoadOrderListing(TestConstants.PluginModKey, false),
                new LoadOrderListing(TestConstants.PluginModKey2, true),
                new LoadOrderListing(TestConstants.PluginModKey3, false),
            },
            fileSystem: fileSystem);
        var lines = fileSystem.File.ReadAllLines(path).ToList();
        Assert.Single(lines);
        Assert.Equal(TestConstants.PluginModKey2.FileName, lines[0]);
    }

    [Theory, MutagenAutoData]
    public void WriteMarkers(
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        var path = Path.Combine(existingDir.Path, "Plugins.txt");
        LoadOrder.Write(
            path,
            GameRelease.SkyrimSE,
            new LoadOrderListing[]
            {
                new LoadOrderListing(TestConstants.PluginModKey, false),
                new LoadOrderListing(TestConstants.PluginModKey2, true),
                new LoadOrderListing(TestConstants.PluginModKey3, false),
            },
            fileSystem: fileSystem);
        var lines = fileSystem.File.ReadAllLines(path).ToList();
        Assert.Equal(3, lines.Count);
        Assert.Equal($"{TestConstants.PluginModKey.FileName}", lines[0]);
        Assert.Equal($"*{TestConstants.PluginModKey2.FileName}", lines[1]);
        Assert.Equal($"{TestConstants.PluginModKey3.FileName}", lines[2]);
    }

    [Theory, MutagenAutoData]
    public void WriteImplicitFilteredOut(
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        var path = Path.Combine(existingDir.Path, "Plugins.txt");
        LoadOrder.Write(
            path,
            GameRelease.SkyrimSE,
            new LoadOrderListing[]
            {
                new LoadOrderListing(TestConstants.Skyrim, true),
                new LoadOrderListing(TestConstants.PluginModKey, true),
                new LoadOrderListing(TestConstants.PluginModKey2, false),
            },
            removeImplicitMods: true,
            fileSystem: fileSystem);
        var lines = fileSystem.File.ReadAllLines(path).ToList();
        Assert.Equal(2, lines.Count);
        Assert.Equal($"*{TestConstants.PluginModKey.FileName}", lines[0]);
        Assert.Equal($"{TestConstants.PluginModKey2.FileName}", lines[1]);
    }

    [Theory, MutagenAutoData]
    public void WriteImplicit(
        IFileSystem fileSystem,
        DirectoryPath existingDir)
    {
        var path = Path.Combine(existingDir.Path, "Plugins.txt");
        LoadOrder.Write(
            path,
            GameRelease.SkyrimSE,
            new LoadOrderListing[]
            {
                new LoadOrderListing(TestConstants.Skyrim, true),
                new LoadOrderListing(TestConstants.PluginModKey, true),
                new LoadOrderListing(TestConstants.PluginModKey2, false),
            },
            removeImplicitMods: false,
            fileSystem: fileSystem);
        var lines = fileSystem.File.ReadAllLines(path).ToList();
        Assert.Equal(3, lines.Count);
        Assert.Equal($"*{TestConstants.Skyrim.FileName}", lines[0]);
        Assert.Equal($"*{TestConstants.PluginModKey.FileName}", lines[1]);
        Assert.Equal($"{TestConstants.PluginModKey2.FileName}", lines[2]);
    }

    #region HasMod
    [Fact]
    public void HasMod_Empty()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMod(TestConstants.LightModKey)
            .ShouldBeFalse();
    }

    [Fact]
    public void HasMod_Typical()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightModKey, true),
            new LoadOrderListing(TestConstants.LightModKey2, false),
            new LoadOrderListing(TestConstants.LightModKey3, true),
        };
        listings
            .ListsMod(TestConstants.LightModKey)
            .ShouldBeTrue();
        listings
            .ListsMod(TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ListsMod(TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ListsMod(TestConstants.LightModKey4)
            .ShouldBeFalse();
    }

    [Fact]
    public void HasMod_Enabled()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightModKey, true),
            new LoadOrderListing(TestConstants.LightModKey2, false),
            new LoadOrderListing(TestConstants.LightModKey3, true),
        };
        listings
            .ListsMod(TestConstants.LightModKey, enabled: true)
            .ShouldBeTrue();
        listings
            .ListsMod(TestConstants.LightModKey, enabled: false)
            .ShouldBeFalse();
        listings
            .ListsMod(TestConstants.LightModKey2, enabled: false)
            .ShouldBeTrue();
        listings
            .ListsMod(TestConstants.LightModKey2, enabled: true)
            .ShouldBeFalse();
        listings
            .ListsMod(TestConstants.LightModKey3, enabled: true)
            .ShouldBeTrue();
        listings
            .ListsMod(TestConstants.LightModKey3, enabled: false)
            .ShouldBeFalse();
    }

    [Fact]
    public void ListsMods_EmptyListings()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(TestConstants.LightModKey, TestConstants.LightModKey2)
            .ShouldBeFalse();
    }

    [Fact]
    public void ListsMods_EmptyInput()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods()
            .ShouldBeTrue();
    }

    [Fact]
    public void ListsMods_Single()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightModKey, true),
            new LoadOrderListing(TestConstants.LightModKey2, false),
            new LoadOrderListing(TestConstants.LightModKey3, true),
        };
        listings
            .ListsMods(TestConstants.LightModKey)
            .ShouldBeTrue();
        listings
            .ListsMods(TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ListsMods(TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ListsMods(TestConstants.LightModKey4)
            .ShouldBeFalse();
    }

    [Fact]
    public void ListsMods_Typical()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightModKey, true),
            new LoadOrderListing(TestConstants.LightModKey2, false),
            new LoadOrderListing(TestConstants.LightModKey3, true),
        };
        listings
            .ListsMods(TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ListsMods(TestConstants.LightModKey, TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ListsMods(TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey4)
            .ShouldBeFalse();
    }

    [Fact]
    public void ListsMods_Enabled_EmptyListings()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(true, TestConstants.LightModKey, TestConstants.LightModKey2)
            .ShouldBeFalse();
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(false, TestConstants.LightModKey, TestConstants.LightModKey2)
            .ShouldBeFalse();
    }

    [Fact]
    public void ListsMods_Enabled_EmptyInput()
    {
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(true)
            .ShouldBeTrue();
        Enumerable.Empty<LoadOrderListing>()
            .ListsMods(false)
            .ShouldBeTrue();
    }

    [Fact]
    public void ListsMods_Enabled_Single()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightModKey, true),
            new LoadOrderListing(TestConstants.LightModKey2, false),
            new LoadOrderListing(TestConstants.LightModKey3, true),
        };
        listings
            .ListsMods(true, TestConstants.LightModKey)
            .ShouldBeTrue();
        listings
            .ListsMods(false, TestConstants.LightModKey)
            .ShouldBeFalse();
        listings
            .ListsMods(false, TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ListsMods(true, TestConstants.LightModKey2)
            .ShouldBeFalse();
        listings
            .ListsMods(true, TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ListsMods(false, TestConstants.LightModKey3)
            .ShouldBeFalse();
        listings
            .ListsMods(true, TestConstants.LightModKey4)
            .ShouldBeFalse();
        listings
            .ListsMods(false, TestConstants.LightModKey4)
            .ShouldBeFalse();
    }

    [Fact]
    public void ListsMods_Enabled_Typical()
    {
        var listings = new LoadOrderListing[]
        {
            new LoadOrderListing(TestConstants.LightModKey, true),
            new LoadOrderListing(TestConstants.LightModKey2, false),
            new LoadOrderListing(TestConstants.LightModKey3, true),
        };
        listings
            .ListsMods(
                true,
                TestConstants.LightModKey, TestConstants.LightModKey3)
            .ShouldBeTrue();
        listings
            .ListsMods(false, TestConstants.LightModKey2)
            .ShouldBeTrue();
        listings
            .ListsMods(
                true,
                TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey3)
            .ShouldBeFalse();
        listings
            .ListsMods(
                true,
                TestConstants.LightModKey, TestConstants.LightModKey2, TestConstants.LightModKey4)
            .ShouldBeFalse();
    }
    #endregion
}