using DynamicData;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Mutagen.Bethesda.Plugins.Order
{
    public static class CreationClubListings
    {
        public static FilePath GetListingsPath(GameCategory category, DirectoryPath dataPath)
        {
            switch (category)
            {
                case GameCategory.Oblivion:
                    throw new ArgumentException();
                case GameCategory.Skyrim:
                case GameCategory.Fallout4:
                    return Path.Combine(Path.GetDirectoryName(dataPath.Path)!, $"{category}.ccc");
                default:
                    throw new NotImplementedException();
            }
        }

        public static IEnumerable<LoadOrderListing> GetListings(GameCategory category, DirectoryPath dataPath)
        {
            var path = GetListingsPath(category, dataPath);
            if (!path.Exists) return Enumerable.Empty<LoadOrderListing>();
            return ListingsFromPath(
                path,
                dataPath);
        }

        public static IEnumerable<LoadOrderListing> ListingsFromPath(
            FilePath cccFilePath,
            DirectoryPath dataPath)
        {
            return File.ReadAllLines(cccFilePath.Path)
                .Select(x => ModKey.FromNameAndExtension(x))
                .Where(x => File.Exists(Path.Combine(dataPath.Path, x.FileName)))
                .Select(x => new LoadOrderListing(x, enabled: true))
                .ToList();
        }

        public static IObservable<IChangeSet<LoadOrderListing>> GetLiveLoadOrder(
            FilePath cccFilePath,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool orderListings = true)
        {
            var raw = ObservableExt.WatchFile(cccFilePath.Path)
                .StartWith(Unit.Default)
                .Select(_ =>
                {
                    try
                    {
                        return GetResponse<IObservable<IChangeSet<ModKey>>>.Succeed(
                            File.ReadAllLines(cccFilePath.Path)
                                .Select(x => ModKey.FromNameAndExtension(x))
                                .AsObservableChangeSet());
                    }
                    catch (Exception ex)
                    {
                        return GetResponse<IObservable<IChangeSet<ModKey>>>.Fail(ex);
                    }
                })
                .Replay(1)
                .RefCount();
            state = raw
                .Select(r => (ErrorResponse)r);
            var ret = ObservableListEx.And(
                raw
                .Select(r =>
                {
                    return r.Value ?? Observable.Empty<IChangeSet<ModKey>>();
                })
                .Switch(),
                ObservableExt.WatchFolderContents(dataFolderPath.Path)
                    .Transform(x =>
                    {
                        if (ModKey.TryFromNameAndExtension(Path.GetFileName(x), out var modKey))
                        {
                            return TryGet<ModKey>.Succeed(modKey);
                        }
                        return TryGet<ModKey>.Failure;
                    })
                    .Filter(x => x.Succeeded)
                    .Transform(x => x.Value)
                    .RemoveKey())
                .Transform(x => new LoadOrderListing(x, true));
            if (orderListings)
            {
                ret = ret.OrderListings();
            }
            return ret;
        }

        public static IObservable<Unit> GetLoadOrderChanged(
            FilePath cccFilePath,
            DirectoryPath dataFolderPath)
        {
            return GetLiveLoadOrder(cccFilePath, dataFolderPath, out _, orderListings: false)
                .QueryWhenChanged(q => Unit.Default);
        }
    }
}
