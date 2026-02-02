using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Analysis;

/// <summary>
/// Static utility methods for analyzing multi-mod (split) files.
/// </summary>
public static class MultiModFileAnalysis
{
    /// <summary>
    /// Checks if split mod files exist for the given mod path.
    /// </summary>
    /// <param name="modPath">The mod path to check for split files</param>
    /// <param name="fileSystem">Optional file system to use. Defaults to real file system.</param>
    /// <returns>True if valid split files exist, false if no split files exist (single mod)</returns>
    /// <exception cref="SplitModException">
    /// Thrown if only one split file exists, or if both split files and original mod exist
    /// </exception>
    public static bool IsMultiModFile(ModPath modPath, IFileSystem? fileSystem = null)
    {
        fileSystem = fileSystem.GetOrDefault();
        var folder = new DirectoryPath(Path.GetDirectoryName(modPath.Path) ?? ".");
        var modKey = modPath.ModKey;
        var splitFiles = DetectSplitFiles(folder, modKey, fileSystem);

        if (splitFiles.Count == 0)
        {
            return false;
        }

        if (splitFiles.Count == 1)
        {
            throw new SplitModException(
                $"Found only one split file for {modKey}. Expected at least 2 split files (_1 and _2).");
        }

        // Check if original unsplit mod exists - this is an error condition
        if (fileSystem.File.Exists(modPath.Path))
        {
            throw new SplitModException(
                $"Found both split files and original mod file for {modKey}. This is an invalid state.");
        }

        return true;
    }

    /// <summary>
    /// Gets the list of split mod files for the given mod path.
    /// Throws if the split files are in an invalid state.
    /// </summary>
    /// <param name="modPath">The mod path to check for split files</param>
    /// <param name="fileSystem">Optional file system to use. Defaults to real file system.</param>
    /// <returns>List of split file paths in order, or empty list if no split files exist</returns>
    /// <exception cref="SplitModException">
    /// Thrown if only one split file exists, or if both split files and original mod exist
    /// </exception>
    public static List<FilePath> GetSplitModFiles(ModPath modPath, IFileSystem? fileSystem = null)
    {
        fileSystem = fileSystem.GetOrDefault();
        var folder = new DirectoryPath(Path.GetDirectoryName(modPath.Path) ?? ".");
        var modKey = modPath.ModKey;
        var splitFiles = DetectSplitFiles(folder, modKey, fileSystem);

        if (splitFiles.Count == 0)
        {
            return splitFiles;
        }

        if (splitFiles.Count == 1)
        {
            throw new SplitModException(
                $"Found only one split file for {modKey}. Expected at least 2 split files (_1 and _2).");
        }

        // Check if original unsplit mod exists - this is an error condition
        if (fileSystem.File.Exists(modPath.Path))
        {
            throw new SplitModException(
                $"Found both split files and original mod file for {modKey}. This is an invalid state.");
        }

        return splitFiles;
    }

    /// <summary>
    /// Detects split files matching the pattern ModKey_1.ext, ModKey_2.ext, etc.
    /// </summary>
    internal static List<FilePath> DetectSplitFiles(DirectoryPath folder, ModKey modKey, IFileSystem fileSystem)
    {
        var splitFiles = new List<FilePath>();
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);

        int index = 1;
        while (true)
        {
            var splitFileName = $"{fileNameWithoutExtension}_{index}{extension}";
            var splitPath = Path.Combine(folder.Path, splitFileName);

            if (!fileSystem.File.Exists(splitPath))
            {
                break;
            }

            splitFiles.Add(splitPath);
            index++;
        }

        return splitFiles;
    }
}
