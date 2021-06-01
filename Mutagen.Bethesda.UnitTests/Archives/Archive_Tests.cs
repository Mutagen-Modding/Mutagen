using System;
using FluentAssertions;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Inis;
using Mutagen.Bethesda.Plugins;
using Noggog;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace Mutagen.Bethesda.UnitTests.Archives
{
    public class Archive_Tests
    {
        private void SetUpIni()
        {
            Archive.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { Ini.GetTypicalPath(GameRelease.SkyrimSE).Path, new MockFileData(@"[Archive]
sResourceArchiveList=SomeExplicitListing.bsa, SomeExplicitListing2.bsa") }
            });
        }

        #region GetApplicableArchivePaths
        [Fact]
        public void GetApplicableArchivePaths_Empty()
        {
            SetUpIni();
            using var temp = Utility.GetTempFolder(nameof(Archive_Tests));
            Archive.GetApplicableArchivePaths(GameRelease.SkyrimSE, temp.Dir.Path, Utility.Skyrim, Enumerable.Empty<FileName>())
                .Should().BeEmpty();
        }

        [Fact]
        public void GetApplicableArchivePaths_BaseMod_Unordered()
        {
            SetUpIni();
            using var temp = Utility.GetTempFolder(nameof(Archive_Tests));
            File.WriteAllText(Path.Combine(temp.Dir.Path, "Skyrim.bsa"), string.Empty);
            File.WriteAllText(Path.Combine(temp.Dir.Path, "SomeExplicitListing.bsa"), string.Empty);
            File.WriteAllText(Path.Combine(temp.Dir.Path, "MyMod.bsa"), string.Empty);
            var applicable = Archive.GetApplicableArchivePaths(GameRelease.SkyrimSE, temp.Dir.Path, Utility.Skyrim, Enumerable.Empty<FileName>())
                .ToArray();
            applicable.Should().BeEquivalentTo(new FilePath[]
            {
                Path.Combine(temp.Dir.Path, "Skyrim.bsa"),
                Path.Combine(temp.Dir.Path, "SomeExplicitListing.bsa")
            });
        }

        [Fact]
        public void GetApplicableArchivePaths_Typical_Unordered()
        {
            SetUpIni();
            var temp = Utility.GetTempFolder(nameof(Archive_Tests));
            File.WriteAllText(Path.Combine(temp.Dir.Path, $"{Utility.MasterModKey2.Name}.bsa"), string.Empty);
            File.WriteAllText(Path.Combine(temp.Dir.Path, "SomeExplicitListing.bsa"), string.Empty);
            File.WriteAllText(Path.Combine(temp.Dir.Path, "MyMod.bsa"), string.Empty);
            var applicable = Archive.GetApplicableArchivePaths(GameRelease.SkyrimSE, temp.Dir.Path, Utility.MasterModKey2, Enumerable.Empty<FileName>())
                .ToArray();
            applicable.Should().BeEquivalentTo(new FilePath[]
            {
                Path.Combine(temp.Dir.Path, $"{Utility.MasterModKey2.Name}.bsa"),
                Path.Combine(temp.Dir.Path, "SomeExplicitListing.bsa")
            });
        }

        [Fact]
        public void GetApplicableArchivePaths_BaseMod_Ordered()
        {
            SetUpIni();
            var temp = Utility.GetTempFolder(nameof(Archive_Tests));
            File.WriteAllText(Path.Combine(temp.Dir.Path, "Skyrim.bsa"), string.Empty);
            File.WriteAllText(Path.Combine(temp.Dir.Path, "SomeExplicitListing.bsa"), string.Empty);
            File.WriteAllText(Path.Combine(temp.Dir.Path, "MyMod.bsa"), string.Empty);
            var applicable = Archive.GetApplicableArchivePaths(GameRelease.SkyrimSE, temp.Dir.Path, Utility.Skyrim)
                .ToArray();
            applicable.Should().BeEquivalentTo(new FilePath[]
            {
                Path.Combine(temp.Dir.Path, "SomeExplicitListing.bsa"),
                Path.Combine(temp.Dir.Path, "Skyrim.bsa"),
            });
        }

        [Fact]
        public void GetApplicableArchivePaths_Typical_Ordered()
        {
            SetUpIni();
            var temp = Utility.GetTempFolder(nameof(Archive_Tests));
            File.WriteAllText(Path.Combine(temp.Dir.Path, $"{Utility.MasterModKey2.Name}.bsa"), string.Empty);
            File.WriteAllText(Path.Combine(temp.Dir.Path, "SomeExplicitListing.bsa"), string.Empty);
            File.WriteAllText(Path.Combine(temp.Dir.Path, "MyMod.bsa"), string.Empty);
            var applicable = Archive.GetApplicableArchivePaths(GameRelease.SkyrimSE, temp.Dir.Path, Utility.MasterModKey2)
                .ToArray();
            applicable.Should().BeEquivalentTo(new FilePath[]
            {
                Path.Combine(temp.Dir.Path, "SomeExplicitListing.bsa"),
                Path.Combine(temp.Dir.Path, $"{Utility.MasterModKey2.Name}.bsa"),
            });
        }
        #endregion

        #region IsApplicable
        [Fact]
        public void IsApplicable_Matches()
        {
            var release = GameRelease.Fallout4;
            Archive.IsApplicable(
                release,
                Utility.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(Utility.PluginModKey.FileName)}{Archive.GetExtension(release)}")
                .Should().BeTrue();
        }

        [Fact]
        public void IsApplicable_MatchesWithSuffix()
        {
            var release = GameRelease.Fallout4;
            Archive.IsApplicable(
                release,
                Utility.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(Utility.PluginModKey.FileName)} - Textures{Archive.GetExtension(release)}")
                .Should().BeTrue();
        }

        [Fact]
        public void IsApplicable_MatchesDespiteNameHavingDelimiter()
        {
            var release = GameRelease.Fallout4;
            var name = "SomeName - ExtraTitleNonsense";
            var modKey = ModKey.FromNameAndExtension($"{name}.esp");
            Archive.IsApplicable(
                    release,
                    modKey,
                    $"{name}{Archive.GetExtension(release)}")
                .Should().BeTrue();
        }

        [Fact]
        public void ModNameIncludesSpecialSuffix()
        {
            var release = GameRelease.Fallout4;
            var name = $"SomeName - Textures";
            var modKey = ModKey.FromNameAndExtension($"{name}.esp");
            Archive.IsApplicable(
                    release,
                    modKey,
                    $"{name}{Archive.GetExtension(release)}")
                .Should().BeTrue();
        }

        [Fact]
        public void IsApplicable_NoMatches()
        {
            var release = GameRelease.Fallout4;
            Archive.IsApplicable(
                release,
                Utility.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(Utility.PluginModKey2.FileName)}{Archive.GetExtension(release)}")
                .Should().BeFalse();
        }

        [Fact]
        public void IsApplicable_NoMatchesWithSuffix()
        {
            var release = GameRelease.Fallout4;
            Archive.IsApplicable(
                release,
                Utility.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(Utility.PluginModKey2.FileName)} - Textures{Archive.GetExtension(release)}")
                .Should().BeFalse();
        }

        [Fact]
        public void IsApplicable_BadExtension()
        {
            Archive.IsApplicable(
                GameRelease.Fallout4,
                Utility.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(Utility.PluginModKey.FileName)}.bad")
                .Should().BeFalse();
        }
        #endregion

        #region GetIniListings
        [Fact]
        public void GetIniListings_Typical()
        {
            Archive.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { Ini.GetTypicalPath(GameRelease.SkyrimSE).Path, new MockFileData(@"[Archive]
sResourceArchiveList=Skyrim - Misc.bsa, Skyrim - Shaders.bsa
sResourceArchiveList2=Skyrim - Voices_en0.bsa, Skyrim - Textures0.bsa") }
            });

            Archive.GetIniListings(GameRelease.SkyrimSE, Ini.GetTypicalPath(GameRelease.SkyrimSE))
                .Should().BeEquivalentTo(new FileName[]
                {
                    "Skyrim - Misc.bsa",
                    "Skyrim - Shaders.bsa",
                    "Skyrim - Voices_en0.bsa",
                    "Skyrim - Textures0.bsa",
                });
        }
        #endregion
    }
}
