using System.IO.Abstractions;
using AutoFixture.Xunit2;
using FluentAssertions;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.AutoData;

public class GameEnvironmentBuilderTests
{
    [Theory]
    [MutagenAutoData(GameRelease.SkyrimSE)]
    public void PluginPathCreated(
        IFileSystem fs,
        [Frozen] IPluginListingsPathContext path,
        IGameEnvironment env)
    {
        fs.File.Exists(path.Path);
        env.LoadOrderFilePath.Should().Be(path.Path);
    }
    
    [Theory]
    [MutagenAutoData(GameRelease.SkyrimSE)]
    public void PluginPathCreatedGeneric(
        IFileSystem fs,
        [Frozen] IPluginListingsPathContext path,
        IGameEnvironment<ISkyrimMod, ISkyrimModGetter> env)
    {
        fs.File.Exists(path.Path);
        env.LoadOrderFilePath.Should().Be(path.Path);
    }
    
    [Theory]
    [MutagenAutoData(GameRelease.SkyrimSE)]
    public void DataDirectoryCreated(
        IFileSystem fs,
        [Frozen] IDataDirectoryProvider path,
        IGameEnvironment env)
    {
        fs.Directory.Exists(path.Path);
        env.DataFolderPath.Should().Be(path.Path);
    }
    
    [Theory]
    [MutagenAutoData(GameRelease.SkyrimSE)]
    public void DataDirectoryGeneric(
        IFileSystem fs,
        [Frozen] IDataDirectoryProvider path,
        IGameEnvironment<ISkyrimMod, ISkyrimModGetter> env)
    {
        fs.Directory.Exists(path.Path);
        env.DataFolderPath.Should().Be(path.Path);
    }
    
    [Theory]
    [MutagenAutoData(GameRelease.SkyrimSE)]
    public void CCPathMatches(
        IFileSystem fs,
        [Frozen] ICreationClubListingsPathProvider path,
        IGameEnvironment env)
    {
        env.CreationClubListingsFilePath.Should().Be(path.Path);
    }
    
    [Theory]
    [MutagenAutoData(GameRelease.SkyrimSE)]
    public void CCPathMatchesGeneric(
        IFileSystem fs,
        [Frozen] ICreationClubListingsPathProvider path,
        IGameEnvironment<ISkyrimMod, ISkyrimModGetter> env)
    {
        env.CreationClubListingsFilePath.Should().Be(path.Path);
    }
    
    [Theory]
    [MutagenAutoData(GameRelease.SkyrimSE)]
    public void GameReleaseShouldMatch(
        IGameEnvironment env)
    {
        env.GameRelease.Should().Be(GameRelease.SkyrimSE);
    }
    
    [Theory]
    [MutagenAutoData(GameRelease.SkyrimSE)]
    public void GameReleaseShouldMatchGeneric(
        IGameEnvironment<ISkyrimMod, ISkyrimModGetter> env)
    {
        env.GameRelease.Should().Be(GameRelease.SkyrimSE);
    }
    
    [Theory]
    [MutagenAutoData(GameRelease.SkyrimSE)]
    public void LoadOrderContents(
        IGameEnvironment env)
    {
        env.LoadOrder.Should().NotBeEmpty();
        env.LoadOrder.ListedOrder.Select(x => x.Mod).Should().AllSatisfy(x => x.Should().NotBeNull());
    }
    
    [Theory]
    [MutagenAutoData(GameRelease.SkyrimSE)]
    public void LoadOrderContentsGeneric(
        IGameEnvironment<ISkyrimMod, ISkyrimModGetter> env)
    {
        env.LoadOrder.Should().NotBeEmpty();
        env.LoadOrder.ListedOrder.Select(x => x.Mod).Should().AllSatisfy(x => x.Should().NotBeNull());
    }
}