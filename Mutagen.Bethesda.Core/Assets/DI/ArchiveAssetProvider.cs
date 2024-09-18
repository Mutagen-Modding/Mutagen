using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Assets.DI;

public class ArchiveAssetProvider : IAssetProvider
{
    private readonly IFileSystem _fileSystem;
    private readonly IGetApplicableArchivePaths _getApplicableArchivePaths;
    private readonly IGameReleaseContext _gameReleaseContext;

    public ArchiveAssetProvider(
        IFileSystem fileSystem,
        IGetApplicableArchivePaths getApplicableArchivePaths,
        IGameReleaseContext gameReleaseContext)
    {
        _fileSystem = fileSystem;
        _getApplicableArchivePaths = getApplicableArchivePaths;
        _gameReleaseContext = gameReleaseContext;
    }

    public bool Exists(DataRelativePath assetPath)
    {
        return GetArchiveFile(assetPath) is not null;
    }

    public bool TryGetStream(DataRelativePath assetPath, [MaybeNullWhen(false)] out Stream stream)
    {
        return TryGetStream(assetPath, null, out stream);
    }

    public bool TryGetStream(DataRelativePath assetPath, IEnumerable<ModKey>? modOrdering, [MaybeNullWhen(false)] out Stream stream)
    {
        var archiveFile = GetArchiveFile(assetPath, modOrdering);
        if (archiveFile is null)
        {
            stream = null;
            return false;
        }
        
        stream = archiveFile.AsStream();
        return true;
    }

    public bool TryGetSize(DataRelativePath assetPath, out uint size)
    {
        return TryGetSize(assetPath, null, out size);
    }

    public bool TryGetSize(DataRelativePath assetPath, IEnumerable<ModKey>? modOrdering, out uint size)
    {
        var archiveFile = GetArchiveFile(assetPath, modOrdering);
        if (archiveFile is null)
        {
            size = 0;
            return false;
        }
        
        size = archiveFile.Size;
        return true;
    }

    private IArchiveFile? GetArchiveFile(DataRelativePath assetPath, IEnumerable<ModKey>? modOrdering = null)
    {
        var dataRelativeDirectoryPath = _fileSystem.Path.GetDirectoryName(assetPath.Path);
        if (dataRelativeDirectoryPath is null) return null;

        var fileName = _fileSystem.Path.GetFileName(assetPath.Path);
        var applicableArchivePaths = _getApplicableArchivePaths.Get(modOrdering);
        foreach (var archivePath in applicableArchivePaths)
        {
            var archiveReader = Archive.CreateReader(_gameReleaseContext.Release, archivePath, _fileSystem);
            if (!archiveReader.TryGetFolder(dataRelativeDirectoryPath, out var archiveFolder)) continue;

            var archiveFile = archiveFolder.Files.FirstOrDefault(file =>
            {
                var archiveFileName = _fileSystem.Path.GetFileName(file.Path);
                return DataRelativePath.PathComparer.Equals(archiveFileName, fileName);
            });
            if (archiveFile is null) continue;

            return archiveFile;
        }

        return null;
    }
}
