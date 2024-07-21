using System.IO.Abstractions;
using AutoFixture.Xunit2;
using FluentAssertions;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Environments;

public class GameEnvironmentBuilderTests
{
    private void Setup(
        IFileSystem fs,
        GameRelease release,
        ModKey[] modKeys,
        IDataDirectoryProvider dataDirectoryProvider,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        LoadOrder.Write(
            pluginListingsPathProvider.Path,
            release,
            modKeys.Select((x, i) => new LoadOrderListing(x, enabled: true)),
            fileSystem: fs);
        modKeys.ForEach(m =>
        {
            var mod = new SkyrimMod(m, SkyrimRelease.SkyrimSE);
            mod.BeginWrite
                .WithNoLoadOrder()
                .ToPath(Path.Combine(dataDirectoryProvider.Path, m.FileName))
                .WithFileSystem(fs)
                .Write();
        });
    }
    
    [Theory]
    [MutagenAutoData]
    public void Typical(
        IFileSystem fs,
        ModKey[] modKeys,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys, dataDirectoryProvider, pluginListingsPathProvider);
        
        using var env = GameEnvironment.Typical.Builder(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .Build();

        env.GameRelease.Should().Be(release);
        env.DataFolderPath.Should().Be(dataDirectoryProvider.Path);
        env.LoadOrderFilePath.Should().Be(pluginListingsPathProvider.Path);
        env.CreationClubListingsFilePath.Should().Be(cccListingPathContext.Path);
        env.LoadOrder.Count.Should().Be(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().Should().Equal(true);
        env.LoadOrder.Select(x => x.Value.Mod).Should().NotContainNulls();
        env.LinkCache.ListedOrder.Select(x => x.ModKey).Should().Equal(modKeys);
    }
    
    [Theory]
    [MutagenAutoData]
    public void TypicalGeneric(
        IFileSystem fs,
        ModKey[] modKeys,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys, dataDirectoryProvider, pluginListingsPathProvider);
        
        using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .Build();

        env.GameRelease.Should().Be(release);
        env.DataFolderPath.Should().Be(dataDirectoryProvider.Path);
        env.LoadOrderFilePath.Should().Be(pluginListingsPathProvider.Path);
        env.CreationClubListingsFilePath.Should().Be(cccListingPathContext.Path);
        env.LoadOrder.Count.Should().Be(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().Should().Equal(true);
        env.LoadOrder.Select(x => x.Value.Mod).Should().NotContainNulls();
        env.LinkCache.ListedOrder.Select(x => x.ModKey).Should().Equal(modKeys);
    }
    
    [Theory]
    [MutagenAutoData]
    public void WithDataDirectory(
        IFileSystem fs,
        ModKey[] modKeys,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        DirectoryPath existingAltDataDir,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        var dataDirectoryProviderUse = Substitute.For<IDataDirectoryProvider>();
        dataDirectoryProviderUse.Path.Returns(existingAltDataDir);
        
        Setup(fs, release, modKeys, dataDirectoryProviderUse, pluginListingsPathProvider);
        
        using var env = GameEnvironment.Typical.Builder(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithTargetDataFolder(dataDirectoryProviderUse.Path)
            .Build();

        env.DataFolderPath.Should().Be(dataDirectoryProviderUse.Path);
        env.LoadOrder.Select(x => x.Value.Mod).Should().NotContainNulls();
    }
    
    [Theory]
    [MutagenAutoData]
    public void WithDataDirectoryGeneric(
        IFileSystem fs,
        ModKey[] modKeys,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        DirectoryPath existingAltDataDir,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        var dataDirectoryProviderUse = Substitute.For<IDataDirectoryProvider>();
        dataDirectoryProviderUse.Path.Returns(existingAltDataDir);
        
        Setup(fs, release, modKeys, dataDirectoryProviderUse, pluginListingsPathProvider);
        
        using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithTargetDataFolder(dataDirectoryProviderUse.Path)
            .Build();

        env.DataFolderPath.Should().Be(dataDirectoryProviderUse.Path);
        env.LoadOrder.Select(x => x.Value.Mod).Should().NotContainNulls();
    }
    
    [Theory]
    [MutagenAutoData]
    public void WithMisalignedDataDirectory(
        IFileSystem fs,
        ModKey[] modKeys,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        DirectoryPath existingAltDataDir,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys, dataDirectoryProvider, pluginListingsPathProvider);

        using var env = GameEnvironment.Typical.Builder(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithTargetDataFolder(existingAltDataDir)
            .Build();

        env.DataFolderPath.Should().Be(existingAltDataDir);
        env.LoadOrder.Select(x => x.Value.Mod).Should().AllSatisfy(x => x.Should().BeNull());
    }
    
    [Theory]
    [MutagenAutoData]
    public void WithMisalignedDataDirectoryGeneric(
        IFileSystem fs,
        ModKey[] modKeys,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        DirectoryPath existingAltDataDir,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys, dataDirectoryProvider, pluginListingsPathProvider);
        
        using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithTargetDataFolder(existingAltDataDir)
            .Build();

        env.DataFolderPath.Should().Be(existingAltDataDir);
        env.LoadOrder.Select(x => x.Value.Mod).Should().AllSatisfy(x => x.Should().BeNull());
    }
    
    [Theory]
    [MutagenAutoData]
    public void WithLoadOrderModKeys(
        IFileSystem fs,
        ModKey[] modKeys,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys, dataDirectoryProvider, pluginListingsPathProvider);

        var modKeysToUse = modKeys.Take(2).ToArray();
        
        using var env = GameEnvironment.Typical.Builder(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithLoadOrder(modKeysToUse)
            .Build();

        env.LoadOrder.Count.Should().Be(modKeysToUse.Length);
        env.LoadOrder.Select(x => x.Key).Should().Equal(modKeysToUse);
        env.LoadOrder.Select(x => x.Value.ModKey).Should().Equal(modKeysToUse);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().Should().Equal(true);
        env.LoadOrder.Select(x => x.Value.Mod).Should().NotContainNulls();
        env.LinkCache.ListedOrder.Select(x => x.ModKey).Should().Equal(modKeysToUse);
    }
    
    [Theory]
    [MutagenAutoData]
    public void WithLoadOrderModKeysGeneric(
        IFileSystem fs,
        ModKey[] modKeys,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys, dataDirectoryProvider, pluginListingsPathProvider);

        var modKeysToUse = modKeys.Take(2).ToArray();
        
        using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithLoadOrder(modKeysToUse)
            .Build();

        env.LoadOrder.Count.Should().Be(modKeysToUse.Length);
        env.LoadOrder.Select(x => x.Key).Should().Equal(modKeysToUse);
        env.LoadOrder.Select(x => x.Value.ModKey).Should().Equal(modKeysToUse);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().Should().Equal(true);
        env.LoadOrder.Select(x => x.Value.Mod).Should().NotContainNulls();
        env.LinkCache.ListedOrder.Select(x => x.ModKey).Should().Equal(modKeysToUse);
    }
    
    [Theory]
    [MutagenAutoData]
    public void WithLoadOrderListings(
        IFileSystem fs,
        ModKey[] modKeys,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys, dataDirectoryProvider, pluginListingsPathProvider);

        var listings = modKeys.Select(x => new LoadOrderListing(x, enabled: false)).ToArray();
        
        using var env = GameEnvironment.Typical.Builder(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithLoadOrder(listings)
            .Build();

        env.LoadOrder.Count.Should().Be(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().Should().Equal(false);
        env.LoadOrder.Select(x => x.Value.Mod).Should().NotContainNulls();
        env.LinkCache.ListedOrder.Select(x => x.ModKey).Should().Equal(modKeys);
    }
    
    [Theory]
    [MutagenAutoData]
    public void WithLoadOrderListingsGeneric(
        IFileSystem fs,
        ModKey[] modKeys,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys, dataDirectoryProvider, pluginListingsPathProvider);

        var listings = modKeys.Select(x => new LoadOrderListing(x, enabled: false)).ToArray();
        
        using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithLoadOrder(listings)
            .Build();

        env.LoadOrder.Count.Should().Be(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().Should().Equal(false);
        env.LoadOrder.Select(x => x.Value.Mod).Should().NotContainNulls();
        env.LinkCache.ListedOrder.Select(x => x.ModKey).Should().Equal(modKeys);
    }
    
    [Theory]
    [MutagenAutoData]
    public void TransformLoadOrder(
        IFileSystem fs,
        ModKey[] modKeys,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys, dataDirectoryProvider, pluginListingsPathProvider);
        
        using var env = GameEnvironment.Typical.Builder(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .TransformModListings(x => x.Select(x => new ModListing<IModGetter>(x.ModKey, x.Mod, enabled: false)))
            .Build();

        env.LoadOrder.Count.Should().Be(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().Should().Equal(false);
        env.LoadOrder.Select(x => x.Value.Mod).Should().NotContainNulls();
        env.LinkCache.ListedOrder.Select(x => x.ModKey).Should().Equal(modKeys);
    }
    
    [Theory]
    [MutagenAutoData]
    public void TransformLoadOrderGeneric(
        IFileSystem fs,
        ModKey[] modKeys,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys, dataDirectoryProvider, pluginListingsPathProvider);

        using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .TransformModListings(x => x.Select(x => new ModListing<ISkyrimModGetter>(x.ModKey, x.Mod, enabled: false)))
            .Build();

        env.LoadOrder.Count.Should().Be(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().Should().Equal(false);
        env.LoadOrder.Select(x => x.Value.Mod).Should().NotContainNulls();
        env.LinkCache.ListedOrder.Select(x => x.ModKey).Should().Equal(modKeys);
    }
    
    [Theory]
    [MutagenAutoData]
    public void WithOutputMod(
        IFileSystem fs,
        ModKey outputModKey,
        ModKey[] modKeys,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys, dataDirectoryProvider, pluginListingsPathProvider);

        var outputMod = new SkyrimMod(outputModKey, SkyrimRelease.SkyrimSE);
        
        using var env = GameEnvironment.Typical.Builder(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithOutputMod(outputMod)
            .Build();

        env.LoadOrder.Count.Should().Be(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().Should().Equal(true);
        env.LoadOrder.Select(x => x.Value.Mod).Should().NotContainNulls();
        env.LinkCache.ListedOrder.Select(x => x.ModKey).Should().Equal(modKeys.And(outputModKey));
    }
    
    [Theory]
    [MutagenAutoData]
    public void WithOutputModGeneric(
        IFileSystem fs,
        ModKey outputModKey,
        ModKey[] modKeys,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys, dataDirectoryProvider, pluginListingsPathProvider);

        var outputMod = new SkyrimMod(outputModKey, SkyrimRelease.SkyrimSE);

        using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithOutputMod(outputMod)
            .Build();

        env.LoadOrder.Count.Should().Be(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).Should().Equal(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().Should().Equal(true);
        env.LoadOrder.Select(x => x.Value.Mod).Should().NotContainNulls();
        env.LinkCache.ListedOrder.Select(x => x.ModKey).Should().Equal(modKeys.And(outputModKey));
    }
}