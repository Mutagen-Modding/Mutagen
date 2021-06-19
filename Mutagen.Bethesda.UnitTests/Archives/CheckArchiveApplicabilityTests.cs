using FluentAssertions;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Plugins;
using Xunit;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Archives
{
    public class CheckArchiveApplicabilityTests
    {
        [Fact]
        public void Matches()
        {
            var release = GameRelease.Fallout4;
            Archive.IsApplicable(
                release,
                Utility.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(Utility.PluginModKey.FileName)}{Archive.GetExtension(release)}")
                .Should().BeTrue();
        }

        [Fact]
        public void MatchesWithSuffix()
        {
            var release = GameRelease.Fallout4;
            Archive.IsApplicable(
                release,
                Utility.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(Utility.PluginModKey.FileName)} - Textures{Archive.GetExtension(release)}")
                .Should().BeTrue();
        }

        [Fact]
        public void MatchesDespiteNameHavingDelimiter()
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
        public void NoMatches()
        {
            var release = GameRelease.Fallout4;
            Archive.IsApplicable(
                release,
                Utility.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(Utility.PluginModKey2.FileName)}{Archive.GetExtension(release)}")
                .Should().BeFalse();
        }

        [Fact]
        public void NoMatchesWithSuffix()
        {
            var release = GameRelease.Fallout4;
            Archive.IsApplicable(
                release,
                Utility.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(Utility.PluginModKey2.FileName)} - Textures{Archive.GetExtension(release)}")
                .Should().BeFalse();
        }

        [Fact]
        public void BadExtension()
        {
            Archive.IsApplicable(
                GameRelease.Fallout4,
                Utility.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(Utility.PluginModKey.FileName)}.bad")
                .Should().BeFalse();
        }
    }
}