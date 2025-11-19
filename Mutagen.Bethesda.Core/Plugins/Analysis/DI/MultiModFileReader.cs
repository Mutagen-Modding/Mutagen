using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System.IO.Abstractions;

namespace Mutagen.Bethesda.Plugins.Analysis.DI;

/// <summary>
/// Reads multiple split mod files and merges them into a single mod object.
/// Used to read mods that were split due to exceeding master limits.
/// </summary>
public interface IMultiModFileReader
{
    /// <summary>
    /// Reads split mod files from a directory and returns a multi-file overlay that presents them as a single unified mod.
    /// The returned mod is read-only (implements IModGetter only).
    /// </summary>
    /// <typeparam name="TModGetter">Getter mod type</typeparam>
    /// <param name="folder">Directory containing the split mod files</param>
    /// <param name="modKey">The base ModKey to look for (without _1, _2 suffixes)</param>
    /// <param name="gameRelease">The game release for the mods</param>
    /// <param name="loadOrder">Load order to use for master ordering</param>
    /// <param name="readParams">Binary read parameters</param>
    /// <returns">Merged mod overlay with the original ModKey (read-only)</returns>
    TModGetter Read<TModGetter>(
        DirectoryPath folder,
        ModKey modKey,
        GameRelease gameRelease,
        IEnumerable<IModMasterStyledGetter> loadOrder,
        BinaryReadParameters readParams)
        where TModGetter : class, IModDisposeGetter;
}

/// <summary>
/// Implementation of multi-mod file reader
/// </summary>
public class MultiModFileReader : IMultiModFileReader
{
    /// <summary>
    /// Reads split mod files from a directory and returns a multi-file overlay.
    /// </summary>
    public TModGetter Read<TModGetter>(
        DirectoryPath folder,
        ModKey modKey,
        GameRelease gameRelease,
        IEnumerable<IModMasterStyledGetter> loadOrder,
        BinaryReadParameters readParams)
        where TModGetter : class, IModDisposeGetter
    {
        var fileSystem = readParams.FileSystem.GetOrDefault();

        // Detect split files
        var splitFiles = DetectSplitFiles(folder, modKey, fileSystem);

        if (splitFiles.Count == 0)
        {
            throw new SplitModException($"No split files found for {modKey} in {folder}");
        }

        // Validate consecutive numbering
        ValidateConsecutiveNumbering(splitFiles, modKey);

        // Create a ModImporter to handle the multi-file import
        var gameReleaseContext = new GameReleaseInjection(gameRelease);
        var modImporter = new Records.DI.ModImporter(fileSystem, gameReleaseContext);

        // Use ModImporter to read and merge the split files
        return modImporter.ImportMultiFile<TModGetter>(modKey, splitFiles.Select(f => (ModPath)f.Path), loadOrder, readParams);
    }

    /// <summary>
    /// Detects split files matching the pattern ModKey_1.ext, ModKey_2.ext, etc.
    /// </summary>
    private List<FilePath> DetectSplitFiles(DirectoryPath folder, ModKey modKey, IFileSystem fileSystem)
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

    /// <summary>
    /// Validates that split files are numbered consecutively with no gaps
    /// </summary>
    private void ValidateConsecutiveNumbering(List<FilePath> splitFiles, ModKey modKey)
    {
        if (splitFiles.Count == 0)
        {
            return;
        }

        // Files should be numbered 1, 2, 3, ... N with no gaps
        // This is implicitly validated by DetectSplitFiles which stops at the first missing file
        // But we can add extra validation here if needed

        if (splitFiles.Count < 2)
        {
            throw new SplitModException(
                $"Found only one split file for {modKey}. Expected at least 2 split files (_1 and _2).");
        }
    }

}
