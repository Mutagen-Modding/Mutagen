using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
namespace Mutagen.Bethesda.Assets.DI;

public class DataDirectoryAssetProvider : IAssetProvider
{
    private readonly IFileSystem _fileSystem;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;

    public DataDirectoryAssetProvider(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider)
    {
        _fileSystem = fileSystem;
        _dataDirectoryProvider = dataDirectoryProvider;
    }

    public bool Exists(DataRelativeAssetPath assetPath)
    {
        var fullPath = _fileSystem.Path.Combine(_dataDirectoryProvider.Path, assetPath.Path);
        return _fileSystem.File.Exists(fullPath);
    }

    public bool TryGetSize(DataRelativeAssetPath assetPath, out uint size)
    {
        var fullPath = _fileSystem.Path.Combine(_dataDirectoryProvider.Path, assetPath.Path);
        if (!_fileSystem.File.Exists(fullPath))
        {
            size = 0;
            return false;
        }

        size = (uint) _fileSystem.FileInfo.New(fullPath).Length;
        return true;
    }

    public bool TryGetStream(DataRelativeAssetPath assetPath, [MaybeNullWhen(false)] out Stream stream)
    {
        var fullPath = _fileSystem.Path.Combine(_dataDirectoryProvider.Path, assetPath.Path);
        if (_fileSystem.File.Exists(fullPath))
        {
            stream = _fileSystem.File.OpenRead(fullPath);
            return true;
        }

        stream = null;
        return false;
    }
}
