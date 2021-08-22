using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mutagen.Bethesda.Core.UnitTests;
using Mutagen.Bethesda.Testing;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
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

        [Fact]
        public void GetListings()
        {
            using var tmp = TestPathing.GetTempFolder(nameof(LoadOrderIntegrationTests));
            var cccPath = Path.Combine(tmp.Dir.Path, "Skyrim.ccc");
            var pluginsPath = Path.Combine(tmp.Dir.Path, "Plugins.txt");
            var dataPath = Path.Combine(tmp.Dir.Path, "Data");
            File.WriteAllLines(cccPath,
                new string[]
                {
                    TestConstants.LightMasterModKey.FileName,
                    TestConstants.LightMasterModKey2.FileName,
                });
            File.WriteAllLines(pluginsPath,
                new string[]
                {
                    $"*{TestConstants.MasterModKey.FileName}",
                    $"{TestConstants.MasterModKey2.FileName}",
                    $"*{TestConstants.LightMasterModKey3.FileName}",
                    $"{TestConstants.LightMasterModKey4.FileName}",
                    $"*{TestConstants.PluginModKey.FileName}",
                    $"{TestConstants.PluginModKey2.FileName}",
                });
            Directory.CreateDirectory(dataPath);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.LightMasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.MasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.MasterModKey2.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.LightMasterModKey3.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.LightMasterModKey4.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.PluginModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.PluginModKey2.FileName), string.Empty);
            var results = LoadOrder.GetListings(
                    game: GameRelease.SkyrimSE,
                    dataPath: dataPath,
                    pluginsFilePath: pluginsPath,
                    creationClubFilePath: cccPath)
                .ToList();
            results.Should().HaveCount(7);
            results.Should().Equal(new ModListing[]
            {
                new ModListing(TestConstants.LightMasterModKey, enabled: true),
                new ModListing(TestConstants.MasterModKey, enabled: true),
                new ModListing(TestConstants.MasterModKey2, enabled: false),
                new ModListing(TestConstants.LightMasterModKey3, enabled: true),
                new ModListing(TestConstants.LightMasterModKey4, enabled: false),
                new ModListing(TestConstants.PluginModKey, enabled: true),
                new ModListing(TestConstants.PluginModKey2, enabled: false),
            });
        }

        [Fact]
        public void GetListings_CreationClubMissing()
        {
            using var tmp = TestPathing.GetTempFolder(nameof(LoadOrderIntegrationTests));
            var cccPath = Path.Combine(tmp.Dir.Path, "Skyrim.ccc");
            var pluginsPath = Path.Combine(tmp.Dir.Path, "Plugins.txt");
            var dataPath = Path.Combine(tmp.Dir.Path, "Data");
            File.WriteAllLines(pluginsPath,
                new string[]
                {
                    $"*{TestConstants.MasterModKey.FileName}",
                    $"{TestConstants.MasterModKey2.FileName}",
                    $"*{TestConstants.LightMasterModKey3.FileName}",
                    $"{TestConstants.LightMasterModKey4.FileName}",
                    $"*{TestConstants.PluginModKey.FileName}",
                    $"{TestConstants.PluginModKey2.FileName}",
                });
            Directory.CreateDirectory(dataPath);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.MasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.MasterModKey2.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.LightMasterModKey3.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.LightMasterModKey4.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.PluginModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.PluginModKey2.FileName), string.Empty);
            var results = LoadOrder.GetListings(
                    game: GameRelease.SkyrimSE,
                    dataPath: dataPath,
                    pluginsFilePath: pluginsPath,
                    creationClubFilePath: cccPath)
                .ToList();
            results.Should().HaveCount(6);
            results.Should().Equal(new ModListing[]
            {
                new ModListing(TestConstants.MasterModKey, enabled: true),
                new ModListing(TestConstants.MasterModKey2, enabled: false),
                new ModListing(TestConstants.LightMasterModKey3, enabled: true),
                new ModListing(TestConstants.LightMasterModKey4, enabled: false),
                new ModListing(TestConstants.PluginModKey, enabled: true),
                new ModListing(TestConstants.PluginModKey2, enabled: false),
            });
        }

        [Fact]
        public void GetListings_NoCreationClub()
        {
            using var tmp = TestPathing.GetTempFolder(nameof(LoadOrderIntegrationTests));
            var pluginsPath = Path.Combine(tmp.Dir.Path, "Plugins.txt");
            var dataPath = Path.Combine(tmp.Dir.Path, "Data");
            File.WriteAllLines(pluginsPath,
                new string[]
                {
                    $"{TestConstants.MasterModKey.FileName}",
                    $"{TestConstants.PluginModKey.FileName}",
                });
            Directory.CreateDirectory(dataPath);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.MasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.PluginModKey.FileName), string.Empty);
            var results = LoadOrder.GetListings(
                    game: GameRelease.Oblivion,
                    dataPath: dataPath,
                    pluginsFilePath: pluginsPath,
                    creationClubFilePath: null)
                .ToList();
            results.Should().HaveCount(2);
            results.Should().Equal(new ModListing[]
            {
                new ModListing(TestConstants.MasterModKey, enabled: true),
                new ModListing(TestConstants.PluginModKey, enabled: true),
            });
        }

        /// <summary>
        /// Vortex lists creation club entries at the start of the plugins.txt, but leaves them marked
        /// as not active?
        /// </summary>
        [Fact]
        public void GetListings_VortexCreationClub()
        {
            using var tmp = TestPathing.GetTempFolder(nameof(LoadOrderIntegrationTests));
            var cccPath = Path.Combine(tmp.Dir.Path, "Skyrim.ccc");
            var pluginsPath = Path.Combine(tmp.Dir.Path, "Plugins.txt");
            var dataPath = Path.Combine(tmp.Dir.Path, "Data");
            File.WriteAllLines(cccPath,
                new string[]
                {
                    TestConstants.LightMasterModKey.FileName,
                    TestConstants.LightMasterModKey2.FileName,
                });
            File.WriteAllLines(pluginsPath,
                new string[]
                {
                    TestConstants.LightMasterModKey2.FileName,
                    TestConstants.LightMasterModKey.FileName,
                    $"*{TestConstants.MasterModKey.FileName}",
                    $"{TestConstants.MasterModKey2.FileName}",
                    $"*{TestConstants.LightMasterModKey3.FileName}",
                    $"{TestConstants.LightMasterModKey4.FileName}",
                    $"*{TestConstants.PluginModKey.FileName}",
                    $"{TestConstants.PluginModKey2.FileName}",
                });
            Directory.CreateDirectory(dataPath);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.LightMasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.LightMasterModKey2.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.MasterModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.MasterModKey2.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.LightMasterModKey3.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.LightMasterModKey4.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.PluginModKey.FileName), string.Empty);
            File.WriteAllText(Path.Combine(dataPath, TestConstants.PluginModKey2.FileName), string.Empty);
            var results = LoadOrder.GetListings(
                    game: GameRelease.SkyrimSE,
                    dataPath: dataPath,
                    pluginsFilePath: pluginsPath,
                    creationClubFilePath: cccPath)
                .ToList();
            results.Should().HaveCount(8);
            results.Should().Equal(new ModListing[]
            {
                new ModListing(TestConstants.LightMasterModKey2, enabled: true),
                new ModListing(TestConstants.LightMasterModKey, enabled: true),
                new ModListing(TestConstants.MasterModKey, enabled: true),
                new ModListing(TestConstants.MasterModKey2, enabled: false),
                new ModListing(TestConstants.LightMasterModKey3, enabled: true),
                new ModListing(TestConstants.LightMasterModKey4, enabled: false),
                new ModListing(TestConstants.PluginModKey, enabled: true),
                new ModListing(TestConstants.PluginModKey2, enabled: false),
            });
        }

        [Fact]
        public void OrderListings()
        {
            ModKey baseEsm = new ModKey("Base", ModType.Master);
            ModKey baseEsm2 = new ModKey("Base2", ModType.Master);
            ModKey ccEsm = new ModKey("CC", ModType.Master);
            ModKey ccEsm2 = new ModKey("CC2", ModType.Master);
            ModKey ccEsl = new ModKey("CC", ModType.LightMaster);
            ModKey ccEsl2 = new ModKey("CC2", ModType.LightMaster);
            ModKey esm = new ModKey("Normal", ModType.Master);
            ModKey esm2 = new ModKey("Normal2", ModType.Master);
            ModKey esl = new ModKey("Normal", ModType.LightMaster);
            ModKey esl2 = new ModKey("Normal2", ModType.LightMaster);
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
            ordered.Should().Equal(new ModKey[]
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
                creationClubListings: new ModKey[]
                {
                    ccEsm,
                    ccEsm2,
                    ccEsm3,
                },
                pluginsListings: new ModKey[]
                {
                    ccEsm2,
                    esm,
                    ccEsm,
                    esm2,
                },
                selector: m => m)
                .Should().Equal(new ModKey[]
                {
                    // First, because wasn't listed on plugins
                    ccEsm3,
                    // 2nd because it was first on the plugins listings
                    ccEsm2,
                    // Was listed last on the plugins listing
                    ccEsm,
                    esm,
                    esm2,
                });
        }

        [Fact]
        public void WriteExclude()
        {
            using var tmpFolder = TestPathing.GetTempFolder(nameof(LoadOrderIntegrationTests));
            var path = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            LoadOrder.Write(
                path,
                GameRelease.Oblivion,
                new ModListing[]
                {
                    new ModListing(TestConstants.PluginModKey, false),
                    new ModListing(TestConstants.PluginModKey2, true),
                    new ModListing(TestConstants.PluginModKey3, false),
                });
            var lines = File.ReadAllLines(path).ToList();
            Assert.Single(lines);
            Assert.Equal(TestConstants.PluginModKey2.FileName, lines[0]);
        }

        [Fact]
        public void WriteMarkers()
        {
            using var tmpFolder = TestPathing.GetTempFolder(nameof(LoadOrderIntegrationTests));
            var path = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            LoadOrder.Write(
                path,
                GameRelease.SkyrimSE,
                new ModListing[]
                {
                    new ModListing(TestConstants.PluginModKey, false),
                    new ModListing(TestConstants.PluginModKey2, true),
                    new ModListing(TestConstants.PluginModKey3, false),
                });
            var lines = File.ReadAllLines(path).ToList();
            Assert.Equal(3, lines.Count);
            Assert.Equal($"{TestConstants.PluginModKey.FileName}", lines[0]);
            Assert.Equal($"*{TestConstants.PluginModKey2.FileName}", lines[1]);
            Assert.Equal($"{TestConstants.PluginModKey3.FileName}", lines[2]);
        }

        [Fact]
        public void WriteImplicitFilteredOut()
        {
            using var tmpFolder = TestPathing.GetTempFolder(nameof(LoadOrderIntegrationTests));
            var path = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            LoadOrder.Write(
                path,
                GameRelease.SkyrimSE,
                new ModListing[]
                {
                    new ModListing(TestConstants.Skyrim, true),
                    new ModListing(TestConstants.PluginModKey, true),
                    new ModListing(TestConstants.PluginModKey2, false),
                },
                removeImplicitMods: true);
            var lines = File.ReadAllLines(path).ToList();
            Assert.Equal(2, lines.Count);
            Assert.Equal($"*{TestConstants.PluginModKey.FileName}", lines[0]);
            Assert.Equal($"{TestConstants.PluginModKey2.FileName}", lines[1]);
        }

        [Fact]
        public void WriteImplicit()
        {
            using var tmpFolder = TestPathing.GetTempFolder(nameof(LoadOrderIntegrationTests));
            var path = Path.Combine(tmpFolder.Dir.Path, "Plugins.txt");
            LoadOrder.Write(
                path,
                GameRelease.SkyrimSE,
                new ModListing[]
                {
                    new ModListing(TestConstants.Skyrim, true),
                    new ModListing(TestConstants.PluginModKey, true),
                    new ModListing(TestConstants.PluginModKey2, false),
                },
                removeImplicitMods: false);
            var lines = File.ReadAllLines(path).ToList();
            Assert.Equal(3, lines.Count);
            Assert.Equal($"*{TestConstants.Skyrim.FileName}", lines[0]);
            Assert.Equal($"*{TestConstants.PluginModKey.FileName}", lines[1]);
            Assert.Equal($"{TestConstants.PluginModKey2.FileName}", lines[2]);
        }

        #region HasMod
        [Fact]
        public void HasMod_Empty()
        {
            Enumerable.Empty<ModListing>()
                .HasMod(TestConstants.LightMasterModKey)
                .Should().BeFalse();
        }

        [Fact]
        public void HasMod_Typical()
        {
            var listings = new ModListing[]
            {
                new ModListing(TestConstants.LightMasterModKey, true),
                new ModListing(TestConstants.LightMasterModKey2, false),
                new ModListing(TestConstants.LightMasterModKey3, true),
            };
            listings
                .HasMod(TestConstants.LightMasterModKey)
                .Should().BeTrue();
            listings
                .HasMod(TestConstants.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMod(TestConstants.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMod(TestConstants.LightMasterModKey4)
                .Should().BeFalse();
        }

        [Fact]
        public void HasMod_Enabled()
        {
            var listings = new ModListing[]
            {
                new ModListing(TestConstants.LightMasterModKey, true),
                new ModListing(TestConstants.LightMasterModKey2, false),
                new ModListing(TestConstants.LightMasterModKey3, true),
            };
            listings
                .HasMod(TestConstants.LightMasterModKey, enabled: true)
                .Should().BeTrue();
            listings
                .HasMod(TestConstants.LightMasterModKey, enabled: false)
                .Should().BeFalse();
            listings
                .HasMod(TestConstants.LightMasterModKey2, enabled: false)
                .Should().BeTrue();
            listings
                .HasMod(TestConstants.LightMasterModKey2, enabled: true)
                .Should().BeFalse();
            listings
                .HasMod(TestConstants.LightMasterModKey3, enabled: true)
                .Should().BeTrue();
            listings
                .HasMod(TestConstants.LightMasterModKey3, enabled: false)
                .Should().BeFalse();
        }

        [Fact]
        public void HasMods_EmptyListings()
        {
            Enumerable.Empty<ModListing>()
                .HasMods(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
                .Should().BeFalse();
        }

        [Fact]
        public void HasMods_EmptyInput()
        {
            Enumerable.Empty<ModListing>()
                .HasMods()
                .Should().BeTrue();
        }

        [Fact]
        public void HasMods_Single()
        {
            var listings = new ModListing[]
            {
                new ModListing(TestConstants.LightMasterModKey, true),
                new ModListing(TestConstants.LightMasterModKey2, false),
                new ModListing(TestConstants.LightMasterModKey3, true),
            };
            listings
                .HasMods(TestConstants.LightMasterModKey)
                .Should().BeTrue();
            listings
                .HasMods(TestConstants.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMods(TestConstants.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMods(TestConstants.LightMasterModKey4)
                .Should().BeFalse();
        }

        [Fact]
        public void HasMods_Typical()
        {
            var listings = new ModListing[]
            {
                new ModListing(TestConstants.LightMasterModKey, true),
                new ModListing(TestConstants.LightMasterModKey2, false),
                new ModListing(TestConstants.LightMasterModKey3, true),
            };
            listings
                .HasMods(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMods(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMods(TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey4)
                .Should().BeFalse();
        }

        [Fact]
        public void HasMods_Enabled_EmptyListings()
        {
            Enumerable.Empty<ModListing>()
                .HasMods(true, TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
                .Should().BeFalse();
            Enumerable.Empty<ModListing>()
                .HasMods(false, TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2)
                .Should().BeFalse();
        }

        [Fact]
        public void HasMods_Enabled_EmptyInput()
        {
            Enumerable.Empty<ModListing>()
                .HasMods(true)
                .Should().BeTrue();
            Enumerable.Empty<ModListing>()
                .HasMods(false)
                .Should().BeTrue();
        }

        [Fact]
        public void HasMods_Enabled_Single()
        {
            var listings = new ModListing[]
            {
                new ModListing(TestConstants.LightMasterModKey, true),
                new ModListing(TestConstants.LightMasterModKey2, false),
                new ModListing(TestConstants.LightMasterModKey3, true),
            };
            listings
                .HasMods(true, TestConstants.LightMasterModKey)
                .Should().BeTrue();
            listings
                .HasMods(false, TestConstants.LightMasterModKey)
                .Should().BeFalse();
            listings
                .HasMods(false, TestConstants.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMods(true, TestConstants.LightMasterModKey2)
                .Should().BeFalse();
            listings
                .HasMods(true, TestConstants.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMods(false, TestConstants.LightMasterModKey3)
                .Should().BeFalse();
            listings
                .HasMods(true, TestConstants.LightMasterModKey4)
                .Should().BeFalse();
            listings
                .HasMods(false, TestConstants.LightMasterModKey4)
                .Should().BeFalse();
        }

        [Fact]
        public void HasMods_Enabled_Typical()
        {
            var listings = new ModListing[]
            {
                new ModListing(TestConstants.LightMasterModKey, true),
                new ModListing(TestConstants.LightMasterModKey2, false),
                new ModListing(TestConstants.LightMasterModKey3, true),
            };
            listings
                .HasMods(
                    true,
                    TestConstants.LightMasterModKey, TestConstants.LightMasterModKey3)
                .Should().BeTrue();
            listings
                .HasMods(false, TestConstants.LightMasterModKey2)
                .Should().BeTrue();
            listings
                .HasMods(
                    true,
                    TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey3)
                .Should().BeFalse();
            listings
                .HasMods(
                    true,
                    TestConstants.LightMasterModKey, TestConstants.LightMasterModKey2, TestConstants.LightMasterModKey4)
                .Should().BeFalse();
        }
        #endregion
    }
}
