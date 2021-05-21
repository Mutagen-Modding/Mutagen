using DynamicData;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Order.Internals;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Plugins.Order
{
    /// <summary>
    /// A static class with LoadOrder related utility functions
    /// </summary>
    public static class LoadOrder
    {
        #region Timestamps
        /// <summary>
        /// Returns whether given game needs timestamp alignment for its load order
        /// </summary>
        /// <param name="game">Game to check</param>
        /// <returns>True if file located</returns>
        public static bool NeedsTimestampAlignment(GameCategory game)
        {
            switch (game)
            {
                case GameCategory.Oblivion:
                    return true;
                case GameCategory.Skyrim:
                    return false;
                case GameCategory.Fallout4:
                    return false;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Constructs a load order from a list of mods and a data folder.
        /// Load Order is sorted to the order the game will load the mod files: by file's date modified timestamp.
        /// </summary>
        /// <param name="incomingLoadOrder">Mods to include</param>
        /// <param name="dataPath">Path to data folder</param>
        /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
        /// <returns>Enumerable of modkeys in load order, excluding missing mods</returns>
        /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
        public static IEnumerable<IModListingGetter> AlignToTimestamps(
            IEnumerable<IModListingGetter> incomingLoadOrder,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            var list = new List<(bool Enabled, ModKey ModKey, DateTime Write)>();
            foreach (var key in incomingLoadOrder)
            {
                ModPath modPath = new ModPath(key.ModKey, Path.Combine(dataPath.Path, key.ModKey.FileName));
                if (!modPath.Path.Exists)
                {
                    if (throwOnMissingMods) throw new MissingModException(modPath);
                    continue;
                }
                list.Add((key.Enabled, key.ModKey, File.GetLastWriteTime(modPath.Path.Path)));
            }
            var comp = new LoadOrderTimestampComparer(incomingLoadOrder.Select(i => i.ModKey).ToList());
            return list
                .OrderBy(i => (i.ModKey, i.Write), comp)
                .Select(i => new ModListing(i.ModKey, i.Enabled));
        }

        /// <summary>
        /// Constructs a load order from a list of mods and a data folder.
        /// Load Order is sorted to the order the game will load the mod files: by file's date modified timestamp.
        /// </summary>
        /// <param name="incomingLoadOrder">Mods and their write timestamps</param>
        /// <returns>Enumerable of modkeys in load order, excluding missing mods</returns>
        /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
        public static IEnumerable<ModKey> AlignToTimestamps(IEnumerable<(ModKey ModKey, DateTime Write)> incomingLoadOrder)
        {
            return incomingLoadOrder
                .OrderBy(i => i, new LoadOrderTimestampComparer(incomingLoadOrder.Select(i => i.ModKey).ToList()))
                .Select(i => i.ModKey);
        }

        /// <summary>
        /// Modifies time stamps of files to match the given ordering
        /// <param name="loadOrder">Order to conform files to</param>
        /// <param name="dataPath">Path to data folder</param>
        /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
        /// <param name="startDate">Date to give the first file</param>
        /// <param name="interval">Time interval to space between each file's date</param>
        /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
        /// </summary>
        public static void AlignTimestamps(
            IEnumerable<ModKey> loadOrder,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true,
            DateTime? startDate = null,
            TimeSpan? interval = null)
        {
            startDate ??= DateTime.Today.AddDays(-1);
            interval ??= TimeSpan.FromMinutes(1);
            foreach (var mod in loadOrder)
            {
                ModPath modPath = new ModPath(mod, Path.Combine(dataPath.Path, mod.FileName));
                if (!modPath.Path.Exists)
                {
                    if (throwOnMissingMods) throw new MissingModException(modPath);
                    continue;
                }
                File.SetLastWriteTime(modPath.Path.Path, startDate.Value);
                startDate = startDate.Value.Add(interval.Value);
            }
        }
        #endregion

        /// <summary>
        /// Returns a load order listing from the usual sources
        /// </summary>
        /// <param name="game">Game type</param>
        /// <param name="dataPath">Path to game's data folder</param>
        /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
        /// <returns>Enumerable of modkeys representing a load order</returns>
        /// <exception cref="ArgumentException">Line in plugin file is unexpected</exception>
        /// <exception cref="FileNotFoundException">If plugin file not located</exception>
        /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
        public static IEnumerable<IModListingGetter> GetListings(
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            if (!PluginListings.TryGetListingsFile(game, out var path))
            {
                throw new FileNotFoundException("Could not locate plugins file");
            }
            return GetListings(
                game: game,
                pluginsFilePath: path,
                creationClubFilePath: CreationClubListings.GetListingsPath(game.ToCategory(), dataPath),
                dataPath: dataPath,
                throwOnMissingMods: throwOnMissingMods);
        }

        public static IEnumerable<IModListingGetter> GetListings(
            GameRelease game,
            FilePath pluginsFilePath,
            FilePath? creationClubFilePath,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            var listings = Enumerable.Empty<IModListingGetter>();
            if (pluginsFilePath.Exists)
            {
                listings = PluginListings.ListingsFromPath(pluginsFilePath, game, dataPath, throwOnMissingMods);
            }
            var implicitListings = Implicits.Get(game).Listings
                .Where(x => File.Exists(Path.Combine(dataPath.Path, x.FileName.String)))
                .Select(x => new ModListing(x, enabled: true));
            var ccListings = Enumerable.Empty<IModListingGetter>();
            if (creationClubFilePath != null && creationClubFilePath.Value.Exists)
            {
                ccListings = CreationClubListings.ListingsFromPath(creationClubFilePath.Value, dataPath);
            }

            return OrderListings(
                implicitListings: implicitListings,
                pluginsListings: listings.Except(implicitListings),
                creationClubListings: ccListings,
                selector: x => x.ModKey);
        }

        public static IEnumerable<T> OrderListings<T>(IEnumerable<T> e, Func<T, ModKey> selector)
        {
            return e.OrderBy(e => selector(e).Type);
        }

        public static IEnumerable<T> OrderListings<T>(
            IEnumerable<T> implicitListings,
            IEnumerable<T> pluginsListings,
            IEnumerable<T> creationClubListings,
            Func<T, ModKey> selector)
        {
            var plugins = pluginsListings
                .Select(selector)
                .ToList();
            return implicitListings
                .Concat(
                    OrderListings(creationClubListings
                        .Select(x =>
                        {
                            if (selector(x).Type == ModType.Plugin)
                            {
                                throw new NotImplementedException("Creation Club does not support esp plugins.");
                            }
                            return x;
                        })
                        // If CC mod is on plugins list, refer to its ordering
                        .OrderBy(selector, Comparer<ModKey>.Create((x, y) =>
                        {
                            var xIndex = plugins.IndexOf(x);
                            var yIndex = plugins.IndexOf(y);
                            if (xIndex == yIndex) return 0;
                            return xIndex - yIndex;
                        })), selector))
                .Concat(pluginsListings)
                .Distinct(selector);
        }

        public static IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            GameRelease game,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool throwOnMissingMods = true)
        {
            return GetLiveLoadOrder(
                game: game,
                dataFolderPath: dataFolderPath,
                loadOrderFilePath: PluginListings.GetListingsPath(game),
                cccLoadOrderFilePath: CreationClubListings.GetListingsPath(game.ToCategory(), dataFolderPath),
                state: out state,
                throwOnMissingMods: throwOnMissingMods);
        }

        public static IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            IObservable<GameRelease> game,
            IObservable<DirectoryPath> dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool throwOnMissingMods = true)
        {
            var obs = Observable.CombineLatest(
                    game,
                    dataFolderPath,
                    (gameVal, dataFolderVal) =>
                    {
                        var lo = GetLiveLoadOrder(
                            game: gameVal,
                            dataFolderPath: dataFolderVal,
                            loadOrderFilePath: PluginListings.GetListingsPath(gameVal),
                            cccLoadOrderFilePath: CreationClubListings.GetListingsPath(gameVal.ToCategory(), dataFolderVal),
                            state: out var state,
                            throwOnMissingMods: throwOnMissingMods);
                        return (LoadOrder: lo, State: state);
                    })
                .Replay(1)
                .RefCount();
            state = obs.Select(x => x.State)
                .Switch();
            return obs.Select(x => x.LoadOrder)
                .Switch();
        }

        // ToDo
        // Add scheduler for throttle
        public static IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            GameRelease game,
            FilePath loadOrderFilePath,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            FilePath? cccLoadOrderFilePath = null,
            bool throwOnMissingMods = true)
        {
            var listings = Implicits.Get(game).Listings
                .Select(x => new ModListing(x, enabled: true))
                .ToArray();
            var listingsChanged = PluginListings.GetLoadOrderChanged(loadOrderFilePath);
            if (cccLoadOrderFilePath != null)
            {
                listingsChanged = listingsChanged.Merge(
                    CreationClubListings.GetLoadOrderChanged(cccFilePath: cccLoadOrderFilePath.Value, dataFolderPath));
            }
            listingsChanged = listingsChanged.PublishRefCount();
            var stateSubj = new BehaviorSubject<Exception?>(null);
            state = stateSubj
                .Distinct()
                .Select(x => x == null ? ErrorResponse.Success : ErrorResponse.Fail(x));
            return Observable.Create<IChangeSet<IModListingGetter>>((observer) =>
            {
                CompositeDisposable disp = new CompositeDisposable();
                SourceList<IModListingGetter> list = new SourceList<IModListingGetter>();
                disp.Add(listingsChanged
                    .StartWith(Unit.Default)
                    .Throttle(TimeSpan.FromMilliseconds(150))
                    .Subscribe(_ =>
                    {
                        // Try three times, in case things locked
                        for (int i = 0; i < 3; i++)
                        {
                            try
                            {
                                // Short circuit if not subscribed anymore
                                if (disp.IsDisposed) return;

                                var refreshedListings = GetListings(
                                    game,
                                    loadOrderFilePath,
                                    cccLoadOrderFilePath,
                                    dataFolderPath,
                                    throwOnMissingMods).ToArray();
                                // ToDo
                                // Upgrade to SetTo mechanics.
                                // SourceLists' EditDiff seems weird
                                list.Clear();
                                list.AddRange(refreshedListings);
                                stateSubj.OnNext(null);
                            }
                            catch (Exception ex)
                            {
                                // Short circuit if not subscribed anymore
                                if (disp.IsDisposed) return;

                                stateSubj.OnNext(ex);
                            }
                        }
                    }));
                list.Connect()
                    .Subscribe(observer);
                return disp;
            });
        }

        public static IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            IObservable<GameRelease> game,
            IObservable<FilePath> loadOrderFilePath,
            IObservable<DirectoryPath> dataFolderPath,
            out IObservable<ErrorResponse> state,
            IObservable<FilePath?>? cccLoadOrderFilePath = null,
            bool throwOnMissingMods = true)
        {
            var obs = Observable.CombineLatest(
                    game,
                    dataFolderPath,
                    loadOrderFilePath,
                    cccLoadOrderFilePath ?? Observable.Return(default(FilePath?)),
                    (gameVal, dataFolderVal, loadOrderFilePathVal, cccVal) =>
                    {
                        var lo = GetLiveLoadOrder(
                            game: gameVal,
                            dataFolderPath: dataFolderVal,
                            loadOrderFilePath: loadOrderFilePathVal,
                            cccLoadOrderFilePath: cccVal,
                            state: out var state,
                            throwOnMissingMods: throwOnMissingMods);
                        return (LoadOrder: lo, State: state);
                    })
                .Replay(1)
                .RefCount();
            state = obs.Select(x => x.State)
                .Switch();
            return obs.Select(x => x.LoadOrder)
                .Switch();
        }

        /// <summary>
        /// Constructs a load order filled with mods constructed
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of listings to import</param>
        /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
        /// This may be unapplicable to some games with only one release, but should still be passed in.
        /// </param>
        public static ILoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<IModListingGetter> loadOrder,
            GameRelease gameRelease)
            where TMod : class, IModGetter
        {
            return Import(
                dataFolder,
                loadOrder,
                (modPath) => ModInstantiator<TMod>.Importer(modPath, gameRelease));
        }

        /// <summary>
        /// Constructs a load order filled with mods constructed
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of mod keys to import</param>
        /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
        /// This may be unapplicable to some games with only one release, but should still be passed in.
        /// </param>
        public static LoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<ModKey> loadOrder,
            GameRelease gameRelease)
            where TMod : class, IModGetter
        {
            return Import(
                dataFolder,
                loadOrder.Select(m => new ModListing(m, enabled: true)),
                (modPath) => ModInstantiator<TMod>.Importer(modPath, gameRelease));
        }

        /// <summary>
        /// Constructs a load order filled with mods constructed by given importer func
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of mod keys to import</param>
        /// <param name="factory">Func to use to create a new mod from a path</param>
        public static LoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<ModKey> loadOrder,
            Func<ModPath, TMod> factory)
            where TMod : class, IModGetter
        {
            return Import(
                dataFolder,
                loadOrder.Select(m => new ModListing(m, enabled: true)),
                factory);
        }

        /// <summary>
        /// Constructs a load order filled with mods constructed by given importer func
        /// </summary>
        /// <param name="dataFolder">Path data folder containing mods</param>
        /// <param name="loadOrder">Unique list of listings to import</param>
        /// <param name="factory">Func to use to create a new mod from a path</param>
        public static LoadOrder<IModListing<TMod>> Import<TMod>(
            DirectoryPath dataFolder,
            IEnumerable<IModListingGetter> loadOrder,
            Func<ModPath, TMod> factory)
            where TMod : class, IModGetter
        {
            var loList = loadOrder.ToList();
            var results = new (ModKey ModKey, int ModIndex, TryGet<TMod> Mod, bool Enabled)[loList.Count];
            try
            {
                Parallel.ForEach(loList, (listing, state, modIndex) =>
                {
                    try
                    {
                        var modPath = new ModPath(listing.ModKey, dataFolder.GetFile(listing.ModKey.FileName).Path);
                        if (!modPath.Path.Exists)
                        {
                            results[modIndex] = (listing.ModKey, (int)modIndex, TryGet<TMod>.Failure, listing.Enabled);
                            return;
                        }
                        var mod = factory(modPath);
                        results[modIndex] = (listing.ModKey, (int)modIndex, TryGet<TMod>.Succeed(mod), listing.Enabled);
                    }
                    catch (Exception ex)
                    {
                        throw RecordException.Enrich(ex, listing.ModKey);
                    }
                });
                return new LoadOrder<IModListing<TMod>>(results
                    .OrderBy(i => i.ModIndex)
                    .Select(item =>
                    {
                        if (item.Mod.Succeeded)
                        {
                            return new ModListing<TMod>(item.Mod.Value, item.Enabled);
                        }
                        else
                        {
                            return ModListing<TMod>.CreateUnloaded(item.ModKey, item.Enabled);
                        }
                    }));
            }
            catch (Exception)
            {
                // We're aborting, but we still want to dispose any that were successful
                foreach (var result in results)
                {
                    if (result.Mod.Value is IDisposable disp)
                    {
                        disp.Dispose();
                    }
                }
                throw;
            }
        }

        public static void Write(
            string path, 
            GameRelease release, 
            IEnumerable<IModListingGetter> loadOrder,
            bool removeImplicitMods = true)
        {
            bool markers = PluginListings.HasEnabledMarkers(release);
            var loadOrderList = loadOrder.ToList();
            if (removeImplicitMods)
            {
                foreach (var implicitMod in Implicits.Get(release).Listings)
                {
                    if (loadOrderList.Count > 0
                        && loadOrderList[0].ModKey == implicitMod
                        && loadOrderList[0].Enabled)
                    {
                        loadOrderList.RemoveAt(0);
                    }
                }
            }
            File.WriteAllLines(path,
                loadOrderList.Where(x =>
                {
                    return (markers || x.Enabled);
                })
                .Select(x =>
                {
                    if (x.Enabled && markers)
                    {
                        return $"*{x.ModKey.FileName}";
                    }
                    else
                    {
                        return x.ModKey.FileName.String;
                    }
                }));
        }
    }

    public interface ILoadOrderGetter : IDisposable
    {
        /// <summary>
        /// Number of listings on the Load Order
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Listings in the order they were listed
        /// </summary>
        IEnumerable<ModKey> ListedOrder { get; }

        /// <summary>
        /// Listings in priority order, where the mod with the highest priority comes first.  (Reverse of ListedOrder)
        /// </summary>
        IEnumerable<ModKey> PriorityOrder { get; }

        /// <summary>
        /// Whether the load order contains a listing with the given key
        /// </summary>
        bool ContainsKey(ModKey key);
    }

    public interface ILoadOrderGetter<out TListing> : ILoadOrderGetter, IReadOnlyList<Noggog.IKeyValue<TListing, ModKey>>, IReadOnlyCache<TListing, ModKey>
        where TListing : IModKeyed
    {
        new TListing this[int index] { get; }
        
        TListing? TryGetAtIndex(int index);

        /// <summary>
        /// Listings in the order they were listed
        /// </summary>
        new IEnumerable<TListing> ListedOrder { get; }

        /// <summary>
        /// Listings in priority order, where the mod with the highest priority comes first.  (Reverse of ListedOrder)
        /// </summary>
        new IEnumerable<TListing> PriorityOrder { get; }

        /// <summary>
        /// Number of listings on the Load Order
        /// </summary>
        new int Count { get; }

        /// <summary>
        /// Whether the load order contains a listing with the given key
        /// </summary>
        new bool ContainsKey(ModKey key);

        /// <summary>
        /// Locates index of an item with given key
        /// </summary>
        /// <param name="key">Key to query</param>
        /// <returns>Index of item on list with key. -1 if not located</returns>
        int IndexOf(ModKey key);
    }

    public interface ILoadOrder<TListing> : ILoadOrderGetter<TListing>
        where TListing : IModKeyed
    {
        /// <summary>
        /// Adds an item to the end of the load order
        /// </summary>
        /// <param name="item">Item to put at end of load order</param>
        /// <exception cref="ArgumentException">If an item with same ModKey exists already</exception>
        void Add(TListing item);

        /// <summary>
        /// Adds items to the end of the load order
        /// </summary>
        /// <param name="items">Items to put at end of load order</param>
        /// <exception cref="ArgumentException">If an item with same ModKey exists already</exception>
        void Add(IEnumerable<TListing> items);

        /// <summary>
        /// Adds an item at the given index in load order, with the given ModKey
        /// </summary>
        /// <param name="item">Item to put at end of load order</param>
        /// <param name="index">Index to insert at</param>
        /// <exception cref="ArgumentException">If an item with same ModKey exists already</exception>
        void Add(TListing item, int index);

        /// <summary>
        /// Clears load order of all items
        /// </summary>
        void Clear();

        bool RemoveKey(ModKey modKey);

        void RemoveAt(int index);

        void Set(TListing listing);

        void Set(IEnumerable<TListing> items);
    }

    /// <summary>
    /// A container for objects with in a specific load order, that are associated with ModKeys.
    /// LoadOrder does not need to be disposed for proper use, but rather can optionally be disposed of which will dispose any contained items that implement IDisposable
    /// </summary>
    public class LoadOrder<TListing> : ILoadOrder<TListing>
        where TListing : IModKeyed
    {
        private readonly List<ItemContainer> _byLoadOrder = new();
        private readonly Dictionary<ModKey, ItemContainer> _byModKey = new();
        private readonly bool _disposeItems;

        /// <inheritdoc />
        public int Count => _byLoadOrder.Count;

        /// <inheritdoc />
        public TListing this[int index] => _byLoadOrder[index].Item;

        IEnumerable<TListing> IReadOnlyCache<TListing, ModKey>.Items => ListedOrder;

        /// <inheritdoc />
        public IEnumerable<ModKey> Keys => _byModKey.Keys;

        /// <inheritdoc />
        public IEnumerable<TListing> ListedOrder => _byLoadOrder.Select(i => i.Item);

        /// <inheritdoc />
        public IEnumerable<TListing> PriorityOrder => ((IEnumerable<ItemContainer>)_byLoadOrder).Reverse().Select(i => i.Item);

        IEnumerable<ModKey> ILoadOrderGetter.ListedOrder => _byLoadOrder.Select(x => x.Item.ModKey);

        IEnumerable<ModKey> ILoadOrderGetter.PriorityOrder => _byLoadOrder.Select(x => x.Item.ModKey).Reverse();

        Noggog.IKeyValue<TListing, ModKey> IReadOnlyList<Noggog.IKeyValue<TListing, ModKey>>.this[int index]
        {
            get
            {
                var cont = _byLoadOrder[index];
                return new KeyValue<TListing, ModKey>(cont.Item.ModKey, cont.Item);
            }
        }

        /// <inheritdoc />
        public TListing this[ModKey key] => _byModKey[key].Item;

        public LoadOrder(bool disposeItems = true)
        {
            _disposeItems = disposeItems;
        }

        public LoadOrder(IEnumerable<TListing> items, bool disposeItems = true)
        {
            _disposeItems = disposeItems;
            int index = 0;
            _byLoadOrder.AddRange(items.Select(i => new ItemContainer(i, index++)));
            foreach (var item in _byLoadOrder)
            {
                try
                {
                    _byModKey.Add(item.Item.ModKey, item);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException($"ModKey was already present: {item.Item.ModKey}");
                }
            }
        }

        /// <summary>
        /// Attempts to retrieve an item given a ModKey
        /// </summary>
        /// <param name="key">ModKey to query for</param>
        /// <param name="value">Result reference to the item</param>
        /// <returns>True if matching key located</returns>
        public bool TryGetValue(ModKey key, [MaybeNullWhen(false)] out TListing value)
        {
            if (_byModKey.TryGetValue(key, out var container))
            {
                value = container.Item;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Attempts to retrieve an item given a ModKey
        /// </summary>
        /// <param name="key">ModKey to query for</param>
        /// <returns>Result reference to the item, or null if not found</returns>
        public TListing? TryGetValue(ModKey key)
        {
            if (_byModKey.TryGetValue(key, out var container))
            {
                return container.Item;
            }
            return default;
        }

        /// <summary>
        /// Attempts to retrieve an item given an index
        /// </summary>
        /// <param name="index">Index to retrieve</param>
        /// <returns>TryGet result of the item</returns>
        public TListing? TryGetAtIndex(int index)
        {
            if (!_byLoadOrder.InRange(index))
            {
                return default;
            }

            return _byLoadOrder[index].Item;
        }

        /// <inheritdoc />
        public void Add(TListing item)
        {
            var index = _byLoadOrder.Count;
            var container = new ItemContainer(item, index);
            try
            {
                _byModKey.Add(item.ModKey, container);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"ModKey was already present: {item.ModKey}");
            }
            _byLoadOrder.Add(container);
        }

        /// <inheritdoc />
        public void Add(IEnumerable<TListing> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        /// <inheritdoc />
        public void Add(TListing item, int index)
        {
            if (!_byLoadOrder.InRange(index))
            {
                throw new ArgumentException("Tried to insert at an out of range index.");
            }
            var container = new ItemContainer(item, index);
            try
            {
                _byModKey.Add(item.ModKey, container);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"ModKey was already present: {item.ModKey}");
            }
            _byLoadOrder.Add(container);
            for (int i = index + 1; i < _byLoadOrder.Count; i++)
            {
                _byLoadOrder[i].Index += 1;
            }
        }

        /// <inheritdoc />
        public bool ContainsKey(ModKey key)
        {
            return IndexOf(key) != -1;
        }

        /// <inheritdoc />
        public int IndexOf(ModKey key)
        {
            if (!_byModKey.TryGetValue(key, out var container))
            {
                return -1;
            }
            return container.Index;
        }

        /// <inheritdoc />
        public void Clear()
        {
            Dispose();
            this._byLoadOrder.Clear();
            this._byModKey.Clear();
        }

        public bool RemoveKey(ModKey key)
        {
            if (!_byModKey.TryGetValue(key, out var registry)) return false;
            _byLoadOrder.RemoveAt(registry.Index);
            for (int i = registry.Index; i < _byLoadOrder.Count; i++)
            {
                _byLoadOrder[i].Index--;
            }

            _byModKey.Remove(key);
            return true;
        }

        public void RemoveAt(int index)
        {
            var item = _byLoadOrder[index];
            _byLoadOrder.RemoveAt(index);
            _byModKey.Remove(item.Item.ModKey);
            for (int i = index; i < _byLoadOrder.Count; i++)
            {
                _byLoadOrder[i].Index--;
            }

            if (_disposeItems && item.Item is IDisposable disp)
            {
                disp.Dispose();
            }
        }

        public void Set(TListing listing)
        {
            if (!_byModKey.TryGetValue(listing.ModKey, out var existing))
            {
                Add(listing);
                return;
            }

            var old = existing.Item;
            existing.Item = listing;

            if (_disposeItems && old is IDisposable disp)
            {
                disp.Dispose();
            }
        }

        public void Set(IEnumerable<TListing> items)
        {
            foreach (var item in items)
            {
                Set(item);
            }
        }

        IEnumerator<Noggog.IKeyValue<TListing, ModKey>> IEnumerable<Noggog.IKeyValue<TListing, ModKey>>.GetEnumerator()
        {
            return ListedOrder.Select(x => new KeyValue<TListing, ModKey>(x.ModKey, x)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Disposes all contained items that implement IDisposable
        /// </summary>
        public void Dispose()
        {
            if (!_disposeItems) return;
            foreach (var item in _byLoadOrder)
            {
                if (item.Item is IDisposable disp)
                {
                    disp.Dispose();
                }
            }
        }

        public IEnumerator<TListing> GetEnumerator()
        {
            foreach (var item in _byLoadOrder)
            {
                yield return item.Item;
            }
        }

        private class ItemContainer
        {
            public TListing Item;
            public int Index;

            public ItemContainer(TListing item, int index)
            {
                Item = item;
                Index = index;
            }
        }
    }
}
