using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records.DI;
using Noggog;

namespace Mutagen.Bethesda.Environments;

public record GameEnvironmentBuilder<TMod, TModGetter>
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
{
    private IGameReleaseContext Release { get; }

    internal IDataDirectoryProvider? DataDirectoryProvider { get; init; }
    internal ILoadOrderListingsProvider? ListingsProvider { get; init; }
    internal IPluginListingsPathProvider? PluginListingsPathProvider { get; init; }
    internal ICreationClubListingsPathProvider? CccListingsPathProvider { get; init; }

    private ImmutableList<Func<IEnumerable<IModListingGetter>, IEnumerable<IModListingGetter>>> LoadOrderProcessors { get; init; }

    private ImmutableList<TMod> MutableMods { get; init; }

    private GameEnvironmentBuilder(GameRelease release)
    {
        Release = new GameReleaseInjection(release);
        LoadOrderProcessors = ImmutableList<Func<IEnumerable<IModListingGetter>, IEnumerable<IModListingGetter>>>.Empty;
        MutableMods = ImmutableList<TMod>.Empty;
    }

    internal GameEnvironmentBuilder(
        IGameReleaseContext releaseProvider,
        IDataDirectoryProvider dataDirectoryProvider,
        ILoadOrderListingsProvider listingsProvider,
        IPluginListingsPathProvider pluginListingsPathProvider,
        ICreationClubListingsPathProvider cccListingsPathProvider)
    {
        Release = releaseProvider;
        DataDirectoryProvider = dataDirectoryProvider;
        ListingsProvider = listingsProvider;
        PluginListingsPathProvider = pluginListingsPathProvider;
        CccListingsPathProvider = cccListingsPathProvider;
        LoadOrderProcessors = ImmutableList<Func<IEnumerable<IModListingGetter>, IEnumerable<IModListingGetter>>>.Empty;
        MutableMods = ImmutableList<TMod>.Empty;
    }

    public static GameEnvironmentBuilder<TMod, TModGetter> Create(GameRelease release)
    {
        return new(release);
    }

    public GameEnvironmentBuilder<TMod, TModGetter> TransformLoadOrder(Func<IEnumerable<IModListingGetter>, IEnumerable<IModListingGetter>> transformer)
    {
        return this with { LoadOrderProcessors = LoadOrderProcessors.Add(transformer) };
    }

    public GameEnvironmentBuilder<TMod, TModGetter> WithOutputMod(TMod mod)
    {
        return this with { MutableMods = MutableMods.Add(mod) };
    }

    public IGameEnvironmentState<TMod, TModGetter> Build()
    {
        var category = new GameCategoryContext(Release);
        var gameLocator = new GameLocator();
        var dataDirectory = DataDirectoryProvider ?? new DataDirectoryProvider(Release, gameLocator);
        var pluginPathProvider = PluginListingsPathProvider ?? new PluginListingsPathProvider(Release);
        var cccPath = CccListingsPathProvider ?? new CreationClubListingsPathProvider(
            category,
            new CreationClubEnabledProvider(category),
            new GameDirectoryProvider(
                Release,
                gameLocator));
        var pluginRawListingsReader = new PluginRawListingsReader(
            IFileSystemExt.DefaultFilesystem,
            new PluginListingsParser(
                new ModListingParser(
                    new HasEnabledMarkersProvider(
                        Release))));

        var listingsProv = ListingsProvider ?? new LoadOrderListingsProvider(
            new OrderListings(),
            new ImplicitListingsProvider(
                IFileSystemExt.DefaultFilesystem,
                dataDirectory,
                new ImplicitListingModKeyProvider(
                    Release)),
            new PluginListingsProvider(
                Release,
                new TimestampedPluginListingsProvider(
                    new TimestampAligner(IFileSystemExt.DefaultFilesystem),
                    new TimestampedPluginListingsPreferences() { ThrowOnMissingMods = false },
                    pluginRawListingsReader,
                    dataDirectory,
                    pluginPathProvider),
                new EnabledPluginListingsProvider(
                    pluginRawListingsReader,
                    pluginPathProvider)),
            new CreationClubListingsProvider(
                IFileSystemExt.DefaultFilesystem,
                dataDirectory,
                cccPath,
                new CreationClubRawListingsReader()));

        var filteredListings = listingsProv.Get();
        foreach (var filter in LoadOrderProcessors)
        {
            filteredListings = filter(filteredListings);
        }

        var loGetter = new LoadOrderImporter<TModGetter>(
            IFileSystemExt.DefaultFilesystem,
            dataDirectory,
            new LoadOrderListingsInjection(filteredListings),
            new ModImporter<TModGetter>(
                IFileSystemExt.DefaultFilesystem,
                Release));

        var lo = loGetter.Import();

        var linkCache = lo.ToMutableLinkCache(MutableMods.ToArray());

        return new GameEnvironmentState<TMod, TModGetter>(
            Release.Release,
            dataFolderPath: dataDirectory.Path,
            loadOrderFilePath: pluginPathProvider.Path,
            creationClubListingsFilePath: cccPath.Path,
            loadOrder: loGetter.Import(),
            linkCache: linkCache);
    }
}