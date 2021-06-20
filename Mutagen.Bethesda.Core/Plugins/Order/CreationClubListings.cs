using DynamicData;
using Noggog;
using System;
using System.Collections.Generic;
using System.Reactive;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Plugins.Order
{
    public static class CreationClubListings
    {
        private static readonly CreationClubPathProvider PathProvider = new(IFileSystemExt.DefaultFilesystem);
        private static readonly CreationClubListingsProvider ListingsProvider = new(
            IFileSystemExt.DefaultFilesystem,
            PathProvider);

        private static readonly CreationClubLiveLoadOrderProvider LiveLoadOrder = new(IFileSystemExt.DefaultFilesystem);
        
        public static FilePath? GetListingsPath(GameCategory category, DirectoryPath dataPath)
        {
            return PathProvider.GetListingsPath(category, dataPath);
        }

        public static IEnumerable<IModListingGetter> GetListings(GameCategory category, DirectoryPath dataPath)
        {
            return ListingsProvider.GetListings(category, dataPath);
        }

        public static IEnumerable<IModListingGetter> ListingsFromPath(
            FilePath cccFilePath,
            DirectoryPath dataPath)
        {
            return ListingsProvider.GetListingsFromPath(cccFilePath, dataPath);
        }

        public static IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            FilePath cccFilePath,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool orderListings = true)
        {
            return LiveLoadOrder.GetLiveLoadOrder(cccFilePath, dataFolderPath, out state, orderListings);
        }

        public static IObservable<Unit> GetLoadOrderChanged(
            FilePath cccFilePath,
            DirectoryPath dataFolderPath)
        {
            return LiveLoadOrder.GetLoadOrderChanged(cccFilePath, dataFolderPath);
        }
    }
}
