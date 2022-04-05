using FluentAssertions;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing;
using Xunit;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.UnitTests.Archives;

public class CheckArchiveApplicabilityTests
{
    [Fact]
    public void Matches()
    {
        var release = GameRelease.Fallout4;
        new CheckArchiveApplicability(
                new ArchiveExtensionProvider(
                    new GameReleaseInjection(release)))
            .IsApplicable(
                TestConstants.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(TestConstants.PluginModKey.FileName)}{Archive.GetExtension(release)}")
            .Should().BeTrue();
    }

    [Fact]
    public void MatchesWithSuffix()
    {
        var release = GameRelease.Fallout4;
        new CheckArchiveApplicability(
                new ArchiveExtensionProvider(
                    new GameReleaseInjection(release)))
            .IsApplicable(
                TestConstants.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(TestConstants.PluginModKey.FileName)} - Textures{Archive.GetExtension(release)}")
            .Should().BeTrue();
    }

    [Fact]
    public void MatchesDespiteNameHavingDelimiter()
    {
        var release = GameRelease.Fallout4;
        var name = "SomeName - ExtraTitleNonsense";
        var modKey = ModKey.FromNameAndExtension($"{name}.esp");
        new CheckArchiveApplicability(
                new ArchiveExtensionProvider(
                    new GameReleaseInjection(release)))
            .IsApplicable(
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
        new CheckArchiveApplicability(
                new ArchiveExtensionProvider(
                    new GameReleaseInjection(release)))
            .IsApplicable(
                modKey,
                $"{name}{Archive.GetExtension(release)}")
            .Should().BeTrue();
    }

    [Fact]
    public void NoMatches()
    {
        var release = GameRelease.Fallout4;
        new CheckArchiveApplicability(
                new ArchiveExtensionProvider(
                    new GameReleaseInjection(release)))
            .IsApplicable(
                TestConstants.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(TestConstants.PluginModKey2.FileName)}{Archive.GetExtension(release)}")
            .Should().BeFalse();
    }

    [Fact]
    public void NoMatchesWithSuffix()
    {
        var release = GameRelease.Fallout4;
        new CheckArchiveApplicability(
                new ArchiveExtensionProvider(
                    new GameReleaseInjection(release)))
            .IsApplicable(
                TestConstants.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(TestConstants.PluginModKey2.FileName)} - Textures{Archive.GetExtension(release)}")
            .Should().BeFalse();
    }

    [Fact]
    public void BadExtension()
    {
        new CheckArchiveApplicability(
                new ArchiveExtensionProvider(
                    new GameReleaseInjection(GameRelease.Fallout4)))
            .IsApplicable(
                TestConstants.PluginModKey,
                $"{Path.GetFileNameWithoutExtension(TestConstants.PluginModKey.FileName)}.bad")
            .Should().BeFalse();
    }
}