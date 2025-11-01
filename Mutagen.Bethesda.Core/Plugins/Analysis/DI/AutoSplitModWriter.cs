using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Analysis.DI;

/// <summary>
/// Component that handles writing mods with automatic splitting when master limits are exceeded.
/// Detects TooManyMastersException and automatically splits the mod into multiple files.
/// </summary>
public class AutoSplitModWriter : IAutoSplitModWriter
{
    private readonly IMultiModFileSplitter _splitter;

    public AutoSplitModWriter(IMultiModFileSplitter splitter)
    {
        _splitter = splitter;
    }

    /// <summary>
    /// Writes a mod to the specified path, automatically splitting if master limit is exceeded.
    /// </summary>
    /// <typeparam name="TMod">Mutable mod type</typeparam>
    /// <typeparam name="TModGetter">Getter mod type</typeparam>
    /// <param name="mod">The mod to write</param>
    /// <param name="path">Target file path</param>
    /// <param name="param">Binary write parameters</param>
    public void Write<TMod, TModGetter>(
        TModGetter mod,
        FilePath path,
        BinaryWriteParameters param)
        where TMod : class, IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TModGetter : class, IModGetter
    {
        var fileSystem = param.FileSystem.GetOrDefault();

        try
        {
            // Try normal write first
            mod.WriteToBinary(path, param);
        }
        catch (TooManyMastersException)
        {
            // Split and write multiple files
            WriteWithSplit<TMod, TModGetter>(mod, path, param, fileSystem);
        }
    }

    private void WriteWithSplit<TMod, TModGetter>(
        TModGetter mod,
        FilePath path,
        BinaryWriteParameters param,
        IFileSystem fileSystem)
        where TMod : class, IMod, TModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TModGetter : class, IModGetter
    {
        // Cast to mutable type for splitting
        if (mod is not TMod mutableMod)
        {
            throw new ArgumentException(
                $"Mod must be of mutable type {typeof(TMod).Name} to support auto-splitting, but was {mod.GetType().Name}");
        }

        // Split the mod using the MultiModFileSplitter
        var splitMods = _splitter.Split<TMod, TModGetter>(mutableMod, Constants.PluginMasterLimit);

        // Clean up old split files that are no longer needed
        CleanupOldSplitFiles(path, splitMods.Count, fileSystem);

        // Write each split mod
        for (int i = 0; i < splitMods.Count; i++)
        {
            var splitMod = splitMods.ElementAt(i);
            var splitPath = GetSplitFilePath(path, i);

            // Modify parameters to correct ModKey to path since split files have different names
            var splitParam = param with { ModKey = ModKeyOption.CorrectToPath };

            // Write the split mod using WriteToBinary (TMod implements IMod which has WriteToBinary)
            splitMod.WriteToBinary(splitPath, splitParam);
        }
    }

    private FilePath GetSplitFilePath(FilePath originalPath, int index)
    {
        var directory = Path.GetDirectoryName(originalPath.Path) ?? string.Empty;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalPath.Path);
        var extension = Path.GetExtension(originalPath.Path);

        // First file gets _1, second gets _2, etc.
        var splitFileName = $"{fileNameWithoutExtension}_{index + 1}{extension}";
        return Path.Combine(directory, splitFileName);
    }

    private void CleanupOldSplitFiles(FilePath originalPath, int currentSplitCount, IFileSystem fileSystem)
    {
        var directory = Path.GetDirectoryName(originalPath.Path);
        if (string.IsNullOrEmpty(directory))
        {
            directory = Directory.GetCurrentDirectory();
        }

        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalPath.Path);
        var extension = Path.GetExtension(originalPath.Path);

        // Look for files with pattern: {basename}_{N}{extension} where N > currentSplitCount
        // Start checking from currentSplitCount + 1 onwards
        for (int i = currentSplitCount + 1; i <= 100; i++) // Check up to 100 to avoid infinite loop
        {
            var potentialOldFile = Path.Combine(directory, $"{fileNameWithoutExtension}_{i}{extension}");

            if (fileSystem.File.Exists(potentialOldFile))
            {
                try
                {
                    fileSystem.File.Delete(potentialOldFile);
                }
                catch
                {
                    // Ignore deletion failures - file might be in use or locked
                    // We don't want to fail the entire write operation because of cleanup
                }
            }
            else
            {
                // If we find a gap in numbering, stop looking
                // (e.g., if _5 doesn't exist, no point checking _6, _7, etc.)
                break;
            }
        }
    }
}
