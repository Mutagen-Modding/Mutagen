using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.IO.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.IO;

public static class PluginUtilityIO
{
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