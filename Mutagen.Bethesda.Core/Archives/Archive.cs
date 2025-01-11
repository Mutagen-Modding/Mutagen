using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins;
using Noggog;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Inis.DI;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using StrongInject;
using Stream = System.IO.Stream;

namespace Mutagen.Bethesda.Archives;

[Register(typeof(IniPathLookup), typeof(IIniPathLookup))]
[Register(typeof(IniPathProvider), typeof(IIniPathProvider))]
[Register(typeof(GetArchiveIniListings), typeof(IGetArchiveIniListings))]
[Register(typeof(CachedArchiveListingDetailsProvider), typeof(IArchiveListingDetailsProvider))]
[Register(typeof(ArchiveExtensionProvider), typeof(IArchiveExtensionProvider))]
[Register(typeof(CheckArchiveApplicability), typeof(ICheckArchiveApplicability))]
[Register(typeof(LoadOrderListingsProvider), typeof(ILoadOrderListingsProvider))]
[Register(typeof(OrderListings), typeof(IOrderListings))]
[Register(typeof(ImplicitListingsProvider), typeof(IImplicitListingsProvider))]
[Register(typeof(ImplicitListingModKeyProvider), typeof(IImplicitListingModKeyProvider))]
[Register(typeof(EnabledPluginListingsProvider), typeof(IEnabledPluginListingsProvider))]
[Register(typeof(TimestampedPluginListingsProvider), typeof(ITimestampedPluginListingsProvider))]
[Register(typeof(PluginListingsProvider), typeof(IPluginListingsProvider))]
[Register(typeof(PluginRawListingsReader), typeof(IPluginRawListingsReader))]
[Register(typeof(PluginListingsPathContext), typeof(IPluginListingsPathContext))]
[Register(typeof(TimestampAligner), typeof(ITimestampAligner))]
[Register(typeof(PluginListingsParser), typeof(IPluginListingsParser))]
[Register(typeof(PluginListingsPathProvider), typeof(IPluginListingsPathProvider))]
[Register(typeof(CreationClubListingsProvider), typeof(ICreationClubListingsProvider))]
[Register(typeof(LoadOrderListingParser), typeof(ILoadOrderListingParser))]
[Register(typeof(CreationClubListingsPathProvider), typeof(ICreationClubListingsPathProvider))]
[Register(typeof(CreationClubRawListingsReader), typeof(ICreationClubRawListingsReader))]
[Register(typeof(TimestampedPluginListingsPreferences), typeof(ITimestampedPluginListingsPreferences))]
[Register(typeof(HasEnabledMarkersProvider), typeof(IHasEnabledMarkersProvider))]
[Register(typeof(GameDirectoryProvider), typeof(IGameDirectoryProvider))]
[Register(typeof(CreationClubEnabledProvider), typeof(ICreationClubEnabledProvider))]
[Register(typeof(GameCategoryContext), typeof(IGameCategoryContext))]
[Register(typeof(PluginListingCommentTrimmer), typeof(IPluginListingCommentTrimmer))]
[Register(typeof(GetApplicableArchivePaths))]
partial class ArchiveContainer : IContainer<GetApplicableArchivePaths>
{
    [Instance] private readonly IFileSystem _fileSystem;
    [Instance] private readonly IGameReleaseContext _gameReleaseContext;
    [Instance] private readonly IDataDirectoryProvider _dataDirectoryProvider;
    [Instance] private readonly IGameDirectoryLookup _gameDirectoryLookup = GameLocatorLookupCache.Instance;

    public ArchiveContainer(
        IFileSystem fileSystem, 
        IGameReleaseContext GameReleaseContext,
        IDataDirectoryProvider DataDirectoryProvider)
    {
        _fileSystem = fileSystem;
        _gameReleaseContext = GameReleaseContext;
        _dataDirectoryProvider = DataDirectoryProvider;
    }
}

[Register(typeof(IniPathLookup), typeof(IIniPathLookup))]
[Register(typeof(IniPathProvider), typeof(IIniPathProvider))]
[Register(typeof(GetArchiveIniListings))]
partial class GetArchiveIniListingsContainer : IContainer<GetArchiveIniListings>
{
    [Instance] private readonly IFileSystem _fileSystem;
    [Instance] private readonly IGameReleaseContext _gameReleaseContext;
    [Instance] private readonly IGameDirectoryLookup _gameDirectoryLookup = GameLocatorLookupCache.Instance;
    
    public GetArchiveIniListingsContainer(
        IFileSystem? fileSystem,
        GameRelease release)
    {
        _fileSystem = fileSystem.GetOrDefault();
        _gameReleaseContext = new GameReleaseInjection(release);
    }
}

public static class Archive
{
    private static GetApplicableArchivePaths GetApplicableArchivePathsDi(
        GameRelease release, 
        DirectoryPath dataFolderPath,
        IFileSystem? fileSystem)
    {
        var cont = new ArchiveContainer(
            fileSystem: fileSystem.GetOrDefault(),
            new GameReleaseInjection(release),
            new DataDirectoryInjection(dataFolderPath));
        return cont.Resolve().Value;
    }
    
    /// <summary>
    /// Returns the preferred extension (.bsa/.ba2) depending on the Game Release
    /// </summary>
    /// <param name="release"></param>
    /// <returns>Archive extension used by the given game release, with period delimiter.</returns>
    public static string GetExtension(GameRelease release)
    {
        switch (release.ToCategory())
        {
            case GameCategory.Oblivion:
            case GameCategory.Skyrim:
                return ".bsa";
            case GameCategory.Fallout4:
                return ".ba2";
            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Creates an Archive reader object from the given path, for the given Game Release.
    /// </summary>
    /// <param name="release">GameRelease the archive is for</param>
    /// <param name="path">Path to create archive reader from</param>
    /// <returns>Archive reader object</returns>
    public static IArchiveReader CreateReader(GameRelease release, FilePath path, IFileSystem? fileSystem = null)
    {
        return new ArchiveReaderProvider(
                fileSystem.GetOrDefault(),
                new GameReleaseInjection(release))
            .Create(path);
    }

    /// <summary>
    /// Enumerates all Archives for a given release that are within a given dataFolderPath.
    /// </summary>
    /// <param name="release">GameRelease to query for</param>
    /// <param name="dataFolderPath">Folder to query within</param>
    /// <param name="fileSystem">FileSystem to use</param>
    /// <param name="returnEmptyIfMissing">If ini file is missing, return empty instead of throwing an exception</param>
    /// <returns></returns>
    public static IEnumerable<FilePath> GetApplicableArchivePaths(
        GameRelease release, DirectoryPath dataFolderPath, IFileSystem? fileSystem = null, 
        bool returnEmptyIfMissing = true)
    {
        return GetApplicableArchivePathsDi(release, dataFolderPath, fileSystem: fileSystem)
            .Get();
    }

    /// <summary>
    /// Enumerates all applicable Archives for a given release and ModKey that are within a given dataFolderPath.<br/>
    /// This call is intended to return Archives related to one specific mod.<br/>
    /// NOTE:  It is currently a bit experimental
    /// </summary>
    /// <param name="release">GameRelease to query for</param>
    /// <param name="dataFolderPath">Folder to query within</param>
    /// <param name="modKey">ModKey to query about</param>
    /// <param name="fileSystem">FileSystem to use</param>
    /// <returns></returns>
    public static IEnumerable<FilePath> GetApplicableArchivePaths(GameRelease release, DirectoryPath dataFolderPath,
        ModKey modKey, IFileSystem? fileSystem = null, bool returnEmptyIfMissing = true)
    {
        return GetApplicableArchivePathsDi(release, dataFolderPath, fileSystem: fileSystem)
            .Get(modKey);
    }

    /// <summary>
    /// Analyzes whether an Archive would typically apply to a given ModKey. <br />
    ///  <br />
    /// - Is extension of the proper type <br />
    /// - Does the name match <br />
    /// - Does the name match, with an extra ` - AssetType` suffix considered
    /// </summary>
    /// <param name="release">Game Release of mod</param>
    /// <param name="modKey">ModKey to check applicability for</param>
    /// <param name="archiveFileName">Filename of the Archive, with extension</param>
    /// <returns>True if Archive is typically applicable to the given ModKey</returns>
    public static bool IsApplicable(GameRelease release, ModKey modKey, FileName archiveFileName)
    {
        return new CheckArchiveApplicability(
                new ArchiveExtensionProvider(
                    new GameReleaseInjection(release)))
            .IsApplicable(modKey, archiveFileName);
    }

    /// <summary>
    /// Queries the related ini file and looks for Archive ordering information
    /// </summary>
    /// <param name="release">GameRelease to query for</param>
    /// <param name="fileSystem">FileSystem to use</param>
    /// <returns>Any Archive ordering info retrieved from the ini definition</returns>
    public static IEnumerable<FileName> GetIniListings(GameRelease release, IFileSystem? fileSystem = null)
    {
        return new GetArchiveIniListingsContainer(fileSystem, release)
            .Resolve().Value
            .Get();
    }

    /// <summary>
    /// Queries the related ini file and looks for Archive ordering information
    /// </summary>
    /// <param name="release">GameRelease to query for</param>
    /// <param name="path">Path to the file containing INI data</param>
    /// <param name="fileSystem">FileSystem to use</param>
    /// <returns>Any Archive ordering info retrieved from the ini definition</returns>
    public static IEnumerable<FileName> GetIniListings(GameRelease release, FilePath path, IFileSystem? fileSystem = null)
    {
        return new GetArchiveIniListingsContainer(fileSystem, release)
            .Resolve().Value
            .Get(path);
    }

    /// <summary>
    /// Queries the related ini file and looks for Archive ordering information
    /// </summary>
    /// <param name="release">GameRelease ini is for</param>
    /// <param name="iniStream">Stream containing INI data</param>
    /// <param name="fileSystem">FileSystem to use</param>
    /// <returns>Any Archive ordering info retrieved from the ini definition</returns>
    public static IEnumerable<FileName> GetIniListings(GameRelease release, Stream iniStream, IFileSystem? fileSystem = null)
    {
        return new GetArchiveIniListingsContainer(fileSystem, release)
            .Resolve().Value
            .Get(iniStream);
    }
}