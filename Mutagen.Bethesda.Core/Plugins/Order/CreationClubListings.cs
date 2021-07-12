using DynamicData;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Concurrency;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order.DI;

namespace Mutagen.Bethesda.Plugins.Order
{
    public static class CreationClubListings
    {
        public static FilePath? GetListingsPath(GameCategory category, DirectoryPath dataPath)
        {
            var categoryInject = new GameCategoryInjection(category);
            return new CreationClubListingsPathProvider(
                categoryInject,
                new CreationClubEnabledProvider(
                    categoryInject),
                new GameDirectoryInjection(dataPath.Directory!.Value)).Path;
        }

        public static IEnumerable<IModListingGetter> GetListings(GameCategory category, DirectoryPath dataPath)
        {
            var gameCategoryInjection = new GameCategoryInjection(category);
            var dataDirectoryInjection = new DataDirectoryInjection(dataPath);
            return new CreationClubListingsProvider(
                IFileSystemExt.DefaultFilesystem,
                dataDirectoryInjection,
                new CreationClubListingsPathProvider(
                    gameCategoryInjection,
                    new CreationClubEnabledProvider(
                        gameCategoryInjection),
                    new GameDirectoryInjection(dataPath.Directory!.Value)),
                new CreationClubRawListingsReader(
                    IFileSystemExt.DefaultFilesystem,
                    dataDirectoryInjection)).Get();
        }

        public static IEnumerable<IModListingGetter> ListingsFromPath(
            FilePath cccFilePath,
            DirectoryPath dataPath,
            IFileSystem? fileSystem = null)
        {
            fileSystem ??= IFileSystemExt.DefaultFilesystem;
            var dataDirectoryInjection = new DataDirectoryInjection(dataPath);
            return new CreationClubListingsProvider(
                fileSystem,
                dataDirectoryInjection,
                new CreationClubListingsPathInjection(cccFilePath),
                new CreationClubRawListingsReader(
                    fileSystem,
                    dataDirectoryInjection)).Get();
        }

        public static IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            FilePath cccFilePath,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            IFileSystem? fileSystem = null,
            IScheduler? scheduler = null)
        {
            fileSystem ??= IFileSystemExt.DefaultFilesystem;
            var cccPath = new CreationClubListingsPathInjection(cccFilePath);
            var dataDir = new DataDirectoryInjection(dataFolderPath);
            return new CreationClubLiveLoadOrderProvider(
                new CreationClubLiveListingsFileReader(
                    fileSystem,
                    new CreationClubRawListingsReader(
                        fileSystem,
                        dataDir),
                    cccPath),
                new CreationClubLiveLoadOrderFolderWatcher(
                    fileSystem,
                    dataDir)).Get(out state, scheduler);
        }

        public static IObservable<Unit> GetLoadOrderChanged(
            FilePath cccFilePath,
            DirectoryPath dataFolderPath)
        {
            return GetLiveLoadOrder(cccFilePath, dataFolderPath, out var _)
                .QueryWhenChanged(q => Unit.Default);
        }
    }
}
