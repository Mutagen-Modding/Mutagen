using System.IO.Abstractions;
using Noggog;

namespace Mutagen.Bethesda.Plugins.IO.DI;

public interface IAssociatedFilesLocator
{
    /// <summary>
    /// Locates all existing associated files for a given mod: <br />
    /// - Mod itself <br />
    /// - Strings files <br />
    /// - Archive files
    /// </summary>
    /// <param name="modPath">Path to mod</param>
    /// <param name="categories">Types of files to look for</param>
    /// <returns>Enumerable of existing files associated with mod</returns>
    IEnumerable<FilePath> GetAssociatedFiles(ModPath modPath, AssociatedModFileCategory? categories = null);
}

public class AssociatedFilesLocator : IAssociatedFilesLocator
{
    private readonly IFileSystem _fileSystem;

    public AssociatedFilesLocator(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
    
    public IEnumerable<FilePath> GetAssociatedFiles(ModPath modPath, AssociatedModFileCategory? categories = null)
    {
        categories ??= AssociatedModFileCategory.Archives | AssociatedModFileCategory.Plugin | AssociatedModFileCategory.RawStrings;

        if (categories.Value.HasFlag(AssociatedModFileCategory.Plugin))
        {
            if (_fileSystem.File.Exists(modPath))
            {
                yield return modPath;
            }
        }

        var dir = modPath.Path.Directory!;
        if (!_fileSystem.Directory.Exists(dir)) yield break;

        if (categories.Value.HasFlag(AssociatedModFileCategory.RawStrings))
        {
            var stringsFolder = Path.Combine(dir, "Strings");
            if (_fileSystem.Directory.Exists(stringsFolder))
            {
                foreach (var stringsFile in _fileSystem.Directory.EnumerateFiles(stringsFolder,
                             $"{modPath.ModKey.Name}_*.*strings"))
                {
                    yield return stringsFile;
                }
            }
        }

        if (categories.Value.HasFlag(AssociatedModFileCategory.Archives))
        {
            foreach (var bsa in EnumerateBsas(dir.Value, modPath.ModKey, "bsa")
                         .And(EnumerateBsas(dir.Value, modPath.ModKey, "ba2")))
            {
                yield return bsa;
            }
        }
    }

    private IEnumerable<FilePath> EnumerateBsas(
        DirectoryPath dir,
        ModKey modKey, 
        string bsaSuffix)
    {
        var bsaPath = new FilePath(Path.Combine(dir, $"{modKey.Name}.{bsaSuffix}"));
        if (_fileSystem.File.Exists(bsaPath))
        {
            yield return bsaPath;
        }
        foreach (var bsa in _fileSystem.Directory.EnumerateFiles(dir, $"{modKey.Name} - *.{bsaSuffix}"))
        {
            yield return bsa;
        }
    }
}