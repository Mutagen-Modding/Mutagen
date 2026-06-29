using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Analysis;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Order;
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

        // Collect all split ModKeys so they can be added to the load order for master sorting.
        var splitParam = AugmentParamsWithSplitModKeys(param, splitMods);

        // Write each split mod
        foreach (var splitMod in splitMods)
        {
            var splitPath = Path.Combine(Path.GetDirectoryName(path)!, splitMod.ModKey.FileName);

            // Write the split mod using WriteToBinary (TMod implements IMod which has WriteToBinary)
            splitMod.WriteToBinary(splitPath, splitParam);
        }
    }

    private BinaryWriteParameters AugmentParamsWithSplitModKeys<TMod>(
        BinaryWriteParameters param,
        IReadOnlyCollection<TMod> splitMods)
        where TMod : IModGetter
    {
        if (param.MastersListOrdering is not MastersListOrderingByLoadOrder loadOrderOrdering)
            return param;

        var splitModKeys = splitMods.Select(m => m.ModKey).ToList();

        ValidateSplitKeyOrder(loadOrderOrdering.LoadOrder, splitModKeys);

        // Only augment if there are split keys not already in the load order
        var existingKeys = loadOrderOrdering.LoadOrder.ToHashSet();
        var missingKeys = splitModKeys.Where(k => !existingKeys.Contains(k)).ToList();
        if (missingKeys.Count == 0)
            return param;

        // Append split file ModKeys to the end of the existing load order
        var augmentedOrder = loadOrderOrdering.LoadOrder.Concat(missingKeys);
        return param with
        {
            MastersListOrdering = new MastersListOrderingByLoadOrder(augmentedOrder)
        };
    }

    private static void ValidateSplitKeyOrder(
        IReadOnlyList<ModKey> loadOrderKeys,
        List<ModKey> splitModKeys)
    {
        var loadOrder = new LoadOrder<LoadOrderListing>(
            loadOrderKeys.Select(k => new LoadOrderListing(k, enabled: true)));

        // Find the indices of split keys that are present in the load order
        var presentSplitKeys = new List<(int splitIndex, int loadOrderIndex, ModKey key)>();
        for (int i = 0; i < splitModKeys.Count; i++)
        {
            var loIndex = loadOrder.IndexOf(splitModKeys[i]);
            if (loIndex >= 0)
            {
                presentSplitKeys.Add((i, loIndex, splitModKeys[i]));
            }
        }

        // If fewer than 2 split keys are present, there's nothing to validate
        if (presentSplitKeys.Count < 2)
            return;

        // Verify that the load order positions increase monotonically with the split index
        for (int i = 1; i < presentSplitKeys.Count; i++)
        {
            var prev = presentSplitKeys[i - 1];
            var curr = presentSplitKeys[i];
            if (curr.loadOrderIndex <= prev.loadOrderIndex)
            {
                throw new SplitModException(
                    $"Split mod files are out of order in the load order: " +
                    $"'{prev.key.FileName}' (index {prev.loadOrderIndex}) " +
                    $"appears before '{curr.key.FileName}' (index {curr.loadOrderIndex}), " +
                    $"but '{curr.key.FileName}' should come after '{prev.key.FileName}'. " +
                    $"Please ensure split files are ordered: base, _2, _3, etc.");
            }
        }
    }

    private FilePath GetSplitFilePath(FilePath originalPath, int index)
    {
        if (index == 0)
        {
            return originalPath;  // First file keeps original name
        }

        var directory = Path.GetDirectoryName(originalPath.Path) ?? string.Empty;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalPath.Path);
        var extension = Path.GetExtension(originalPath.Path);

        // index 1 → _2, index 2 → _3, etc.
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

        // Use wildcard pattern to find all split files: {basename}_*{extension}
        var searchPattern = $"{fileNameWithoutExtension}_*{extension}";

        foreach (var filePath in fileSystem.Directory.EnumerateFiles(directory, searchPattern))
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);

            // With new naming: currentSplitCount=2 means files are Name.esp + Name_2.esp
            // Delete files where suffix number > currentSplitCount (so _3, _4, etc.)
            if (MultiModFileAnalysis.IsSplitFileName(fileName, fileNameWithoutExtension, out var splitNumber)
                && splitNumber > currentSplitCount)
            {
                try
                {
                    fileSystem.File.Delete(filePath);
                }
                catch
                {
                    // Ignore deletion failures - file might be in use or locked
                    // We don't want to fail the entire write operation because of cleanup
                }
            }
        }
    }
}
