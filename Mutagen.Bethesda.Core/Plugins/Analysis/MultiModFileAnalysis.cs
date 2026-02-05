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
                $"Found only one split file for {modKey}. Expected at least 2 split files (base and _2).");
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
                $"Found only one split file for {modKey}. Expected at least 2 split files (base and _2).");
        }

        return splitFiles;
    }

    /// <summary>
    /// Detects split files matching the pattern ModKey.ext, ModKey_2.ext, ModKey_3.ext, etc.
    /// First file is the base (no suffix), subsequent files have _2, _3, etc.
    /// </summary>
    internal static List<FilePath> DetectSplitFiles(DirectoryPath folder, ModKey modKey, IFileSystem fileSystem)
    {
        var splitFiles = new List<FilePath>();
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(modKey.FileName);
        var extension = Path.GetExtension(modKey.FileName);

        // Check for _2 to determine if this is a split set
        var secondFile = Path.Combine(folder.Path, $"{fileNameWithoutExtension}_2{extension}");
        if (!fileSystem.File.Exists(secondFile))
        {
            return splitFiles;  // No split files (or just a normal single file)
        }

        // First file is the base (no suffix)
        var baseFile = Path.Combine(folder.Path, modKey.FileName);
        if (!fileSystem.File.Exists(baseFile))
        {
            return splitFiles;  // Missing base file - invalid state
        }

        // It's a split set - add base file and find all numbered files
        splitFiles.Add(baseFile);

        int index = 2;
        while (true)
        {
            var splitFileName = $"{fileNameWithoutExtension}_{index}{extension}";
            var splitPath = Path.Combine(folder.Path, splitFileName);

            if (!fileSystem.File.Exists(splitPath)) break;

            splitFiles.Add(splitPath);
            index++;
        }

        return splitFiles;
    }
}
