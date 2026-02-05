using System.IO.Abstractions;
using System.Text;
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
using Mutagen.Bethesda.Strings.DI;
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
        env.LoadOrder.Select(x => x.Key).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqualEnumerable(true);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldEqualEnumerable(modKeys);
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
        env.LoadOrder.Select(x => x.Key).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqualEnumerable(true);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldEqualEnumerable(modKeys);
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
        env.LoadOrder.Select(x => x.Key).ShouldEqualEnumerable(modKeysToUse);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldEqualEnumerable(modKeysToUse);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqualEnumerable(true);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldEqualEnumerable(modKeysToUse);
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
        env.LoadOrder.Select(x => x.Key).ShouldEqualEnumerable(modKeysToUse);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldEqualEnumerable(modKeysToUse);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqualEnumerable(true);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldEqualEnumerable(modKeysToUse);
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
        env.LoadOrder.Select(x => x.Key).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqualEnumerable(false);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldEqualEnumerable(modKeys);
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
        env.LoadOrder.Select(x => x.Key).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqualEnumerable(false);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldEqualEnumerable(modKeys);
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
        env.LoadOrder.Select(x => x.Key).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqualEnumerable(false);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldEqualEnumerable(modKeys);
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
        env.LoadOrder.Select(x => x.Key).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqualEnumerable(false);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldEqualEnumerable(modKeys);
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
        env.LoadOrder.Select(x => x.Key).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqualEnumerable(true);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldEqualEnumerable(modKeys.And(outputModKey));
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
        env.LoadOrder.Select(x => x.Key).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.ModKey).ShouldEqualEnumerable(modKeys);
        env.LoadOrder.Select(x => x.Value.Enabled).Distinct().ShouldEqualEnumerable(true);
        env.LoadOrder.Select(x => x.Value.Mod).ShouldNotContain(x => x == null);
        env.LinkCache.ListedOrder.Select(x => x.ModKey).ShouldEqualEnumerable(modKeys.And(outputModKey));
    }

    [Theory]
    [MutagenAutoData]
    public void ExtraDataOwnerFromAnotherMod(
        IFileSystem fs,
        DirectoryPath existingDataDir,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        // Create master mod with an NPC
        var masterModKey = new ModKey("TestMaster", ModType.Master);
        var masterMod = new SkyrimMod(masterModKey, SkyrimRelease.SkyrimSE);
        var npc = masterMod.Npcs.AddNew("TestNPC");
        var masterPath = Path.Combine(existingDataDir, masterModKey.FileName);
        masterMod.WriteToBinary(masterPath, new BinaryWriteParameters()
        {
            FileSystem = fs
        });

        // Create plugin mod that references the NPC from master via ExtraData
        var pluginModKey = new ModKey("TestPlugin", ModType.Plugin);
        var pluginMod = new SkyrimMod(pluginModKey, SkyrimRelease.SkyrimSE);

        // Add a container with an entry that has ExtraData with NpcOwner
        var container = pluginMod.Containers.AddNew("TestContainer");
        container.Items = new Noggog.ExtendedList<ContainerEntry>();
        var entry = new ContainerEntry
        {
            Item = new ContainerItem
            {
                Item = new FormLink<IItemGetter>(npc.FormKey),
                Count = 1
            },
            Data = new ExtraData
            {
                Owner = new NpcOwner
                {
                    Npc = new FormLink<INpcGetter>(npc.FormKey),
                    Global = new FormLink<IGlobalGetter>()
                }
            }
        };
        container.Items.Add(entry);

        var pluginPath = Path.Combine(existingDataDir, pluginModKey.FileName);
        pluginMod.BeginWrite
            .ToPath(pluginPath)
            .WithLoadOrder(masterMod)
            .WithFileSystem(fs)
            .Write();

        var dataDirectoryProvider = new DataDirectoryInjection(existingDataDir);

        // Build GameEnvironment
        using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithLoadOrder(masterModKey, pluginModKey)
            .Build();

        // Verify we can access the ExtraData.Owner field
        var loadedPlugin = env.LoadOrder[pluginModKey].Mod;
        loadedPlugin.ShouldNotBeNull();

        var loadedContainer = loadedPlugin!.Containers.First();
        var loadedEntry = loadedContainer.Items.First();
        var owner = loadedEntry.Data?.Owner;

        // With the LinkCache available, this should correctly resolve to NpcOwner
        owner.ShouldNotBeNull();
        owner.ShouldBeOfType<NpcOwner>($"Expected NpcOwner but got {owner.GetType().Name}");

        var npcOwner = owner as NpcOwner;
        npcOwner.ShouldNotBeNull();
        npcOwner.Npc.FormKey.ShouldBe(npc.FormKey);
    }

    [Theory]
    [MutagenAutoData]
    public void WithUtf8Encoding_ReadsUtf8Characters(
        IFileSystem fs,
        ModKey modKey,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        // Create a mod with UTF8 characters in NPC name
        const string utf8TestString = "Привет мир"; // "Hello world" in Russian

        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        npc.EditorID = "TestNpc";
        npc.Name = utf8TestString;

        var modPath = Path.Combine(dataDirectoryProvider.Path, modKey.FileName);

        // Write with UTF8 encoding
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fs)
            .WithUtf8Encoding()
            .Write();

        // Setup load order
        LoadOrder.Write(
            pluginListingsPathProvider.Path,
            release,
            new[] { new LoadOrderListing(modKey, enabled: true) },
            fileSystem: fs);

        // Build environment with UTF8 encoding
        using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithUtf8Encoding()
            .Build();

        // Verify the mod was loaded with UTF8 encoding
        env.LoadOrder.Count.ShouldBe(1);
        var loadedMod = env.LoadOrder[modKey].Mod;
        loadedMod.ShouldNotBeNull();

        var loadedNpc = loadedMod!.Npcs.First();
        loadedNpc.Name?.String.ShouldBe(utf8TestString, "UTF8 characters should be preserved");
    }

    [Theory]
    [MutagenAutoData]
    public void WithUtf8Encoding_False_UsesDefaultEncoding(
        IFileSystem fs,
        ModKey modKey,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        // Create a mod with UTF8 characters
        const string utf8TestString = "Привет";

        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        npc.EditorID = "TestNpc";
        npc.Name = utf8TestString;

        var modPath = Path.Combine(dataDirectoryProvider.Path, modKey.FileName);

        // Write WITH UTF8 encoding
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fs)
            .WithUtf8Encoding()
            .Write();

        // Setup load order
        LoadOrder.Write(
            pluginListingsPathProvider.Path,
            release,
            new[] { new LoadOrderListing(modKey, enabled: true) },
            fileSystem: fs);

        // Build environment WITHOUT UTF8 encoding (default)
        using var env = GameEnvironment.Typical.Builder<ISkyrimMod, ISkyrimModGetter>(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithUtf8Encoding(false)
            .Build();

        // Verify the mod was loaded but with corrupted UTF8 (read as default encoding)
        env.LoadOrder.Count.ShouldBe(1);
        var loadedMod = env.LoadOrder[modKey].Mod;
        loadedMod.ShouldNotBeNull();

        var loadedNpc = loadedMod!.Npcs.First();
        // The name will be corrupted because UTF8 bytes were read with wrong encoding
        loadedNpc.Name?.String.ShouldNotBe(utf8TestString,
            "Reading UTF8 data without UTF8 encoding should result in corrupted text");
    }

    [Theory]
    [MutagenAutoData]
    public void WithUtf8Encoding_NonGenericBuilder_ReadsUtf8Characters(
        IFileSystem fs,
        ModKey modKey,
        GameRelease release,
        IDataDirectoryProvider dataDirectoryProvider,
        ICreationClubListingsPathProvider cccListingPathContext,
        IPluginListingsPathContext pluginListingsPathProvider)
    {
        // Create a mod with UTF8 characters
        const string utf8TestString = "日本語"; // Japanese

        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        var npc = mod.Npcs.AddNew();
        npc.EditorID = "TestNpc";
        npc.Name = utf8TestString;

        var modPath = Path.Combine(dataDirectoryProvider.Path, modKey.FileName);

        // Write with UTF8 encoding
        mod.BeginWrite
            .ToPath(modPath)
            .WithNoLoadOrder()
            .WithFileSystem(fs)
            .WithUtf8Encoding()
            .Write();

        // Setup load order
        LoadOrder.Write(
            pluginListingsPathProvider.Path,
            release,
            new[] { new LoadOrderListing(modKey, enabled: true) },
            fileSystem: fs);

        // Build environment with UTF8 encoding using non-generic builder
        using var env = GameEnvironment.Typical.Builder(GameRelease.SkyrimSE)
            .WithResolver(t =>
            {
                if (t == typeof(IFileSystem)) return fs;
                if (t == typeof(IDataDirectoryProvider)) return dataDirectoryProvider;
                if (t == typeof(IPluginListingsPathContext)) return pluginListingsPathProvider;
                if (t == typeof(ICreationClubListingsPathProvider)) return cccListingPathContext;
                return default;
            })
            .WithUtf8Encoding()
            .Build();

        // Verify the mod was loaded with UTF8 encoding
        env.LoadOrder.Count.ShouldBe(1);
        var loadedMod = env.LoadOrder[modKey].Mod as ISkyrimModGetter;
        loadedMod.ShouldNotBeNull();

        var loadedNpc = loadedMod!.Npcs.First();
        loadedNpc.Name?.String.ShouldBe(utf8TestString, "UTF8 characters should be preserved in non-generic builder");
    }
}