using System.IO.Abstractions;
using Noggog;

namespace Mutagen.Bethesda.Plugins.IO.DI;

public interface IModFilesMover
{
    /// <summary>
    /// Moves a mod and all associated files to new target location. <br/>
    /// This acts as an "overwrite", for even the related associated files.  This means
    /// that if you overwrite a localized mod with an unlocalized one, the old strings files
    /// will be removed.
    /// </summary>
    /// <param name="pathToPlugin">Path to the plugin to move</param>
    /// <param name="newDirectory">Directory to move the plugin to, and cleanup old files within</param>
    /// <param name="overwrite">
    /// If false, this will throw if a conflicting mod exists in the output directory. <br />
    /// If true, it will overwrite and delete all files associated with the replaced mod
    /// </param>
    /// <param name="categories">Types of files to process</param>
    void MoveModTo(
        ModPath pathToPlugin,
        DirectoryPath newDirectory,
        bool overwrite = false,
        AssociatedModFileCategory? categories = null);
    
    /// <summary>
    /// Copies a mod and all associated files to new target location. <br/>
    /// This acts as an "overwrite", for even the related associated files.  This means
    /// that if you overwrite a localized mod with an unlocalized one, the old strings files
    /// will be removed.
    /// </summary>
    /// <param name="pathToPlugin">Path to the plugin to copy</param>
    /// <param name="newDirectory">Directory to copy the plugin to, and cleanup old files within</param>
    /// <param name="overwrite">
    /// If false, this will throw if a conflicting mod exists in the output directory. <br />
    /// If true, it will overwrite and delete all files associated with the replaced mod
    /// </param>
    /// <param name="categories">Types of files to process</param>
    void CopyModTo(
        ModPath pathToPlugin,
        DirectoryPath newDirectory,
        bool overwrite = false,
        AssociatedModFileCategory? categories = null);
}

public class ModFilesMover : IModFilesMover
{
    private readonly IFileSystem _fileSystem;
    private readonly IAssociatedFilesLocator _associatedFilesLocator;

    public ModFilesMover(
        IFileSystem fileSystem,
        IAssociatedFilesLocator associatedFilesLocator)
    {
        _fileSystem = fileSystem;
        _associatedFilesLocator = associatedFilesLocator;
    }

    public void MoveModTo(
        ModPath pathToPlugin,
        DirectoryPath newDirectory,
        bool overwrite = false,
        AssociatedModFileCategory? categories = null)
    {
        MoveOrCopyModTo(
            pathToPlugin: pathToPlugin,
            newDirectory: newDirectory,
            move: true,
            overwrite: overwrite,
            categories: categories);
    }

    public void CopyModTo(
        ModPath pathToPlugin,
        DirectoryPath newDirectory,
        bool overwrite = false,
        AssociatedModFileCategory? categories = null)
    {
        MoveOrCopyModTo(
            pathToPlugin: pathToPlugin,
            newDirectory: newDirectory,
            move: false,
            overwrite: overwrite,
            categories: categories);
    }

    private void MoveOrCopyModTo(
        ModPath pathToPlugin,
        DirectoryPath newDirectory,
        bool move,
        bool overwrite = false,
        AssociatedModFileCategory? categories = null)
    {
        var associatedSourceFiles = _associatedFilesLocator
            .GetAssociatedFiles(pathToPlugin, categories)
            .ToArray();
        var pathToNewPlugin = Path.Combine(newDirectory, pathToPlugin.Path.Name);
        var associatedTargetFiles = _associatedFilesLocator
            .GetAssociatedFiles(pathToNewPlugin, categories)
            .ToHashSet();

        if (!overwrite)
        {
            var associatedFile = associatedTargetFiles
                .Select<FilePath, FilePath?>(x => x)
                .FirstOrDefault();
            if (associatedFile != null)
            {
                throw new IOException($"Mod file already exists: {associatedFile}");
            }
        }
        
        foreach (var sourceFile in associatedSourceFiles)
        {
            var relPath = sourceFile.GetRelativePathTo(pathToPlugin.Path.Directory!.Value);
            FilePath newPath = Path.Combine(newDirectory, relPath);
            newPath.Directory?.Create(_fileSystem);
            if (move)
            {
                _fileSystem.File.Move(sourceFile, newPath, overwrite: true);
            }
            else
            {
                _fileSystem.File.Copy(sourceFile, newPath, overwrite: true);
            }
            associatedTargetFiles.Remove(newPath);
        }

        foreach (var oldTargetFile in associatedTargetFiles)
        {
            _fileSystem.File.Delete(oldTargetFile);
            if (oldTargetFile.Directory?.EnumerateFiles(recursive: true, fileSystem: _fileSystem).Any() ?? false)
            {
                _fileSystem.Directory.Delete(oldTargetFile.Directory);
            }
        }
    }
}