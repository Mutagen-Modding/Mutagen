using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.IO.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.IO;

public static class PluginUtilityIO
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
    /// <param name="fileSystem">FileSystem to do operation on</param>
    public static void MoveModTo(
        ModPath pathToPlugin,
        DirectoryPath newDirectory,
        bool overwrite = false,
        AssociatedModFileCategory? categories = null,
        IFileSystem? fileSystem = null)
    {
        var loc = new AssociatedFilesLocator(fileSystem.GetOrDefault());
        var mover = new ModFilesMover(fileSystem.GetOrDefault(), loc);
        mover.MoveModTo(pathToPlugin, newDirectory, overwrite, categories);
    }

    /// <summary>
    /// Locates all existing associated files for a given mod: <br />
    /// - Mod itself <br />
    /// - Strings files <br />
    /// - Archive files
    /// </summary>
    /// <param name="modPath">Path to mod</param>
    /// <param name="categories">Types of files to look for</param>
    /// <param name="fileSystem">FileSystem to do operation on</param>
    /// <returns>Enumerable of existing files associated with mod</returns>
    public static IEnumerable<FilePath> GetAssociatedFiles(
        ModPath modPath, 
        AssociatedModFileCategory? categories = null,
        IFileSystem? fileSystem = null)
    {
        var loc = new AssociatedFilesLocator(fileSystem.GetOrDefault());
        return loc.GetAssociatedFiles(modPath, categories);
    }
}