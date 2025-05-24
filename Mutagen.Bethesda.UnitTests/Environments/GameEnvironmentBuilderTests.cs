﻿using System.IO.Abstractions;
using AutoFixture.Xunit2;
using Shouldly;
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
using Noggog.Testing.Extensions;
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
                .ToPath(Path.Combine(dataDirectoryProvider.Path, m.FileName))
                .WithNoLoadOrder()
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

        env.GameRelease.ShouldBe(release);
        env.DataFolderPath.ShouldBe(dataDirectoryProvider.Path);
        env.LoadOrderFilePath.ShouldBe(pluginListingsPathProvider.Path);
        env.CreationClubListingsFilePath.ShouldBe(cccListingPathContext.Path);
        env.LoadOrder.Count.ShouldBe(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqual(true);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldBe(modKeys);
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

        env.GameRelease.ShouldBe(release);
        env.DataFolderPath.ShouldBe(dataDirectoryProvider.Path);
        env.LoadOrderFilePath.ShouldBe(pluginListingsPathProvider.Path);
        env.CreationClubListingsFilePath.ShouldBe(cccListingPathContext.Path);
        env.LoadOrder.Count.ShouldBe(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqual(true);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldBe(modKeys);
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

        env.DataFolderPath.ShouldBe(dataDirectoryProviderUse.Path);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
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

        env.DataFolderPath.ShouldBe(dataDirectoryProviderUse.Path);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
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

        env.DataFolderPath.ShouldBe(existingAltDataDir);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldAllBe(x => x == null);
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

        env.DataFolderPath.ShouldBe(existingAltDataDir);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldAllBe(x => x == null);
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

        env.LoadOrder.Count.ShouldBe(modKeysToUse.Length);
        env.LoadOrder.Select(x => x.Key).ShouldBe(modKeysToUse);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldBe(modKeysToUse);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqual(true);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldBe(modKeysToUse);
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

        env.LoadOrder.Count.ShouldBe(modKeysToUse.Length);
        env.LoadOrder.Select(x => x.Key).ShouldBe(modKeysToUse);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldBe(modKeysToUse);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqual(true);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldBe(modKeysToUse);
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

        env.LoadOrder.Count.ShouldBe(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqual(false);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldBe(modKeys);
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

        env.LoadOrder.Count.ShouldBe(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqual(false);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldBe(modKeys);
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

        env.LoadOrder.Count.ShouldBe(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqual(false);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldBe(modKeys);
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

        env.LoadOrder.Count.ShouldBe(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqual(false);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldBe(modKeys);
    }
    
    [Theory]
    [MutagenAutoData]
    public void TransformLoadOrderWithManualLoadOrder(
        IFileSystem fs,
        ModKey[] modKeys,
        ModKey[] modKeys2,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys.And(modKeys2).ToArray(), dataDirectoryProvider, pluginListingsPathProvider);

        using var env = GameEnvironment.Typical.Builder(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithLoadOrder(modKeys2)
            .TransformLoadOrderListings(x => x.Where(x => x.ModKey != modKeys2[1]))
            .Build();
        
        var expectedKeys = modKeys2
            .Where(x => x != modKeys2[1])
            .ToArray();

        env.LoadOrder.Count.ShouldBe(expectedKeys.Length);
        env.LoadOrder.Select(x => x.Key).ShouldBe(expectedKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldBe(expectedKeys);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldBe(expectedKeys);
    }
    
    [Theory]
    [MutagenAutoData]
    public void TransformLoadOrderWithManualLoadOrderGeneric(
        IFileSystem fs,
        ModKey[] modKeys,
        ModKey[] modKeys2,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys.And(modKeys2).ToArray(), dataDirectoryProvider, pluginListingsPathProvider);

        using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithLoadOrder(modKeys2)
            .TransformLoadOrderListings(x => x.Where(x => x.ModKey != modKeys2[1]))
            .Build();
        
        var expectedKeys = modKeys2
            .Where(x => x != modKeys2[1])
            .ToArray();

        env.LoadOrder.Count.ShouldBe(expectedKeys.Length);
        env.LoadOrder.Select(x => x.Key).ShouldBe(expectedKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldBe(expectedKeys);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldBe(expectedKeys);
    }
    
    [Theory]
    [MutagenAutoData]
    public void HardcodingLoadOrderWipesTransforms(
        IFileSystem fs,
        ModKey[] modKeys,
        ModKey[] modKeys2,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys.And(modKeys2).ToArray(), dataDirectoryProvider, pluginListingsPathProvider);

        using var env = GameEnvironment.Typical.Builder(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .TransformLoadOrderListings(x => x.Select(x => new LoadOrderListing($"ABC{x.ModKey}", x.Enabled)))
            .WithLoadOrder(modKeys2)
            .Build();
        
        env.LoadOrder.Count.ShouldBe(modKeys2.Length);
        env.LoadOrder.Select(x => x.Key).ShouldBe(modKeys2);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldBe(modKeys2);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldBe(modKeys2);
    }
    
    [Theory]
    [MutagenAutoData]
    public void HardcodingLoadOrderWipesTransformsGeneric(
        IFileSystem fs,
        ModKey[] modKeys,
        ModKey[] modKeys2,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        Setup(fs, release, modKeys.And(modKeys2).ToArray(), dataDirectoryProvider, pluginListingsPathProvider);

        using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .TransformLoadOrderListings(x => x.Select(x => new LoadOrderListing($"ABC{x.ModKey}", x.Enabled)))
            .WithLoadOrder(modKeys2)
            .Build();
        
        env.LoadOrder.Count.ShouldBe(modKeys2.Length);
        env.LoadOrder.Select(x => x.Key).ShouldBe(modKeys2);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldBe(modKeys2);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldBe(modKeys2);
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

        env.LoadOrder.Count.ShouldBe(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqual(true);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldBe(modKeys.And(outputModKey));
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

        env.LoadOrder.Count.ShouldBe(modKeys.Length);
        env.LoadOrder.Select(x => x.Key).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldBe(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqual(true);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldBe(modKeys.And(outputModKey));
    }
}