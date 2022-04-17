using IniParser;
using IniParser.Model.Configuration;
using IniParser.Parser;
using Mutagen.Bethesda.Archives.Ba2;
using Mutagen.Bethesda.Archives.Bsa;
using Mutagen.Bethesda.Inis;
using Mutagen.Bethesda.Plugins;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Stream = System.IO.Stream;
using StreamReader = System.IO.StreamReader;

namespace Mutagen.Bethesda.Archives;

public static class Archive
{
    private static GetApplicableArchivePaths GetApplicableArchivePathsDi(GameRelease release, DirectoryPath dataFolderPath)
    {
        var gameReleaseInjection = new GameReleaseInjection(release);
        var ext = new ArchiveExtensionProvider(gameReleaseInjection);
        return new GetApplicableArchivePaths(
            IFileSystemExt.DefaultFilesystem,
            new GetArchiveIniListings(
                IFileSystemExt.DefaultFilesystem,
                gameReleaseInjection),
            new CheckArchiveApplicability(
                ext),
            new DataDirectoryInjection(dataFolderPath),
            ext);
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
    public static IArchiveReader CreateReader(GameRelease release, FilePath path)
    {
        return new ArchiveReaderProvider(
                IFileSystemExt.DefaultFilesystem,
                new GameReleaseInjection(release))
            .Create(path);
    }

    /// <summary>
    /// Enumerates all Archives for a given release that are within a given dataFolderPath.
    /// </summary>
    /// <param name="release">GameRelease to query for</param>
    /// <param name="dataFolderPath">Folder to query within</param>
    /// <returns></returns>
    public static IEnumerable<FilePath> GetApplicableArchivePaths(GameRelease release, DirectoryPath dataFolderPath)
    {
        return GetApplicableArchivePathsDi(release, dataFolderPath)
            .Get();
    }

    /// <summary>
    /// Enumerates all Archives for a given release that are within a given dataFolderPath.
    /// </summary>
    /// <param name="release">GameRelease to query for</param>
    /// <param name="dataFolderPath">Folder to query within</param>
    /// <param name="archiveOrdering">Archive ordering overload.  Empty enumerable means no ordering.</param>
    /// <returns></returns>
    public static IEnumerable<FilePath> GetApplicableArchivePaths(GameRelease release, DirectoryPath dataFolderPath, IEnumerable<FileName>? archiveOrdering)
    {
        return GetApplicableArchivePathsDi(release, dataFolderPath)
            .Get(archiveOrdering);
    }

    /// <summary>
    /// Enumerates all applicable Archives for a given release and ModKey that are within a given dataFolderPath.<br/>
    /// This call is intended to return Archives related to one specific mod.<br/>
    /// NOTE:  It is currently a bit experimental
    /// </summary>
    /// <param name="release">GameRelease to query for</param>
    /// <param name="dataFolderPath">Folder to query within</param>
    /// <param name="modKey">ModKey to query about</param>
    /// <returns></returns>
    public static IEnumerable<FilePath> GetApplicableArchivePaths(GameRelease release, DirectoryPath dataFolderPath, ModKey modKey)
    {
        return GetApplicableArchivePathsDi(release, dataFolderPath)
            .Get(modKey);
    }

    /// <summary>
    /// Enumerates all applicable Archives for a given release and ModKey that are within a given dataFolderPath.<br/>
    /// This call is intended to return Archives related to one specific mod.<br/>
    /// NOTE:  It is currently a bit experimental
    /// </summary>
    /// <param name="release">GameRelease to query for</param>
    /// <param name="dataFolderPath">Folder to query within</param>
    /// <param name="modKey">ModKey to query about</param>
    /// <param name="archiveOrdering">Archive ordering overload.  Empty enumerable means no ordering.</param>
    /// <returns></returns>
    public static IEnumerable<FilePath> GetApplicableArchivePaths(GameRelease release, DirectoryPath dataFolderPath, ModKey modKey, IEnumerable<FileName>? archiveOrdering)
    {
        return GetApplicableArchivePathsDi(release, dataFolderPath)
            .Get(modKey, archiveOrdering);
    }

    /// <summary>
    /// Enumerates all applicable Archives for a given release and ModKey that are within a given dataFolderPath.<br/>
    /// This call is intended to return Archives related to one specific mod.<br/>
    /// NOTE:  It is currently a bit experimental
    /// </summary>
    /// <param name="release">GameRelease to query for</param>
    /// <param name="dataFolderPath">Folder to query within</param>
    /// <param name="modKey">ModKey to query about</param>
    /// <param name="archiveOrdering">How to order the archive paths.  Null for no ordering</param>
    /// <returns>Full paths of Archives that apply to the given mod and exist</returns>
    public static IEnumerable<FilePath> GetApplicableArchivePaths(GameRelease release, DirectoryPath dataFolderPath, ModKey modKey, IComparer<FileName>? archiveOrdering)
    {
        return GetApplicableArchivePathsDi(release, dataFolderPath)
            .Get(modKey, archiveOrdering);
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
    /// <returns>Any Archive ordering info retrieved from the ini definition</returns>
    public static IEnumerable<FileName> GetIniListings(GameRelease release)
    {
        return new GetArchiveIniListings(
                IFileSystemExt.DefaultFilesystem,
                new GameReleaseInjection(release))
            .Get();
    }

    /// <summary>
    /// Queries the related ini file and looks for Archive ordering information
    /// </summary>
    /// <param name="release">GameRelease to query for</param>
    /// <param name="path">Path to the file containing INI data</param>
    /// <returns>Any Archive ordering info retrieved from the ini definition</returns>
    public static IEnumerable<FileName> GetIniListings(GameRelease release, FilePath path)
    {
        return new GetArchiveIniListings(
                IFileSystemExt.DefaultFilesystem,
                new GameReleaseInjection(release))
            .Get(path);
    }

    /// <summary>
    /// Queries the related ini file and looks for Archive ordering information
    /// </summary>
    /// <param name="release">GameRelease ini is for</param>
    /// <param name="iniStream">Stream containing INI data</param>
    /// <returns>Any Archive ordering info retrieved from the ini definition</returns>
    public static IEnumerable<FileName> GetIniListings(GameRelease release, Stream iniStream)
    {
        return new GetArchiveIniListings(
                IFileSystemExt.DefaultFilesystem,
                new GameReleaseInjection(release))
            .Get(iniStream);
    }
}